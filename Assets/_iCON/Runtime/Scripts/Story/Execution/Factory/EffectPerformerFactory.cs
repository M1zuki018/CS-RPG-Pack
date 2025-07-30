using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryStar.Story.Attributes;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using iCON.Utility;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// EffectPerformerのインスタンスを生成するFactory
    /// </summary>
    public static class EffectPerformerFactory
    {
        /// <summary>
        /// エフェクトタイプとハンドラータイプのマッピング
        /// </summary>
        private static Dictionary<EffectOrderType, Type> _handlerTypes;
        
        /// <summary>
        /// ファクトリが初期化済みかどうか
        /// </summary>
        private static bool _isInitialized = false;

        /// <summary>
        /// 自動でハンドラータイプを発見・登録
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            try
            {
                DiscoverAndRegisterHandlers();
                _isInitialized = true;
                    
                LogUtility.Info($"Effect Performerファクトリを初期化完了: {_handlerTypes.Count}個のハンドラーを登録", LogCategory.System);
            }
            catch (Exception ex)
            {
                LogUtility.Error($"Effect Performerファクトリの初期化に失敗: {ex.Message}", LogCategory.System);
                throw new InvalidOperationException("EffectPerformerFactoryの初期化に失敗しました", ex);
            }
        }
        
        /// <summary>
        /// 指定されたエフェクトタイプに対応するハンドラーインスタンスを生成する
        /// </summary>
        public static EffectPerformerBase CreateHandler(EffectOrderType effectType, params object[] constructorArgs)
        {
            // 初期化済みかチェック
            EnsureInitialized();
            
            if (!_handlerTypes.TryGetValue(effectType, out var handlerType))
            {
                LogUtility.Warning($"未登録のエフェクトタイプが指定されました: {effectType}", LogCategory.System);
                return null;
            }

            return CreateHandlerInstance(handlerType, constructorArgs);
        }

        /// <summary>
        /// 登録済みの全エフェクトタイプに対してハンドラーインスタンスを生成する
        /// </summary>
        public static Dictionary<EffectOrderType, EffectPerformerBase> CreateAllHandlers(StoryView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view), "StoryViewは必須パラメータです");
            }
            
            // 初期化済みかチェック
            EnsureInitialized();

            var handlers = new Dictionary<EffectOrderType, EffectPerformerBase>();
            var failedHandlers = new List<EffectOrderType>();

            foreach (var (orderType, handlerType) in _handlerTypes)
            {
                try
                {
                    // インスタンスを生成
                    var handler = CreateHandlerWithDependencyInjection(handlerType, view);
                    if (handler != null)
                    {
                        // handlerが生成出来たら辞書に追加
                        handlers[orderType] = handler;
                    }
                    else
                    {
                        // 生成出来なかった場合失敗したリストに記録
                        failedHandlers.Add(orderType);
                    }
                }
                catch (Exception ex)
                {
                    LogUtility.Error($"Effect Performerの生成に失敗: {orderType} - {ex.Message}", LogCategory.System);
                    failedHandlers.Add(orderType);
                }
            }

            // 失敗したハンドラーがある場合は警告ログを出力
            if (failedHandlers.Count > 0)
            {
                LogUtility.Warning($"一部のEffect Performerの生成に失敗しました: [{string.Join(", ", failedHandlers)}]", LogCategory.System);
            }

            LogUtility.Info($"Effect Performerハンドラー生成完了: 成功 {handlers.Count}個, 失敗 {failedHandlers.Count}個", LogCategory.System);
            return handlers;
        }

        #region Private Methods
        
        /// <summary>
        /// アセンブリからEffect Performerを自動発見し、内部辞書に登録する
        /// EffectPerformerAttributeが付与されたクラスが対象
        /// </summary>
        private static void DiscoverAndRegisterHandlers()
        {
            _handlerTypes.Clear();

            // ロードされているアセンブリを取得
            var assemblies = GetLoadedAssemblies();
            var totalDiscovered = 0;

            foreach (var assembly in assemblies)
            {
                if (ShouldSkipAssembly(assembly))
                {
                    // システムアセンブリをスキップして処理を高速化
                    continue;
                }
                
                // EffectPerformerを見つける
                var discoveredCount = ProcessAssembly(assembly);
                totalDiscovered += discoveredCount;
            }

            if (totalDiscovered == 0)
            {
                LogUtility.Warning("Effect Performerが一つも発見されませんでした", LogCategory.System);
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
        /// システムアセンブリや外部ライブラリをスキップしてパフォーマンスを向上
        /// </summary>
        private static bool ShouldSkipAssembly(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;
            
            // システムアセンブリをスキップ
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
                   || assembly.GlobalAssemblyCache; // GACアセンブリもスキップ
        }

        /// <summary>
        /// 指定されたアセンブリからEffect Performerを発見・登録する
        /// </summary>
        private static int ProcessAssembly(Assembly assembly)
        {
            try
            {
                var handlerTypes = FindEffectPerformerTypes(assembly);
                return RegisterHandlerTypes(handlerTypes);
            }
            catch (ReflectionTypeLoadException ex)
            {
                LogUtility.Warning($"アセンブリの型読み込みエラー [{assembly.FullName}]: {ex.Message}", LogCategory.System);
                
                // 部分的に読み込み可能な型があれば処理を続行
                if (ex.Types != null)
                {
                    var validTypes = ex.Types.Where(t => t != null).ToArray();
                    var handlerTypes = FilterEffectPerformerTypes(validTypes);
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
        /// アセンブリからEffect Performer候補の型を抽出する
        /// </summary>
        private static Type[] FindEffectPerformerTypes(Assembly assembly)
        {
            var allTypes = assembly.GetTypes();
            return FilterEffectPerformerTypes(allTypes);
        }

        /// <summary>
        /// 型配列からEffect Performer条件に合致する型をフィルタリングする
        /// </summary>
        private static Type[] FilterEffectPerformerTypes(Type[] types)
        {
            return types
                .Where(IsValidEffectPerformerType)
                .ToArray();
        }
        
        /// <summary>
        /// 指定された型がEffect Performerとして有効かどうかを判定する
        /// </summary>
        private static bool IsValidEffectPerformerType(Type type)
        {
            return typeof(EffectPerformerBase).IsAssignableFrom(type)
                   && !type.IsInterface
                   && !type.IsAbstract
                   && type.GetCustomAttribute<EffectPerformerAttribute>() != null;
        }
        
        /// <summary>
        /// 発見されたハンドラー型を内部辞書に登録する
        /// </summary>
        private static int RegisterHandlerTypes(Type[] handlerTypes)
        {
            var registeredCount = 0;

            foreach (var type in handlerTypes)
            {
                var attribute = type.GetCustomAttribute<EffectPerformerAttribute>();
                if (attribute != null)
                {
                    if (_handlerTypes.ContainsKey(attribute.EffectType))
                    {
                        LogUtility.Warning($"重複するエフェクトタイプが検出されました: {attribute.EffectType} - {type.Name}で上書きします", LogCategory.System);
                    }

                    _handlerTypes[attribute.EffectType] = type;
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
                // 初期化が済んでいなければ初期化を行う
                Initialize();
            }
        }
        
        /// <summary>
        /// 指定された型とコンストラクタ引数を使用してハンドラーインスタンスを生成する
        /// </summary>
        private static EffectPerformerBase CreateHandlerInstance(Type handlerType, object[] constructorArgs)
        {
            try
            {
                return (EffectPerformerBase)Activator.CreateInstance(handlerType, constructorArgs);
            }
            catch (Exception ex)
            {
                LogUtility.Error($"ハンドラーインスタンスの生成に失敗: {handlerType.Name} - {ex.Message}", LogCategory.System);
                return null;
            }
        }
        
        /// <summary>
        /// StoryViewを依存性として注入してハンドラーインスタンスを生成する
        /// コンストラクタのパラメータを解析し、適切な引数を自動的に構築
        /// </summary>
        private static EffectPerformerBase CreateHandlerWithDependencyInjection(Type handlerType, StoryView storyView)
        {
            try
            {
                var constructors = handlerType.GetConstructors();
                var constructor = SelectBestConstructor(constructors);

                if (constructor == null)
                {
                    LogUtility.Warning($"適切なコンストラクタが見つかりません: {handlerType.Name}", LogCategory.System);
                    return null;
                }

                var constructorArgs = BuildConstructorArguments(constructor, storyView);
                return (EffectPerformerBase)Activator.CreateInstance(handlerType, constructorArgs);
            }
            catch (Exception ex)
            {
                LogUtility.Error($"依存性注入によるインスタンス生成に失敗: {handlerType.Name} - {ex.Message}", LogCategory.System);
                return null;
            }
        }
        
        /// <summary>
        /// 利用可能なコンストラクタから最適なものを選択する
        /// 現在はパラメータ数が最も多いコンストラクタを選択
        /// </summary>
        private static ConstructorInfo SelectBestConstructor(ConstructorInfo[] constructors)
        {
            return constructors
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();
        }

        /// <summary>
        /// コンストラクタのパラメータに基づいて適切な引数配列を構築する
        /// StoryView型のパラメータには提供されたインスタンスを注入し、その他の型にはnullまたはデフォルト値を設定
        /// </summary>
        private static object[] BuildConstructorArguments(ConstructorInfo constructor, StoryView storyView)
        {
            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                
                if (paramType == typeof(StoryView))
                {
                    args[i] = storyView;
                }
                else if (paramType.IsValueType)
                {
                    // 値型の場合はデフォルト値を設定
                    args[i] = Activator.CreateInstance(paramType);
                }
                else
                {
                    // 参照型の場合はnullを設定
                    args[i] = null;
                }
            }

            return args;
        }
        
        #endregion
    }
}