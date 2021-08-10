using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ActionHelper.scripts.Editor
{
    public partial class Main : EditorWindow
    {
        [SerializeField] private List<Entity> entities;
        private AnimatorController _controller;
        private AnimationClip _conditionedAnimation;
        private AnimationClip _idleAnimation;
        private Vector2 _scroll;
        private GUIStyle _boxNormal;
        private GUIStyle _boxSelected;
        private string _layerName;
        private string _parameterName;
        private int _tab;
        private int _selected = Const.DEFAULT_SELECTED;
        private bool _optional;

        [MenuItem("ActionHelper/Main")]
        private static void Init()
        {
            var window = (Main) GetWindow(typeof(Main));
            window.Show();
        }

        private void OnEnable() => EditorCoroutineUtility.StartCoroutine(Update.Check(), this);

        private void OnGUI()
        {
            InitStyles();

            if (Update.IsUpdated)
                GUILayout.Box(Const.MSG_UPDATE_REQUIRED, GUILayout.MaxWidth(float.MaxValue));

            GUILayout.Label(Const.BASE_SETTINGS, EditorStyles.boldLabel);
            _controller = EditorGUILayout.ObjectField(
                Const.ANIMATOR, _controller, typeof(AnimatorController), false) as AnimatorController;
            _layerName = EditorGUILayout.TextField(Const.LAYER_NAME, _layerName);
            _parameterName = EditorGUILayout.TextField(Const.PARAMTER_NAME, _parameterName);

            _optional = GUILayout.Toggle(_optional, Const.OPTIONAL);

            if (_optional) Optional();
            
            GUILayout.Label(Const.MODES, EditorStyles.boldLabel);
            _tab = GUILayout.Toolbar(_tab, Const.MENUS);
            
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

        private void SingleToggle()
        {
            _idleAnimation = EditorGUILayout.ObjectField(
                Const.IDLE, _idleAnimation, typeof(AnimationClip), false) as AnimationClip;
            _conditionedAnimation = EditorGUILayout.ObjectField(
                Const.CONDITION, _conditionedAnimation, typeof(AnimationClip), false) as AnimationClip;

            if (GUILayout.Button(Const.SAVE)) AppendLayerWithParameter(_idleAnimation, _conditionedAnimation);
        }

        private void MultipleToggle()
        {
            ScriptableObject target = this;
            var so = new SerializedObject(target);
            var stringsProperty = so.FindProperty("entities");
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(Const.ADD) && stringsProperty.arraySize < Const.VRC_MAX_INT)
            {
                stringsProperty.arraySize += 1;
            }

            if (GUILayout.Button(Const.REMOVE) && stringsProperty.arraySize > Const.ZERO)
            {
                if (_selected == Const.DEFAULT_SELECTED)
                {
                    stringsProperty.arraySize -= 1;
                }
                else
                {
                    stringsProperty.DeleteArrayElementAtIndex(_selected);
                    _selected = Const.DEFAULT_SELECTED;
                }
            }

            if (GUILayout.Button(Const.REMOVE_ALL))
            {
                stringsProperty.ClearArray();
                _selected = Const.DEFAULT_SELECTED;
            }

            EditorGUILayout.EndHorizontal();


            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MaxHeight(400));

            for (var i = 0; i < stringsProperty.arraySize; i++)
            {
                DrawAnimationClips(stringsProperty, i);
            }

            EditorGUILayout.EndScrollView();
            
            so.ApplyModifiedProperties();
            
            if (GUILayout.Button(Const.SAVE)) AppendLayerWithParameter();
        }

        private void DrawAnimationClips(SerializedProperty sp, int index)
        {
            var item = sp.GetArrayElementAtIndex(index);
            var animationClip = item.FindPropertyRelative("animationClip");

            EditorGUI.indentLevel += 1;
            
            var rect = EditorGUILayout.BeginHorizontal(index == _selected ? _boxSelected : _boxNormal);
            
            EditorGUILayout.LabelField($"{Const.CONDITION} {index}", GUILayout.MinWidth(10));
            EditorGUILayout.PropertyField(animationClip, new GUIContent(""));
            
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel -= 1;

            if (Event.current.type == EventType.MouseDown)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    _selected = index;
                    Event.current.Use();
                }
            }
        }

        private void AppendLayerWithParameter(Motion idleClip, Motion conditionedClip)
        {
            if (CheckCondition()) return;
            
            #if VRC_SDK_VRCSDK3
            if (_optional)
            {
                var result = CreateVrcParameters(VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.ValueType.Bool);
                if (!result) return;
            }
            #endif
            
            AddParameterAndLayer(AnimatorControllerParameterType.Bool);
            

            var stateMachine = StateMachinePosition();
            var firstState = stateMachine.AddState(Const.IDLE, new Vector3(400, 0));
            var lastState = stateMachine.AddState(Const.CONDITION, new Vector3(400, 100));

            firstState.AddTransition(lastState)
                .AddCondition(AnimatorConditionMode.If, 0, _parameterName);
            lastState.AddTransition(firstState)
                .AddCondition(AnimatorConditionMode.IfNot, 0, _parameterName);

            firstState.motion = idleClip;
            lastState.motion = conditionedClip;
        }

        private void AppendLayerWithParameter()
        {
            if (CheckCondition()) return;
            
            #if VRC_SDK_VRCSDK3
            if (_optional)
            {
                var result = CreateVrcParameters(VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.ValueType.Int);
                if (!result) return;
            }
            #endif
            
            AddParameterAndLayer(AnimatorControllerParameterType.Int);
            
            
            for (var i = 0; i < entities.Count; i++)
            {
                var stateMachine = StateMachinePosition();
                var state = _controller.layers.Last().stateMachine
                    .AddState(entities[i].animationClip.name, new Vector3(400, i * 100));
                stateMachine.AddAnyStateTransition(state)
                    .AddCondition(AnimatorConditionMode.Equals, i, _parameterName);
                state.motion = entities[i].animationClip;
            }
        }

        private bool CheckCondition()
        {
            if (!_controller)
            {
                Alert(Const.MSG_REQUIREMENTS);
                return true;
            }

            if (string.IsNullOrEmpty(_parameterName) || string.IsNullOrEmpty(_layerName))
            {
                Alert(Const.MSG_ENTER_PARAMETER_NAME_OR_LAYER_NAME);
                return true;
            }

            if (_controller.parameters.All(x => x.name != _parameterName)) return false;
            Alert(Const.MSG_DUPLICATED);
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
            EditorUtility.DisplayDialog(Const.ALERT, content, Const.OK);
        }

        private void InitStyles()
        {
            //Normal
            if (_boxNormal == null) _boxNormal = new GUIStyle(GUI.skin.box);

            //Selected
            if (_boxSelected != null) return;
            _boxSelected = new GUIStyle(GUI.skin.box)
            {
                normal = {background = MakeStyleBackground(new Color(0.0f, 0.5f, 1f, 0.5f))}
            };
        }

        private AnimatorStateMachine StateMachinePosition()
        {
            var stateMachine = _controller.layers.Last().stateMachine;
            stateMachine.entryPosition = new Vector3(0, 0);
            stateMachine.anyStatePosition = new Vector3(0, 200);
            stateMachine.exitPosition = new Vector3(0, 400);

            return stateMachine;
        }

        private Texture2D MakeStyleBackground(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}