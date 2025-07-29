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
    /// <summary>
    /// ハンドラーファクトリー
    /// </summary>
    public static class OrderHandlerFactory
    {
        private static Dictionary<OrderType, Type> _handlerTypes;
        private static bool _isInitialized = false;

        /// <summary>
        /// 自動でハンドラータイプを発見・登録
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            _handlerTypes = new Dictionary<OrderType, Type>();
            
            // アセンブリ内の全ハンドラーを自動発見
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var handlerTypes = assembly.GetTypes()
                        .Where(type => typeof(OrderHandlerBase).IsAssignableFrom(type) 
                                      && !type.IsInterface 
                                      && !type.IsAbstract
                                      && type.GetCustomAttribute<OrderHandlerAttribute>() != null)
                        .ToArray();

                    foreach (var type in handlerTypes)
                    {
                        var attribute = type.GetCustomAttribute<OrderHandlerAttribute>();
                        if (attribute != null)
                        {
                            _handlerTypes[attribute.OrderType] = type;
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
        public static OrderHandlerBase CreateHandler(OrderType orderType, params object[] constructorArgs)
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
                return (OrderHandlerBase)Activator.CreateInstance(handlerType, constructorArgs);
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
        public static Dictionary<OrderType, OrderHandlerBase> CreateAllHandlers(StoryView view, Action endAction)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            var handlers = new Dictionary<OrderType, OrderHandlerBase>();

            foreach (var kvp in _handlerTypes)
            {
                var orderType = kvp.Key;
                var handlerType = kvp.Value;

                try
                {
                    OrderHandlerBase handler = null;

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
                            else if (param.ParameterType == typeof(Action))
                                args.Add(endAction);
                            else
                                args.Add(null); // デフォルト値
                        }

                        handler = (OrderHandlerBase)Activator.CreateInstance(handlerType, args.ToArray());
                    }
                    else
                    {
                        handler = (OrderHandlerBase)Activator.CreateInstance(handlerType);
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
