using GamePlay.App.Home;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.App
{
    public class AppButton : MonoBehaviour
    {
        public Button button;
        public GameObject highlightObject;
        public AppGridControl.CellData cellData;

        public void Awake()
        {
            highlightObject.SetActive(false);
        }
    }
}

