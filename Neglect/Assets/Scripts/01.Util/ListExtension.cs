using System.Collections.Generic;

namespace Util
{
    public static class ListExtension
    {
        public static T Random<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        public static List<T> Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int random1 = UnityEngine.Random.Range(0, list.Count);
                int random2 = UnityEngine.Random.Range(0, list.Count);

                (list[random1], list[random2]) = (list[random2], list[random1]);
            }

            return list;
        }
    }
}