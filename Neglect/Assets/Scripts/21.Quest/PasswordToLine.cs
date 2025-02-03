using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PasswordToLine : MonoBehaviour
{
    // Start is called before the first frame update
    [Tooltip("�н����� ����")]
    public List<GameObject> PasswordPointers = new List<GameObject>(); // �н����� ����

    [Tooltip("�н����� ����")]
    public LineRenderer[] PasswordLine = new LineRenderer[10];

    private List<int> InputPassword = new List<int>(); //���� �Է¹��� �н�����

    [Tooltip("�巡�� Ž�� ����")]
    public double DetectedRange = 0.3f; // ���� ���콺���� Ž�� ����

    private bool IsCrack = false; // �н����� Ǫ�� ������ 

    [Tooltip("���� �н�����")]
    public List<int> AnswerPassword;


    public void Init()
    {
        IsCrack = false;
        InputPassword.Clear();
        LineClear();
    }

    public void RePositionPointers()
    {

    }

    public void LineDraw()
    {
        for (int i = 1; i < InputPassword.Count; i++)
        {
            PasswordLine[i].positionCount = 2;
            PasswordLine[i].SetPosition(0, PasswordPointers[InputPassword[i-1]].transform.localPosition);
            PasswordLine[i].SetPosition(1, PasswordPointers[InputPassword[i]].transform.localPosition);
        }
        Vector3 curmouse = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        curmouse.z = 0;
        PasswordLine[InputPassword.Count].positionCount = 2;
        PasswordLine[InputPassword.Count].SetPosition(0, PasswordPointers[InputPassword[InputPassword.Count - 1]].transform.localPosition);
        PasswordLine[InputPassword.Count].SetPosition(1, curmouse);
        /*
        PasswordLine.positionCount = InputPassword.Count+1;
        for (int i = 0; i < InputPassword.Count; i++)
        {
            PasswordLine.SetPosition(i, PasswordPointers[InputPassword[i]].transform.localPosition);
        }
        Vector3 curmouse = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        curmouse.z = 0;
        PasswordLine.SetPosition(InputPassword.Count, curmouse);
        */
    }

    public void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            IsCrack = StartCrack();
        }

        if (Mouse.current.leftButton.isPressed && IsCrack)
        {
            LineDraw();
            int AddPointer = DetectedPoint(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            if (AddPointer > 0)
            {
                InputPassword.Add(AddPointer);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            IsCrack = false;
            Debug.Log("��й�ȣ üũ ���� :" + PasswordCheck());
            InputPassword.Clear();
            LineClear();
        }
    }

    public void LineClear()
    {
        for (int i = 0; i < 10; i++)
        {
            PasswordLine[i].positionCount = 0;
        }
    }  //���� Ŭ����
    public bool PasswordCheck()  //�Է� ��й�ȣ�� ���� ��й�ȣ�� ��
    {
        bool flag = true;
        if (AnswerPassword.Count != InputPassword.Count)
        {
            return false;
        }

        for(int i = 0;  i < InputPassword.Count; i++)
        {
            if (InputPassword[i] != AnswerPassword[i])
            {
                flag = false;
                break;
            }
        }

        return flag;
    }
    public bool StartCrack()
    {
        int StartNum = DetectedPoint(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        if(StartNum >= 0)
        {
            InputPassword.Add(StartNum);
            return true;
        }
        return false;
    } // �� �ձ� ����
    public int DetectedPoint(Vector3 MousePosition) //���� ����� �� �ε��� ��ȯ ������ -1
    {
        MousePosition.z = 0;        
        float MinDistance = 9999999f;
        float PreDistance = 9999999f;
        int ClosePointerIndex = -1;
        for (int i = 0; i < PasswordPointers.Count; i++)
        {
            if (InputPassword.Contains(i))//�̹� �Էµ� ���� ����
                continue;

            PreDistance = Vector2.Distance(MousePosition, PasswordPointers[i].transform.localPosition);
            if (MinDistance > PreDistance)
            {
                ClosePointerIndex = i;
                MinDistance = PreDistance;
            }
        }

        if (MinDistance <= DetectedRange)
        {
            return ClosePointerIndex;
        }

        return -1;
    }
}
