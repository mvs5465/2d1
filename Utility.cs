using UnityEngine;

public static class Utility
{
    public static GameObject AttachChildObject(GameObject parent)
    {
        GameObject child = new();
        child.transform.parent = parent.transform;
        child.transform.localPosition = Vector2.zero;
        return child;
    }
    public static GameObject AttachChildObject(GameObject parent, string name)
    {
        GameObject child = new(name);
        child.transform.parent = parent.transform;
        child.transform.localPosition = Vector2.zero;
        return child;
    }
}