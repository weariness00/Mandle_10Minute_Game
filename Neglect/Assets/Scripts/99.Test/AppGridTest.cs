using GamePlay.App.Dummy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppGridTest : MonoBehaviour
{
    public DummyApp app;
    
    // Update is called once per frame
    void Update()
    {
        Debug.Log($"{app.name} : {app.appCellSize}");
    }
}
