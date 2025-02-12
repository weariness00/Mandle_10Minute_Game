using GamePlay.Phone;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationControl : MonoBehaviour
{
    [SerializeField] private Button appButtonPrefab;
    [SerializeField] private Transform appButtonParent;

    public IPhoneApplication currentPlayApplication;

    private Dictionary<string, Button> appButtonDictionary = new();
    private Dictionary<string, IPhoneApplication> applicationList = new(); // 앱 이름, 앱
    
    public void AddApp(IPhoneApplication app)
    {
        applicationList.Add(app.AppName, app);
        app.AppInstall();

        var appButton = Instantiate(appButtonPrefab, appButtonParent);
        if(app.AppIcon) appButton.image.sprite = app.AppIcon;
        appButton.onClick.AddListener(app.AppPlay); // 앱 중단하고 다시 시작할시에 바꿔야함 이건 테스트용
    }

    // 어플리케이션 실행
    public void OnApp(IPhoneApplication app)
    {
        if(currentPlayApplication != null) currentPlayApplication.AppPause();
        currentPlayApplication = app;
        
        app.AppPlay();
    }

    // 홈 화면으로 이동
    public void OnHome()
    {
        if(currentPlayApplication != null) currentPlayApplication.AppPause();
    }

    // 어플리케이션이 실행된 것들 확인하는 메뉴로 이동
    public void OnAppListMenu()
    {
        if(currentPlayApplication != null) currentPlayApplication.AppPause();
    }
}

