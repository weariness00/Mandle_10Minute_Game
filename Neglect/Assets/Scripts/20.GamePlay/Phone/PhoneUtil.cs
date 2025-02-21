using GamePlay.Phone;
using System.Collections.Generic;
using UnityEngine;

public static class PhoneUtil
{
    private static Dictionary<string, PhoneControl> phoneDictionary = new();
    public static PhoneControl currentPhone;

    public static void Release()
    {
        phoneDictionary.Clear();
    }
    
    public static void AddPhone(PhoneControl phone)
    {
        if(!phoneDictionary.TryAdd(phone.phoneName, phone))
            Debug.LogWarning($"{phone.phoneName}이름의 Phone이 이미 추가되어 있습니다.");
    }
    
    public static PhoneControl GetPhone(string phoneName) =>phoneDictionary.GetValueOrDefault(phoneName);

    public static void SetLayer(Object obj)
    {
        if(obj is GameObject go)
            SetLayer(go);
        else if(obj is Component component)
            SetLayer(component.gameObject);
    }

    public static void SetLayer(GameObject obj)
    {
        if(currentPhone == null) return;

        foreach (var transform in obj.GetComponentsInChildren<Transform>(true))
        {
            transform.gameObject.layer = LayerMask.NameToLayer("Phone");
        }
    }

    public static T InstantiateUI<T>(T component) where T : Object => InstantiateUI(component, currentPhone.phoneName);
    public static T InstantiateUI<T>(T component, string phoneName) where T : Object
    {
        var phone = GetPhone(phoneName);
        return InstantiateUI(component, phone);
    }
    
    public static T InstantiateUI<T>(T component, PhoneControl phone) where T : Object
    {
        var homeApp = phone.applicationControl.GetApp("Home");
        var homeView = (HomeView)homeApp;
        var obj = Object.Instantiate(component, homeView.uiCanvas.transform);
        SetLayer(obj);
        return obj;
    }
    
    public static T InstantiateUI<T>(T component, out PhoneControl phone) where T : Object => InstantiateUI(component, currentPhone.phoneName, out phone);
    public static T InstantiateUI<T>(T component, string phoneName, out PhoneControl phone) where T : Object
    {
        phone = GetPhone(phoneName);
        var homeApp = phone.applicationControl.GetHomeApp();
        Debug.Assert(homeApp != null, $"Phone에 Home App이 존재하지 않아 {component.name}을 생성할 수 없습니다.");
        
        var obj = Object.Instantiate(component, homeApp.uiCanvas.transform);
        SetLayer(obj);
        return obj;
    }

    public static Canvas GetPhoneCanvas(PhoneControl phone)
    {
        var homeApp = phone.applicationControl.GetApp("Home");
        var homeView = (HomeView)homeApp;
        return homeView.uiCanvas;
    }
}