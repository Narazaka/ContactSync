using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;

namespace Narazaka.VRChat.ContactSync.Editor
{
    public class AnimatorStateWithLayerInfo
    {
        public AnimatorState State;
        public AnimatorControllerLayer Layer;

        public AnimatorStateWithLayerInfo(AnimatorState state, AnimatorControllerLayer layer)
        {
            State = state;
            Layer = layer;
        }

        public static implicit operator AnimatorState(AnimatorStateWithLayerInfo animatorStateWithLayerInfo) => animatorStateWithLayerInfo.State;

        public AnimatorStateWithLayerInfo Position(float x, float y) => Position(new Vector2(x, y));
        public AnimatorStateWithLayerInfo Position(Vector2 position)
        {
            var states = Layer.stateMachine.states;
            var index = System.Array.FindIndex(states, s => s.state == State);
            states[index] = new ChildAnimatorState { state = State, position = position };
            Layer.stateMachine.states = states;
            return this;
        }

        public AnimatorStateWithLayerInfo WriteDefault(bool wd)
        {
            State.writeDefaultValues = wd;
            return this;
        }

        public AnimatorStateWithLayerInfo TimeParameter(string parameter)
        {
            State.timeParameter = parameter;
            State.timeParameterActive = true;
            return this;
        }

        public AnimatorStateWithLayerInfo Motion(Motion motion)
        {
            State.motion = motion;
            return this;
        }

        public AnimatorStateTransition AddTransition(AnimatorState destinationState)
        {
            var transition = AnimatorUtil.CreateTransition();
            transition.destinationState = destinationState;
            var transitions = State.transitions;
            ArrayUtility.Add(ref transitions, transition);
            State.transitions = transitions;
            return transition;
        }

        public AnimatorStateTransition AddExitTransition()
        {
            var transition = AnimatorUtil.CreateTransition();
            transition.isExit = true;
            var transitions = State.transitions;
            ArrayUtility.Add(ref transitions, transition);
            State.transitions = transitions;
            return transition;
        }

        public AnimatorStateWithLayerInfo CreateClip(string name, System.Action<AnimationClip> action)
        {
            var clip = new AnimationClip { name = name };
            Motion(clip);
            action(clip);
            return this;
        }

        public AnimatorStateWithLayerInfo Create1DBlendTree(string name, string blendParameter, System.Action<BlendTree> action)
        {
            var clip = new BlendTree { blendType = BlendTreeType.Simple1D, name = name, blendParameter = blendParameter, useAutomaticThresholds = false };
            Motion(clip);
            action(clip);
            return this;
        }

        public AnimatorStateWithLayerInfo CreateDirectBlendTree(string name, System.Action<BlendTree> action)
        {
            var clip = new BlendTree { blendType = BlendTreeType.Direct, name = name };
            Motion(clip);
            action(clip);
            return this;
        }

        public AnimatorStateWithLayerInfo AddBehaviour(StateMachineBehaviour behaviour)
        {
            var behaviours = State.behaviours;
            ArrayUtility.Add(ref behaviours, behaviour);
            State.behaviours = behaviours;
            return this;
        }

        public AnimatorStateWithLayerInfo AddParameterDriver(VRC.SDKBase.VRC_AvatarParameterDriver.Parameter parameter)
        {
            var behaviours = State.behaviours;
            var driver = System.Array.Find(behaviours, b => b is VRCAvatarParameterDriver) as VRCAvatarParameterDriver;
            if (driver == null)
            {
                driver = ScriptableObject.CreateInstance<VRCAvatarParameterDriver>();
                ArrayUtility.Add(ref behaviours, driver);
                State.behaviours = behaviours;
            }
            driver.parameters.Add(parameter);
            return this;
        }
    }
}
