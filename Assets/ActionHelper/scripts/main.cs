using System;
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
        private AnimatorController controller;
        private AnimationClip animation;
        [SerializeField] private List<AnimationClip> animations;
        private int tab;
        private int index = 0;

        [MenuItem("ActionHelper/Main")]
        static void Init()
        {
            var window = (Main) GetWindow(typeof(Main));
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Animator", EditorStyles.boldLabel);
            controller =
                EditorGUILayout.ObjectField("Animator", controller, typeof(AnimatorController), false) as
                    AnimatorController;
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            tab = GUILayout.Toolbar(tab, new[] {"Single Toggle", "Multiple Toggle"});

            switch (tab)
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
            EditorGUILayout.TextField("Layer Name", "");
            EditorGUILayout.TextField("Parameter Name", "");
            animation =
                EditorGUILayout.ObjectField("Animation", animation, typeof(AnimationClip), false) as AnimationClip;
        }

        private void MultipleToggle()
        {
            EditorGUILayout.TextField("Layer Name", "");
            EditorGUILayout.TextField("Parameter Name", "");
            
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty stringsProperty = so.FindProperty("animations");

            // if (stringsProperty.arraySize > VRC_MAX_INT) stringsProperty.arraySize = VRC_MAX_INT;
            EditorGUILayout.PropertyField(stringsProperty, true);
            so.ApplyModifiedProperties();

            // animations.Add(EditorGUILayout.ObjectField($"IDLE", animations[index++], typeof(AnimationClip), false) as AnimationClip);

            // if (!GUILayout.Button("Add Animation")) return;
            // animations.Add(EditorGUILayout.ObjectField($"IDLE", null, typeof(AnimationClip), false) as AnimationClip);
        }
    }
}