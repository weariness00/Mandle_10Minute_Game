using UnityEngine;

namespace GamePlay.Phone
{
    public interface IPhoneApplication
    {
        public string AppName { get; }
        public Sprite AppIcon { get; set; }
        
        public void AppInstall(); // 앱 처음 설치시
        public void AppPlay(); // 앱 시작시
        public void AppResume(); // 앱 중단후 시작시
        public void AppPause(); // 앱 중단시
        public void AppUnInstall(); // 앱 삭제시
    }
}