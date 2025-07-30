using System;
using System.Collections.Generic;
using System.Linq;
using CryStar.Story.Attributes;
using CryStar.Story.Enums;
using CryStar.Story.Execution;
using CryStar.Story.UI;

namespace CryStar.Story.Factory
{
    /// <summary>
    /// Order Handler Factory
    /// </summary>
    public static class OrderHandlerFactory
    {
        /// <summary>
        /// ファクトリを初期化
        /// </summary>
        public static void Initialize()
        {
            HandlerFactoryBase<OrderHandlerBase, OrderType, OrderHandlerAttribute>.Initialize();
        }

        /// <summary>
        /// 指定されたオーダータイプのハンドラーを生成
        /// </summary>
        public static OrderHandlerBase CreateHandler(OrderType orderType, params object[] constructorArgs)
        {
            return HandlerFactoryBase<OrderHandlerBase, OrderType, OrderHandlerAttribute>.CreateHandler(orderType,
                constructorArgs);
        }

        /// <summary>
        /// 全てのハンドラーを生成
        /// </summary>
        public static Dictionary<OrderType, OrderHandlerBase> CreateAllHandlers(StoryView storyView, Action endAction)
        {
            return HandlerFactoryBase<OrderHandlerBase, OrderType, OrderHandlerAttribute>
                .CreateAllHandlers(type => BuildConstructorArguments(type, storyView, endAction));
        }

        /// <summary>
        /// 登録されているハンドラー数を取得
        /// </summary>
        public static int GetRegisteredHandlerCount()
        {
            return HandlerFactoryBase<OrderHandlerBase, OrderType, OrderHandlerAttribute>
                .GetRegisteredHandlerCount();
        }

        /// <summary>
        /// 指定されたオーダータイプが登録済みかどうかを確認
        /// </summary>
        public static bool IsHandlerRegistered(OrderType orderType)
        {
            return HandlerFactoryBase<OrderHandlerBase, OrderType, OrderHandlerAttribute>
                .IsHandlerRegistered(orderType);
        }

        /// <summary>
        /// コンストラクタ引数を構築する
        /// </summary>
        private static object[] BuildConstructorArguments(Type handlerType, StoryView storyView, Action endAction)
        {
            var constructors = handlerType.GetConstructors();
            var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

            if (constructor == null)
                return Array.Empty<object>();

            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;

                if (paramType == typeof(StoryView))
                {
                    args[i] = storyView;
                }
                else if (paramType == typeof(Action))
                {
                    args[i] = endAction;
                }
                else if (paramType.IsValueType)
                {
                    args[i] = Activator.CreateInstance(paramType);
                }
                else
                {
                    args[i] = null;
                }
            }

            return args;
        }
    }
}
