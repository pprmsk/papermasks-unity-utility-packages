using UnityEditor;
using UnityEngine;

namespace PAPERMASK.Utilities
{
    [CustomPropertyDrawer(typeof(RichHeaderAttribute))]
    public class RichHeaderDrawer : DecoratorDrawer
    {
        private const int PADDING = 2;

        public override void OnGUI(Rect position)
        {
            RichHeaderAttribute header = (RichHeaderAttribute)attribute;
            GUIStyle style = new(EditorStyles.boldLabel);
            style.richText = true;
            EditorGUI.LabelField(position, header.text, style);
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight + PADDING;
        }
    }
}