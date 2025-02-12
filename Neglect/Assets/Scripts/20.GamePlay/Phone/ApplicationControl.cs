using GamePlay.Phone;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class ApplicationControl : MonoBehaviour
{
    [SerializeField] private Button appButtonPrefab;
    [SerializeField] private Transform appButtonParent;

    public UnityEvent<IPhoneApplication> OnAddAppEvent = new();
    public UnityEvent<IPhoneApplication> OnAppEvent = new();
    public IPhoneApplication currentPlayApplication;

    private Dictionary<string, Button> appButtonDictionary = new();
    private Dictionary<string, IPhoneApplication> applicationList = new(); // 앱 이름, 앱
    
    public void AddApp(IPhoneApplication app)
    {
        applicationList.Add(app.AppName, app);
        app.AppInstall();

        var appButton = Instantiate(appButtonPrefab, appButtonParent);
        if (app.AppIcon) appButton.image.sprite = app.AppIcon;
        appButton.onClick.AddListener(app.AppPlay); // 앱 중단하고 다시 시작할시에 바꿔야함 이건 테스트용
        
        OnAddAppEvent?.Invoke(app);
    }

    // 어플리케이션 실행
    public void OnApp(IPhoneApplication app)
    {
        if (currentPlayApplication != null) currentPlayApplication.AppPause();
        currentPlayApplication = app;

        app.AppPlay();
        
        OnAppEvent.Invoke(app);
    }

    // 홈 화면으로 이동
    public void OnHome()
    {
        if (currentPlayApplication != null) currentPlayApplication.AppPause();
    }

    // 어플리케이션이 실행된 것들 확인하는 메뉴로 이동
    public void OnAppListMenu()
    {
        if (currentPlayApplication != null) currentPlayApplication.AppPause();
    }
}