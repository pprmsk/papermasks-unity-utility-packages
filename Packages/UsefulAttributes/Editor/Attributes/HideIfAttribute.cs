using UnityEngine;

namespace PAPERMASK.Utilities
{
    public class HideIfAttribute : PropertyAttribute
    {
        public readonly string condition;
        public readonly bool hideIfTrue;

        public HideIfAttribute(string condition, bool hideIfTrue = true)
        {
            this.condition = condition;
            this.hideIfTrue = hideIfTrue;
        }
    }
}