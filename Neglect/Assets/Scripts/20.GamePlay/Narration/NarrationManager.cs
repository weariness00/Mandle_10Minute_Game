using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

namespace GamePlay.Narration
{
    public class NarrationManager : Singleton<NarrationManager>
    {
        public static NarrationScriptableObject Data => NarrationScriptableObject.Instance;
        
        [Tooltip("Player 나레이션 클래스")] public Narrator playerNarration;
        [Tooltip("Achievement 나레이션 클래스")] public Narrator achievementNarration;

        private List<NarrationData> achievementNarrationList = new();

        public void StartNarrationID(int id) => StartNarration(Data.GetNarrationID(id));
        public void StartNarration(NarrationData data)
        {
            // 업적 나레이션
            if (data.target == 0)
            {
                var index = achievementNarrationList.BinarySearch(data);
                if (index < 0)
                {
                    achievementNarration.StartNarration(data);
                    achievementNarrationList.Add(data);
                }
            }
            // 플레이어 나레이션
            else if (data.target == 1)
            {
                playerNarration.StartNarration(data);
            }
        }
    }
}

