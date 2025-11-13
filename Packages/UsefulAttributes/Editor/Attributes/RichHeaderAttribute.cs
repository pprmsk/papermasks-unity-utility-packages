using UnityEngine;

public class RichHeaderAttribute : PropertyAttribute
{
    public string text;
    public RichHeaderAttribute(string text) => this.text = text;
}