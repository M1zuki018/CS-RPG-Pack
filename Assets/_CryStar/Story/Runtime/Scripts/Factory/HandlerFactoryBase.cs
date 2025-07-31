using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryStar.Utility;
using CryStar.Utility.Enum;

namespace CryStar.Story.Factory
{
    /// <summary>
    /// ハンドラーファクトリのベースクラス
    /// 異なるタイプのハンドラーを統一的に管理・生成する
    /// </summary>
    /// <typeparam name="THandler">ハンドラーのベース型</typeparam>
    /// <typeparam name="TEnum">ハンドラータイプを表すEnum型</typeparam>
    /// <typeparam name="TAttribute">ハンドラーに付与される属性型</typeparam>
    public static class HandlerFactoryBase<THandler, TEnum, TAttribute>
        where THandler : class, IHandlerBase
        where TEnum : struct, Enum
        where TAttribute : System.Attribute, IHandlerAttribute<TEnum>
    {
        #region Fields

        /// <summary>
        /// ハンドラータイプとクラス型のマッピング
        /// </summary>
        private static readonly Dictionary<TEnum, Type> _handlerTypes = new();

        /// <summary>
        /// ファクトリが初期化済みかどうかのフラグ
        /// </summary>
        private static bool _isInitialized = false;

        /// <summary>
        /// 初期化時のスレッドセーフティを保証するロックオブジェクト
        /// </summary>
        private static readonly object _initializationLock = new();

        /// <summary>
        /// ハンドラータイプの名前（ログ用）
        /// </summary>
        private static readonly string _handlerTypeName = typeof(THandler).Name;

        #endregion

        #region Public Methods

        /// <summary>
        /// ファクトリを初期化し、利用可能なハンドラーを自動発見・登録する
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            lock (_initializationLock)
            {
                if (_isInitialized) return;

                try
                {
                    DiscoverAndRegisterHandlers();
                    _isInitialized = true;

                    LogUtility.Info($"{_handlerTypeName}ファクトリを初期化完了: {_handlerTypes.Count}個のハンドラーを登録",
                        LogCategory.System);
                }
                catch (Exception ex)
                {
                    LogUtility.Error($"{_handlerTypeName}ファクトリの初期化に失敗: {ex.Message}", LogCategory.System);
                    throw new InvalidOperationException($"{_handlerTypeName}ファクトリの初期化に失敗しました", ex);
                }
            }
        }

        /// <summary>
        /// 指定されたハンドラータイプに対応するインスタンスを生成する
        /// </summary>
        public static THandler CreateHandler(TEnum handlerType, params object[] constructorArgs)
        {
            EnsureInitialized();

            if (!_handlerTypes.TryGetValue(handlerType, out var type))
            {
                LogUtility.Warning($"未登録の{_handlerTypeName}タイプが指定されました: {handlerType}", LogCategory.System);
                return null;
            }

            return CreateHandlerInstance(type, constructorArgs);
        }

        /// <summary>
        /// 登録済みの全ハンドラータイプに対してインスタンスを生成する
        /// </summary>
        public static Dictionary<TEnum, THandler> CreateAllHandlers(Func<Type, object[]> dependencyResolver)
        {
            if (dependencyResolver == null)
                throw new ArgumentNullException(nameof(dependencyResolver));

            EnsureInitialized();

            var handlers = new Dictionary<TEnum, THandler>();
            var failedHandlers = new List<TEnum>();

            foreach (var (handlerType, classType) in _handlerTypes)
            {
                try
                {
                    var constructorArgs = dependencyResolver(classType);
                    var handler = CreateHandlerInstance(classType, constructorArgs);

                    if (handler != null)
                    {
                        handlers[handlerType] = handler;
                    }
                    else
                    {
                        failedHandlers.Add(handlerType);
                    }
                }
                catch (Exception ex)
                {
                    LogUtility.Error($"{_handlerTypeName}の生成に失敗: {handlerType} - {ex.Message}", LogCategory.System);
                    failedHandlers.Add(handlerType);
                }
            }

            if (failedHandlers.Count > 0)
            {
                LogUtility.Warning($"一部の{_handlerTypeName}の生成に失敗しました: [{string.Join(", ", failedHandlers)}]",
                    LogCategory.System);
            }

            LogUtility.Info($"{_handlerTypeName}生成完了: 成功 {handlers.Count}個, 失敗 {failedHandlers.Count}個",
                LogCategory.System);
            return handlers;
        }

        /// <summary>
        /// 現在登録されているハンドラータイプの数を取得する
        /// </summary>
        public static int GetRegisteredHandlerCount()
        {
            EnsureInitialized();
            return _handlerTypes.Count;
        }

        /// <summary>
        /// 指定されたハンドラータイプが登録済みかどうかを確認する
        /// </summary>
        public static bool IsHandlerRegistered(TEnum handlerType)
        {
            EnsureInitialized();
            return _handlerTypes.ContainsKey(handlerType);
        }

        /// <summary>
        /// 登録済みのハンドラータイプ一覧を取得する
        /// </summary>
        public static IReadOnlyCollection<TEnum> GetRegisteredHandlerTypes()
        {
            EnsureInitialized();
            return _handlerTypes.Keys.ToList().AsReadOnly();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// アセンブリからハンドラーを自動発見し、内部辞書に登録する
        /// </summary>
        private static void DiscoverAndRegisterHandlers()
        {
            _handlerTypes.Clear();

            var assemblies = GetLoadedAssemblies();
            var totalDiscovered = 0;
            var processedAssemblies = 0;

            foreach (var assembly in assemblies)
            {
                if (ShouldSkipAssembly(assembly))
                    continue;

                var discoveredCount = ProcessAssembly(assembly);
                totalDiscovered += discoveredCount;
                processedAssemblies++;

                if (discoveredCount > 0)
                {
                    LogUtility.Debug($"アセンブリ [{assembly.GetName().Name}] から {discoveredCount}個の{_handlerTypeName}を発見",
                        LogCategory.System);
                }
            }

            LogUtility.Info($"{_handlerTypeName}発見処理完了: {processedAssemblies}個のアセンブリを処理し、{totalDiscovered}個のハンドラーを登録",
                LogCategory.System);

            if (totalDiscovered == 0)
            {
                LogUtility.Warning($"{_handlerTypeName}が一つも発見されませんでした。{typeof(TAttribute).Name}が正しく付与されているか確認してください",
                    LogCategory.System);
            }
        }

        /// <summary>
        /// 現在ロードされているアセンブリを安全に取得する
        /// </summary>
        private static Assembly[] GetLoadedAssemblies()
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies();
            }
            catch (Exception ex)
            {
                LogUtility.Error($"アセンブリの取得に失敗: {ex.Message}", LogCategory.System);
                return Array.Empty<Assembly>();
            }
        }

        /// <summary>
        /// 処理をスキップすべきアセンブリかどうかを判定する
        /// </summary>
        private static bool ShouldSkipAssembly(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;

            var systemPrefixes = new[]
            {
                "System.",
                "Microsoft.",
                "Unity.",
                "UnityEngine.",
                "UnityEditor.",
                "mscorlib",
                "netstandard"
            };

            return systemPrefixes.Any(prefix => assemblyName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                   || assembly.GlobalAssemblyCache;
        }

        /// <summary>
        /// 指定されたアセンブリからハンドラーを発見・登録する
        /// </summary>
        private static int ProcessAssembly(Assembly assembly)
        {
            try
            {
                var handlerTypes = FindHandlerTypes(assembly);
                return RegisterHandlerTypes(handlerTypes);
            }
            catch (ReflectionTypeLoadException ex)
            {
                LogUtility.Warning($"アセンブリの型読み込みエラー [{assembly.FullName}]: {ex.Message}", LogCategory.System);

                if (ex.Types != null)
                {
                    var validTypes = ex.Types.Where(t => t != null).ToArray();
                    var handlerTypes = FilterHandlerTypes(validTypes);
                    return RegisterHandlerTypes(handlerTypes);
                }

                return 0;
            }
            catch (Exception ex)
            {
                LogUtility.Warning($"アセンブリ処理エラー [{assembly.FullName}]: {ex.Message}", LogCategory.System);
                return 0;
            }
        }

        /// <summary>
        /// アセンブリからハンドラー候補の型を抽出する
        /// </summary>
        private static Type[] FindHandlerTypes(Assembly assembly)
        {
            var allTypes = assembly.GetTypes();
            return FilterHandlerTypes(allTypes);
        }

        /// <summary>
        /// 型配列からハンドラー条件に合致する型をフィルタリングする
        /// </summary>
        private static Type[] FilterHandlerTypes(Type[] types)
        {
            return types
                .Where(IsValidHandlerType)
                .ToArray();
        }

        /// <summary>
        /// 指定された型がハンドラーとして有効かどうかを判定する
        /// </summary>
        private static bool IsValidHandlerType(Type type)
        {
            return typeof(THandler).IsAssignableFrom(type)
                   && !type.IsInterface
                   && !type.IsAbstract
                   && type.GetCustomAttribute<TAttribute>() != null;
        }

        /// <summary>
        /// 発見されたハンドラー型を内部辞書に登録する
        /// </summary>
        private static int RegisterHandlerTypes(Type[] handlerTypes)
        {
            var registeredCount = 0;

            foreach (var type in handlerTypes)
            {
                var attribute = type.GetCustomAttribute<TAttribute>();
                if (attribute != null)
                {
                    if (_handlerTypes.ContainsKey(attribute.HandlerType))
                    {
                        LogUtility.Warning(
                            $"重複する{_handlerTypeName}タイプが検出されました: {attribute.HandlerType} - {type.Name}で上書きします",
                            LogCategory.System);
                    }

                    _handlerTypes[attribute.HandlerType] = type;
                    registeredCount++;
                }
            }

            return registeredCount;
        }

        /// <summary>
        /// ファクトリが初期化されていることを保証する
        /// </summary>
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }

        /// <summary>
        /// 指定された型とコンストラクタ引数を使用してハンドラーインスタンスを生成する
        /// </summary>
        private static THandler CreateHandlerInstance(Type handlerType, object[] constructorArgs)
        {
            try
            {
                return (THandler)Activator.CreateInstance(handlerType, constructorArgs);
            }
            catch (Exception ex)
            {
                LogUtility.Error($"{_handlerTypeName}インスタンスの生成に失敗: {handlerType.Name} - {ex.Message}",
                    LogCategory.System);
                return null;
            }
        }

        #endregion
    }
}