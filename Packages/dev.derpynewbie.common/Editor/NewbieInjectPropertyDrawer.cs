using UnityEditor;
using UnityEngine;

namespace DerpyNewbie.Common.Editor
{
    [CustomPropertyDrawer(typeof(NewbieInject))]
    public class NewbieInjectPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var newbieInject = (NewbieInject)attribute;
            
            using (new EditorGUI.DisabledScope(true))
            {
                label.text = $"{label.text} (NewbieInjected-{newbieInject.Scope})";
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}