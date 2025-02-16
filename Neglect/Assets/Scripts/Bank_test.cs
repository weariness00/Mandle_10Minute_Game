using GamePlay;
using GamePlay.Phone;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class Bank_test : MonoBehaviour
{

    // Start is called before the first frame update
    public PhoneControl phone;
    public void Start()
    {
        
        SceneUtil.AsyncAddBank(scene =>
        {
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                if (app != null) phone.applicationControl.AddApp(app);
            }
        });
    }
}
