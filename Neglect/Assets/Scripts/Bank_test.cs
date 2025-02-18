using GamePlay;
using GamePlay.Phone;
using UnityEditor;
using UnityEngine;

public class Bank_test : MonoBehaviour
{

    // Start is called before the first frame update
    public PhoneControl phone;
    public void Start()
    {
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Bank_test))]
public class BankTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = target as Bank_test;
        if (GUILayout.Button("은행 씬 로드"))
        {
            SceneUtil.AsyncAddBank(scene =>
            {
                foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                {
                    var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                    if (app != null) PhoneUtil.currentPhone.applicationControl.AddApp(app);
                }
            });
        }
    }
}
#endif