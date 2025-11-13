using UnityEngine;

namespace PAPERMASK.Utilities
{
    public class ValidateAttribute : PropertyAttribute
    {
        public readonly ValidateType validationType;
        public ValidateAttribute(ValidateType type)
        {
            validationType = type;
        }
    }
}