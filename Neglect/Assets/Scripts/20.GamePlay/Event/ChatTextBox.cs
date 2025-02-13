using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace GamePlay.Event
{
    public class ChatTextBox : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI Text;
        public void SetText(string text)
        {
            Text.text = InsertNewlines(text,10);
        }
        public static string InsertNewlines(string input, int chunkSize = 10)
        {
            return Regex.Replace(input, $"(.{{{chunkSize}}})", "$1\n");
        }

    }
}