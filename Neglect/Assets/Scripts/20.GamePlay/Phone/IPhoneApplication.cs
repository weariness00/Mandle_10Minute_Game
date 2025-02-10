namespace GamePlay.Phone
{
    public interface IPhoneApplication
    {
        public string AppName { get; }
        
        public void OnLoad(); // 앱 처음 켯을때
        public void OnPlay(); // 앱 시작시
        public void OnResume(); // 앱 중단후 시작시
        public void OnPause(); // 앱 중단시
        public void OnExit(); // 앱 끌 시
    }
}