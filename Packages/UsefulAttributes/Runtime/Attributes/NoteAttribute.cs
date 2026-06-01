using UnityEngine;

namespace PAPERMASK.Utilities
{
    public class NoteAttribute : PropertyAttribute
    {
        public string text;

        public NoteAttribute(string text)
        {
            this.text = text;
        }
    }
}