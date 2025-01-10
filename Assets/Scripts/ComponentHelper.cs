using System.Collections.Generic;
using UnityEngine;

public static class ComponentHelper
{
    public static List<T> GetComponentsInAllChildren<T>(Transform parent) where T : Component
    {
        List<T> components = new List<T>();

        // Check if the parent itself has the component
        T component = parent.GetComponent<T>();
        if (component != null)
        {
            components.Add(component);
        }

        // Recursively check all children
        foreach (Transform child in parent)
        {
            components.AddRange(GetComponentsInAllChildren<T>(child));
        }

        return components;
    }
}
