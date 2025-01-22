using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace Narazaka.VRChat.ContactSync.Editor
{
    public static class AnimatorUtil
    {
        public static AnimatorControllerLayer AddNewLayer(this AnimatorController controller, string name)
        {
            AnimatorControllerLayer animatorControllerLayer = new AnimatorControllerLayer();
            animatorControllerLayer.name = controller.MakeUniqueLayerName(name);
            animatorControllerLayer.stateMachine = new AnimatorStateMachine();
            animatorControllerLayer.stateMachine.name = animatorControllerLayer.name;
            animatorControllerLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            animatorControllerLayer.defaultWeight = 1f;
            animatorControllerLayer.stateMachine.entryPosition = new Vector2(0, 0);
            animatorControllerLayer.stateMachine.exitPosition = new Vector2(0, -100);
            animatorControllerLayer.stateMachine.anyStatePosition = new Vector2(0, -200);

            var layers = controller.layers;
            ArrayUtility.Add(ref layers, animatorControllerLayer);
            controller.layers = layers;
            return animatorControllerLayer;
        }

        public static void DefaultState(this AnimatorControllerLayer layer, AnimatorState state) => layer.stateMachine.defaultState = state;
        public static void EntryPosition(this AnimatorControllerLayer layer, float x, float y) => layer.EntryPosition(new Vector2(x, y));
        public static void EntryPosition(this AnimatorControllerLayer layer, Vector2 position) => layer.stateMachine.entryPosition = position;
        public static void ExitPosition(this AnimatorControllerLayer layer, float x, float y) => layer.ExitPosition(new Vector2(x, y));
        public static void ExitPosition(this AnimatorControllerLayer layer, Vector2 position) => layer.stateMachine.exitPosition = position;
        public static void AnyStatePosition(this AnimatorControllerLayer layer, float x, float y) => layer.AnyStatePosition(new Vector2(x, y));
        public static void AnyStatePosition(this AnimatorControllerLayer layer, Vector2 position) => layer.stateMachine.anyStatePosition = position;

        public static AnimatorStateWithLayerInfo AddNewState(this AnimatorControllerLayer layer, string name)
        {
            var stateMachine = layer.stateMachine;
            AnimatorState animatorState = new AnimatorState();
            animatorState.hideFlags = HideFlags.HideInHierarchy;
            animatorState.name = stateMachine.MakeUniqueStateName(name);
            animatorState.writeDefaultValues = false;
            animatorState.motion = AnimationClipUtil.EmptyClip;

            var states = stateMachine.states;
            var item = new ChildAnimatorState { state = animatorState, position = Vector3.zero };
            ArrayUtility.Add(ref states, item);
            stateMachine.states = states;
            return new AnimatorStateWithLayerInfo(animatorState, layer);
        }

        public static T If<T>(this T transition, string parameter) where T : AnimatorTransitionBase => transition
            .AddCondition(new AnimatorCondition { mode = AnimatorConditionMode.If, threshold = 0, parameter = parameter });
        public static T IfNot<T>(this T transition, string parameter) where T : AnimatorTransitionBase => transition
            .AddCondition(new AnimatorCondition { mode = AnimatorConditionMode.IfNot, threshold = 0, parameter = parameter });

        public static T Equal<T>(this T transition, string parameter, float threshold) where T : AnimatorTransitionBase => transition
            .AddCondition(new AnimatorCondition { mode = AnimatorConditionMode.Equals, threshold = threshold, parameter = parameter });
        public static T NotEqual<T>(this T transition, string parameter, float threshold) where T : AnimatorTransitionBase => transition
            .AddCondition(new AnimatorCondition { mode = AnimatorConditionMode.NotEqual, threshold = threshold, parameter = parameter });

        public static T Greater<T>(this T transition, string parameter, float threshold) where T : AnimatorTransitionBase => transition
            .AddCondition(new AnimatorCondition { mode = AnimatorConditionMode.Greater, threshold = threshold, parameter = parameter });
        public static T Less<T>(this T transition, string parameter, float threshold) where T : AnimatorTransitionBase => transition
            .AddCondition(new AnimatorCondition { mode = AnimatorConditionMode.Less, threshold = threshold, parameter = parameter });

        public static T AddCondition<T>(this T transition, AnimatorCondition condition) where T : AnimatorTransitionBase
        {
            var c = transition.conditions;
            ArrayUtility.Add(ref c, condition);
            transition.conditions = c;
            return transition;
        }

        public static AnimatorStateTransition AutoExit(this AnimatorStateTransition transition)
        {
            transition.hasExitTime = true;
            return transition;
        }

        public static AnimatorTransition AddEntryTransition(this AnimatorControllerLayer layer, AnimatorState destinationState)
        {
            var transition = new AnimatorTransition();
            transition.hideFlags = HideFlags.HideInHierarchy;
            transition.destinationState = destinationState;
            var entryTransitions = layer.stateMachine.entryTransitions;
            ArrayUtility.Add(ref entryTransitions, transition);
            layer.stateMachine.entryTransitions = entryTransitions;
            return transition;
        }

        public static AnimatorStateTransition CreateTransition()
        {
            return new AnimatorStateTransition
            {
                hideFlags = HideFlags.HideInHierarchy,
                hasExitTime = false,
                hasFixedDuration = true,
                exitTime = 0f,
                duration = 0f,
                offset = 0f,
                canTransitionToSelf = false,
                interruptionSource = TransitionInterruptionSource.None,
            };
        }
    }
}
