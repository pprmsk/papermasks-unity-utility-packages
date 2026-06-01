using UnityEngine;
using System.Collections.Generic;

namespace PAPERMASK.Utilities
{
    public class PrefixAttribute : PropertyAttribute
    {
        public readonly string prefix;
        public readonly Color color;

        private static readonly Dictionary<string, Color> PrefixColorOverrides = new()
    {
        { "SObj", Color.royalBlue},
        { "Comp", Color.paleGreen},
        { "Cfg", Color.gold},
        { "Debug", Color.gray},
        { "Prefab", Color.lightCyan},
        { "Temp", Color.darkRed},
    };

        public PrefixAttribute(string prefix, string hexColor = null)
        {
            this.prefix = prefix;

            if (PrefixColorOverrides.TryGetValue(prefix, out Color overrideColor))
            {
                if (hexColor != null)
                {
                    Debug.LogWarning($"Prefix '{prefix}' has a color override. The provided color '{hexColor}' will be ignored.");
                }

                color = overrideColor;
                return;
            }

            if (ColorUtility.TryParseHtmlString(hexColor, out Color col))
            {
                color = col;
            }
            else
            {
                color = Color.white;
            }
        }
    }
}