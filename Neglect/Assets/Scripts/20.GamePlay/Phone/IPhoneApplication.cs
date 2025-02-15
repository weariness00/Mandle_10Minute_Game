using UnityEngine;

namespace GamePlay.Phone
{
    public interface IPhoneApplication
    {
        public string AppName { get; }
        public Sprite AppIcon { get; set; }
        public Vector2Int VerticalResolution { get; set; }
        public PhoneControl Phone { get; }
        
        public void AppInstall(PhoneControl phone); // 앱 처음 설치시
        public void AppPlay(PhoneControl phone); // 앱 시작시
        public void AppResume(PhoneControl phone); // 앱 중단후 시작시
        public void AppPause(PhoneControl phone); // 앱 중단시
        public void AppExit(PhoneControl phone); // 앱 종료시
        public void AppUnInstall(PhoneControl phone); // 앱 삭제시
    }
}