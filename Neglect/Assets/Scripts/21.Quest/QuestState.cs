namespace Quest
{
    public enum QuestState
    {
        NotStarted, // 시작 안함
        Wait, // 시작 후 대기
        InProgress, // 진행중
        Completed, // 해결
        Failed // 실패
    }
}