using UnityEngine;

public class PhoneUtil
{
    public static void SetLayer(GameObject obj)
    {
        foreach (var transform in obj.GetComponentsInChildren<Transform>())
        {
            transform.gameObject.layer = LayerMask.NameToLayer("Phone");
        }
    }
}

