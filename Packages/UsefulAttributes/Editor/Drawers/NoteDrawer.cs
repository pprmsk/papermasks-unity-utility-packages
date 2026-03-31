using UnityEditor;
using UnityEngine;

namespace PAPERMASK.Utilities
{
    [CustomPropertyDrawer(typeof(NoteAttribute))]
    public class NoteDecoratorDrawer : DecoratorDrawer
    {
        const float paddingLeft = 6f;
        const float paddingRight = 6f;
        const float paddingTop = 2f;
        const float paddingBottom = 3f;

        const float outerSpacingTop = 6f;
        const float outerSpacingBottom = 8f;

        const float inspectorMargin = 20f;
        const float textHeightMargin = 2f;

        public override void OnGUI(Rect position)
        {
            NoteAttribute note = (NoteAttribute)attribute;

            position.y += outerSpacingTop;
            position.height -= (outerSpacingTop + outerSpacingBottom);

            EditorGUI.DrawRect(position, new Color(0f, 0f, 0f, 0.25f));

            GUIStyle style = new(EditorStyles.wordWrappedLabel)
            {
                richText = true
            };

            string text = $"<color=#FFD966><b>NOTE:</b></color> {note.text}";

            Rect textRect = new(
                position.x + paddingLeft,
                position.y + paddingTop,
                position.width - (paddingLeft + paddingRight),
                position.height - (paddingTop + paddingBottom)
            );

            EditorGUI.LabelField(textRect, text, style);
        }

        public override float GetHeight()
        {
            NoteAttribute note = (NoteAttribute)attribute;

            GUIStyle style = new(EditorStyles.wordWrappedLabel)
            {
                richText = true
            };

            string text = $"<color=#FFD966><b>NOTE:</b></color> {note.text}";

            float usableWidth = EditorGUIUtility.currentViewWidth - (paddingLeft + paddingRight) - inspectorMargin;

            float textHeight = style.CalcHeight(new GUIContent(text), usableWidth);

            return textHeight + paddingTop + paddingBottom + outerSpacingTop + outerSpacingBottom + textHeightMargin;
        }
    }
}