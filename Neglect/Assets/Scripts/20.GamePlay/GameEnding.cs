using System;
using UnityEngine;

namespace GamePlay
{
    public class GameEnding : MonoBehaviour
    {
        public Canvas mainCanvas;

        [Header("Good Ending 관련")] 
        public GameObject goodObject;
        public Canvas goodEndingCanvas;
        
        [Header("Bad Ending 관련")]
        public GameObject badObject;
        public Canvas badEndingCanvas;

        public void Awake()
        {
            mainCanvas.gameObject.SetActive(false);
            
            goodObject.SetActive(false);
            goodEndingCanvas.gameObject.SetActive(false);
            
            badObject.SetActive(false);
            badEndingCanvas.gameObject.SetActive(false);
        }

        public void GoodEnding()
        {
            mainCanvas.gameObject.SetActive(true);
            
            goodObject.SetActive(true);
            goodEndingCanvas.gameObject.SetActive(true);
        }
        
        public void BadEnding()
        {
            mainCanvas.gameObject.SetActive(true);
            
            badObject.SetActive(true);
            badEndingCanvas.gameObject.SetActive(true);
        }
    }
}

