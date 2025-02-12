using GamePlay.Phone;
using System.Collections.Generic;
using UnityEngine;

public static class PhoneUtil
{
    public static Dictionary<string, PhoneControl> phoneDictionary = new();
    
    public static void SetLayer(GameObject obj)
    {
        foreach (var transform in obj.GetComponentsInChildren<Transform>())
        {
            transform.gameObject.layer = LayerMask.NameToLayer("Phone");
        }
    }

    public static T InstantiateUI<T>(T prefab, string name) where T : Object
    {
        if (phoneDictionary.TryGetValue(name, out var phone))
        {
            var obj = Object.Instantiate(prefab, phone.phoneUICanvas.transform);
            return obj;
        }
        else
        {
            Debug.LogError($"{name}에 해당하는 Phone Control이 없어 UI를 생성 할 수 없습니다.");
            return null;
        }
    }
}

