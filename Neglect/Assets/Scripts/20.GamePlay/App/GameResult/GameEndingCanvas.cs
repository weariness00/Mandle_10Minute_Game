using UnityEngine;

namespace GamePlay.App.GameResult
{
    public class GameEndingCanvas : MonoBehaviour
    {
        public Canvas mainCanvas;
        
        [Header("Good Ending 관련")] 
        public Canvas goodEndingCanvas;
        
        [Header("Bad Ending 관련")]
        public Canvas badEndingCanvas;
    }
}

