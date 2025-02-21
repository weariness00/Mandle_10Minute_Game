using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SeoTestTestTest;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
public class Motion_Test : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject phone;

    public void motion()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(phone.transform.DOLocalMove(new Vector3(0,-15,0), 1f).From());
        seq.Join(phone.transform.DOScale(1.8f, 1f).From().SetEase(Ease.InOutBack));
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Motion_Test))]
    public class Motion_TestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as Motion_Test;

            if (GUILayout.Button("연출"))
            {
                script.motion();
            }

        }
    }

#endif
}