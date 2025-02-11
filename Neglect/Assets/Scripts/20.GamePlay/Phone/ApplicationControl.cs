using GamePlay.Phone;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationControl : MonoBehaviour
{
    [SerializeField] private Button appButtonPrefab;
    [SerializeField] private Transform appButtonParent;

    public IPhoneApplication currentApplication;


    private Dictionary<string, Button> appButtonDictionary = new();
    private Dictionary<string, IPhoneApplication> applicationList = new(); // 앱 이름, 앱

    public void AddApp(IPhoneApplication app)
    {
        applicationList.Add(app.AppName, app);
        if (!ReferenceEquals(currentApplication, null))
        {
            currentApplication.AppPause();
        }
        app.AppInstall();

        var appButton = Instantiate(appButtonPrefab, appButtonParent);
        if(app.AppIcon) appButton.image.sprite = app.AppIcon;
        appButton.onClick.AddListener(app.AppPlay); // 앱 중단하고 다시 시작할시에 바꿔야함 이건 테스트용
    }
}

