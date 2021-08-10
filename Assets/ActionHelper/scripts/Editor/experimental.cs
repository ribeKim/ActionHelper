
#if VRC_SDK_VRCSDK3
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace ActionHelper.scripts.Editor
{
    public partial class Main
    {
        private VRCExpressionParameters _expression;
        private void Optional(int mode)
        {
            _expression = EditorGUILayout.ObjectField(
                "text", _expression, typeof(VRCExpressionParameters), false) as VRCExpressionParameters;
        }
        

        private void CreateVrcParameters(VRCExpressionParameters.ValueType type)
        {
            var item = new VRCExpressionParameters.Parameter();
            item.name = _parameterName;
            item.valueType = type;
            
            _expression.parameters = _expression.parameters.Append(item).ToArray();
        }
    }
}

#endif