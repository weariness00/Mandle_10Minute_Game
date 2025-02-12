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
    public IPhoneApplication currentPlayApplication;

    private Dictionary<string, IPhoneApplication> applicationList = new(); // 앱 이름, 앱

    public void Start()
    {
        Debug.Assert(phone != null, "Application Control에는 Phone Control이 필요합니다.");
    }

    public IPhoneApplication GetApp(string appName) => applicationList.GetValueOrDefault(appName);
    public void AddApp(IPhoneApplication app)
    {
        applicationList.Add(app.AppName, app);
        app.AppInstall(phone);
        
        OnAddAppEvent?.Invoke(app);
    }

    // 어플리케이션 실행
    public void OnApp(IPhoneApplication app)
    {
        if (currentPlayApplication != null) currentPlayApplication.AppPause(phone);
        currentPlayApplication = app;

        app.AppPlay(phone);
        
        OnAppEvent.Invoke(app);
    }

    // 홈 화면으로 이동
    public void OnHome()
    {
        if (currentPlayApplication != null) currentPlayApplication.AppPause(phone);
    }

    // 어플리케이션이 실행된 것들 확인하는 메뉴로 이동
    public void OnAppListMenu()
    {
        if (currentPlayApplication != null) currentPlayApplication.AppPause(phone);
    }
}