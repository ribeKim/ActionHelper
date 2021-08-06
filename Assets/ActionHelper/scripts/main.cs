#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ActionHelper.scripts
{
    public class Main : EditorWindow
    {
        private const int VRC_MAX_INT = 255;
        [SerializeField] private List<AnimationClip> animations;
        private AnimatorController _controller;
        private AnimationClip _conditionedAnimation;
        private AnimationClip _idleAnimation;
        private string _layerName;
        private string _parameterName;
        private Vector2 _scroll;
        private int _tab;

        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            _controller =
                EditorGUILayout.ObjectField("Animator", _controller, typeof(AnimatorController), false) as
                    AnimatorController;
            _layerName = EditorGUILayout.TextField("Layer Name", _layerName);
            _parameterName = EditorGUILayout.TextField("Parameter Name", _parameterName);
            GUILayout.Label("Modes", EditorStyles.boldLabel);
            _tab = GUILayout.Toolbar(_tab, new[] {"Single Toggle", "Multiple Toggle"});

            switch (_tab)
            {
                case 0:
                    SingleToggle();
                    break;
                case 1:
                    MultipleToggle();
                    break;
            }
        }

        [MenuItem("ActionHelper/Main")]
        private static void Init()
        {
            var window = (Main) GetWindow(typeof(Main));
            window.Show();
        }

        private void SingleToggle()
        {
            _idleAnimation =
                EditorGUILayout.ObjectField("IDLE", _idleAnimation, typeof(AnimationClip), false) as AnimationClip;
            _conditionedAnimation =
                EditorGUILayout.ObjectField("Condition", _conditionedAnimation, typeof(AnimationClip),
                    false) as AnimationClip;

            if (GUILayout.Button("Save")) AppendLayerWithParameter(_idleAnimation, _conditionedAnimation);
        }

        private void MultipleToggle()
        {
            ScriptableObject target = this;
            var so = new SerializedObject(target);
            var stringsProperty = so.FindProperty("animations");

            if (stringsProperty.arraySize > VRC_MAX_INT) stringsProperty.arraySize = VRC_MAX_INT;

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MaxHeight(400));
            EditorGUILayout.PropertyField(stringsProperty, true);
            EditorGUILayout.EndScrollView();
            so.ApplyModifiedProperties();


            if (GUILayout.Button("Save")) AppendLayerWithParameter(animations);
        }

        private void AppendLayerWithParameter(Motion idleClip, Motion conditionedClip)
        {
            if (CheckCondition()) return;
            AddParameterAndLayer(AnimatorControllerParameterType.Bool);

            var firstState = _controller.layers.Last().stateMachine.AddState("IDLE", new Vector3(400, 0, 0));
            var lastState = _controller.layers.Last().stateMachine.AddState("Condition", new Vector3(400, 100, 0));

            firstState.AddTransition(lastState)
                .AddCondition(AnimatorConditionMode.If, 0, _parameterName);
            lastState.AddTransition(firstState)
                .AddCondition(AnimatorConditionMode.IfNot, 0, _parameterName);

            firstState.motion = idleClip;
            lastState.motion = conditionedClip;
        }

        private void AppendLayerWithParameter(IList<AnimationClip> clips)
        {
            if (CheckCondition()) return;
            AddParameterAndLayer(AnimatorControllerParameterType.Int);

            foreach (var animationClip in clips)
            {
                var index = clips.IndexOf(animationClip);
                var stateMachine = _controller.layers.Last().stateMachine;
                var state = _controller.layers.Last().stateMachine
                    .AddState(animationClip.name, new Vector3(400, index * 100, 0));
                stateMachine.AddAnyStateTransition(state)
                    .AddCondition(AnimatorConditionMode.Equals, index, _parameterName);
                state.motion = animationClip;
            }
        }

        private bool CheckCondition()
        {
            if (!_controller)
            {
                Alert("Requirement: Controller");
                return true;
            }

            if (string.IsNullOrEmpty(_parameterName) || string.IsNullOrEmpty(_layerName))
            {
                Alert("Enter ParameterName or LayerName");
                return true;
            }

            if (_controller.parameters.All(x => x.name != _parameterName)) return false;
            Alert("Duplicated ParameterName");
            return true;
        }

        private void AddParameterAndLayer(AnimatorControllerParameterType type)
        {
            _controller.AddParameter(_parameterName, type);
            _controller.AddLayer(_layerName);
            var layers = _controller.layers;
            layers.Last().defaultWeight = 1;
            _controller.layers = layers;
        }

        private void Alert(string content)
        {
            EditorUtility.DisplayDialog("Alert", content, "Ok");
        }
    }
}
#endif