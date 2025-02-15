using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BankWriteMemo : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI WrtieMemoInputText;
    public int AnswerAmount;

    public bool CheckAmount()
    {
        string inputText = WrtieMemoInputText.text.Trim();
        inputText = Regex.Replace(inputText, @"\u200B", "");
        int InputAmount = 0;
        InputAmount = (inputText == "") ? 0 : int.Parse(inputText);
        return (InputAmount ==  AnswerAmount);
    }
    
    void Start()
    {
        
    }

}
