using GamePlay.Phone;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationControl : MonoBehaviour
{
    public Dictionary<string, IPhoneApplication> applicationList; // 앱 이름, 앱
    public IPhoneApplication currentApplication;

    public void AddApp(IPhoneApplication app)
    {
        applicationList.Add(app.AppName, app);
        if (!ReferenceEquals(currentApplication, null))
        {
            currentApplication.OnPause();
        }
        app.OnLoad();
    }
}

