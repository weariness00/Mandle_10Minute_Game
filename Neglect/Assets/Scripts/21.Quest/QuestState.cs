namespace Quest
{
    public enum QuestState
    {
        NotStarted, // 시작 안함
        InProgress, // 진행중
        Wait, // 대기
        Completed, // 해결
        Failed // 실패
    }
}