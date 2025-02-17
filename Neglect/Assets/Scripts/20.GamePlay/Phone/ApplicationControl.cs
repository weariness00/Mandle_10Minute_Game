using GamePlay.Phone;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class ApplicationControl : MonoBehaviour
{
    [SerializeField] private PhoneControl phone;

    public UnityEvent<IPhoneApplication> OnAddAppEvent = new();
    public UnityEvent<IPhoneApplication> OnAppEvent = new();
    public UnityEvent<IPhoneApplication> OnAppResumeEvent = new();
    public IPhoneApplication currentPlayApplication;

    private Dictionary<string, IPhoneApplication> applicationDictionary = new(); // 앱 이름, 앱

    public void Start()
    {
        Debug.Assert(phone != null, "Application Control에는 Phone Control이 필요합니다.");
    }

    public IPhoneApplication GetApp(string appName) => applicationDictionary.GetValueOrDefault(appName);
    public void AddApp(IPhoneApplication app)
    {
        OnAddAppEvent?.Invoke(app);
        app.AppInstall(phone);
    }

    // 어플리케이션 실행
    public void OpenApp(IPhoneApplication app)
    {
        if (currentPlayApplication != null) currentPlayApplication.AppPause(phone);

        // 앱을 켰을시 처음 킨거면 dict에 추가한 후 add 이벤트 실행
        if (applicationDictionary.TryAdd(app.AppName, app))
        {
            app.AppPlay(phone);
            OnAppEvent?.Invoke(app);
        }
        else
        {
            app.AppResume(phone);
            OnAppResumeEvent?.Invoke(app);
        }
        currentPlayApplication = app;
    }

    public void CloseApp()
    {
        if(currentPlayApplication!= null && currentPlayApplication.AppName != "Home")
            CloseApp(currentPlayApplication);
    }
    
    public void CloseApp(IPhoneApplication app)
    {
        if (currentPlayApplication == app) currentPlayApplication = null;

        applicationDictionary.Remove(app.AppName);
        app.AppExit(phone);

        OnHome();
    }

    // 홈 화면으로 이동
    public void OnHome()
    {
        if(currentPlayApplication is { AppName: "Home" }) return;
        if (applicationDictionary.TryGetValue("Home", out var app)) OpenApp(app);
    }

    // 어플리케이션이 실행된 것들 확인하는 메뉴로 이동
    public void OnAppListMenu()
    {
        if (currentPlayApplication != null) currentPlayApplication.AppPause(phone);
    }
}