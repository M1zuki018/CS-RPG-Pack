using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryStar.Story.Attribute;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using iCON.Utility;

namespace CryStar.Story.Execution
{
    public static class EffectOrderPerformerFactory
{
    private static Dictionary<EffectOrderType, Type> _handlerTypes;
        private static bool _isInitialized = false;

        /// <summary>
        /// 自動でハンドラータイプを発見・登録
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            _handlerTypes = new Dictionary<EffectOrderType, Type>();
            
            // アセンブリ内の全ハンドラーを自動発見
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var handlerTypes = assembly.GetTypes()
                        .Where(type => typeof(EffectOrderPerformerBase).IsAssignableFrom(type) 
                                      && !type.IsInterface 
                                      && !type.IsAbstract
                                      && type.GetCustomAttribute<EffectOrderHandlerAttribute>() != null)
                        .ToArray();

                    foreach (var type in handlerTypes)
                    {
                        var attribute = type.GetCustomAttribute<EffectOrderHandlerAttribute>();
                        if (attribute != null)
                        {
                            _handlerTypes[attribute.EffectType] = type;
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // アセンブリ読み込みエラーをログに出力
                    LogUtility.Warning($"Assembly load error: {ex.Message}", LogCategory.System);
                }
            }

            _isInitialized = true;
            LogUtility.Info($"Registered {_handlerTypes.Count} order handlers", LogCategory.System);
        }

        /// <summary>
        /// ハンドラーを作成
        /// </summary>
        public static EffectOrderPerformerBase CreateHandler(EffectOrderType orderType, params object[] constructorArgs)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (!_handlerTypes.TryGetValue(orderType, out var handlerType))
            {
                return null;
            }

            try
            {
                return (EffectOrderPerformerBase)Activator.CreateInstance(handlerType, constructorArgs);
            }
            catch (Exception ex)
            {
                LogUtility.Error($"Failed to create handler for {orderType}: {ex.Message}", LogCategory.System);
                return null;
            }
        }

        /// <summary>
        /// 全てのハンドラーを作成
        /// </summary>
        public static Dictionary<EffectOrderType, EffectOrderPerformerBase> CreateAllHandlers(StoryView view)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            var handlers = new Dictionary<EffectOrderType, EffectOrderPerformerBase>();

            foreach (var kvp in _handlerTypes)
            {
                var orderType = kvp.Key;
                var handlerType = kvp.Value;

                try
                {
                    EffectOrderPerformerBase handler = null;

                    // コンストラクタの型に応じて適切な引数を渡す
                    var constructors = handlerType.GetConstructors();
                    var constructor = constructors.FirstOrDefault();

                    if (constructor != null)
                    {
                        var parameters = constructor.GetParameters();
                        var args = new List<object>();

                        foreach (var param in parameters)
                        {
                            if (param.ParameterType == typeof(StoryView))
                                args.Add(view);
                            else
                                args.Add(null); // デフォルト値
                        }

                        handler = (EffectOrderPerformerBase)Activator.CreateInstance(handlerType, args.ToArray());
                    }
                    else
                    {
                        handler = (EffectOrderPerformerBase)Activator.CreateInstance(handlerType);
                    }

                    if (handler != null)
                    {
                        handlers[orderType] = handler;
                    }
                }
                catch (Exception ex)
                {
                    LogUtility.Error($"Failed to create handler for {orderType}: {ex.Message}", LogCategory.System);
                }
            }

            return handlers;
        }
}

}
