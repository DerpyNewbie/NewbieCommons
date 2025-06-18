using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DerpyNewbie.Common.Editor
{
    [CustomPropertyDrawer(typeof(NewbieInject))]
    public class NewbieInjectPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var newbieInject = (NewbieInject)attribute;
            var propertyField = new PropertyField(property);
            propertyField.SetEnabled(false);

            propertyField.label = $"{property.displayName} (NewbieInjected-{newbieInject.Scope})";

            if (fieldInfo.FieldType.IsArray)
            {
                // FIXME: workaround for issue where you cannot disable array resizing #34
                propertyField.schedule.Execute(() =>
                {
                    var parent = propertyField.parent.parent;
                    while (parent is not ListView and not null)
                        parent = parent.parent;

                    if (parent is not ListView listView) return;

                    listView.headerTitle = $"{property.displayName} (NewbieInjected-{newbieInject.Scope})";

                    listView.SetEnabled(false);
                    if (listView.Q<HelpBox>("newbie-inject-no-array-resize-warn") != null) return;
                    var helpBox = new HelpBox(
                        "Changes will be overwritten on inject phase!",
                        HelpBoxMessageType.Warning)
                    {
                        name = "newbie-inject-no-array-resize-warn"
                    };
                    listView.showAddRemoveFooter = false;
                    listView.hierarchy[0].Add(helpBox);
                });
            }

            return propertyField;
        }

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