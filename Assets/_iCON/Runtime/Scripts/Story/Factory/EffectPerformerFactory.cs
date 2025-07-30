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
    /// Effect Performer Factory
    /// </summary>
    public static class EffectPerformerFactory
    {
        /// <summary>
        /// ファクトリを初期化
        /// </summary>
        public static void Initialize()
        {
            HandlerFactoryBase<EffectPerformerBase, EffectOrderType, EffectPerformerAttribute>.Initialize();
        }

        /// <summary>
        /// 指定されたエフェクトタイプのハンドラーを生成
        /// </summary>
        public static EffectPerformerBase CreateHandler(EffectOrderType effectType, params object[] constructorArgs)
        {
            return HandlerFactoryBase<EffectPerformerBase, EffectOrderType, EffectPerformerAttribute>.CreateHandler(
                effectType, constructorArgs);
        }

        /// <summary>
        /// 全てのハンドラーを生成
        /// </summary>
        public static Dictionary<EffectOrderType, EffectPerformerBase> CreateAllHandlers(StoryView storyView)
        {
            if (storyView == null)
                throw new ArgumentNullException(nameof(storyView), "StoryViewは必須パラメータです");

            return HandlerFactoryBase<EffectPerformerBase, EffectOrderType, EffectPerformerAttribute>
                .CreateAllHandlers(type => BuildConstructorArguments(type, storyView));
        }

        /// <summary>
        /// 登録されているハンドラー数を取得
        /// </summary>
        public static int GetRegisteredHandlerCount()
        {
            return HandlerFactoryBase<EffectPerformerBase, EffectOrderType, EffectPerformerAttribute>
                .GetRegisteredHandlerCount();
        }

        /// <summary>
        /// 指定されたエフェクトタイプが登録済みかどうかを確認
        /// </summary>
        public static bool IsHandlerRegistered(EffectOrderType effectType)
        {
            return HandlerFactoryBase<EffectPerformerBase, EffectOrderType, EffectPerformerAttribute>
                .IsHandlerRegistered(effectType);
        }

        /// <summary>
        /// コンストラクタ引数を構築する
        /// </summary>
        private static object[] BuildConstructorArguments(Type handlerType, StoryView storyView)
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