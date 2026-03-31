using UnityEngine;

namespace PAPERMASK.Utilities
{
    public class HideIfAttribute : PropertyAttribute
    {
        public readonly string condition;
        public readonly object compareValue;
        public readonly bool hasCompareValue;
        public readonly bool hideIfTrue;

        public HideIfAttribute(string condition, bool hideIfTrue = true)
        {
            this.condition = condition;
            this.hideIfTrue = hideIfTrue;
        }

        public HideIfAttribute(string condition, object compareValue, bool hideIfTrue = true)
        {
            this.condition = condition;
            this.compareValue = compareValue;
            this.hideIfTrue = hideIfTrue;
            hasCompareValue = true;
        }
    }
}