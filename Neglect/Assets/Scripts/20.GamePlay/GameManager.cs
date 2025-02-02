using System;
using Quest;
using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int a = 0;
    public void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(1f)).Subscribe(_ => QuestManager.Instance.OnValueChange(QuestEvent.GameRank, a++));
    }
}

