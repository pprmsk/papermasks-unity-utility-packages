using UnityEngine;

namespace PAPERMASK.Utilities
{
    public class NullValueLabelAttribute : PropertyAttribute
    {
        public readonly string label;

        public NullValueLabelAttribute(string label = "Default")
        {
            this.label = label;
        }
    }
}