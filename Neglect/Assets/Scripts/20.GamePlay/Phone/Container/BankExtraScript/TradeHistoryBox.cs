using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace GamePlay.Container
{
    public class TradeHistoryBox : MonoBehaviour
    {
        public TextMeshProUGUI tradeTopicText; //월세인지 월세 초과금인지.
        public TextMeshProUGUI tradeTypeText; //출금 인지 입금인지
        public TextMeshProUGUI tradeAmount;

        public static string AddCommas(string input)
        {
            input = input.Trim();
            input = Regex.Replace(input, @"\u200B", "");
            int length = input.Length;
            if (length <= 3)
                return input;
            string result = "";
            int count = 0;
            for (int i = length - 1; i >= 0; i--)
            {
                result = input[i] + result;
                count++;

                if (count % 3 == 0 && i != 0)
                {
                    result = "," + result;
                }
            }
            return result;
        }

        public void BoxDataSetting(int type, int amount)
        {
            if (type == -1) // 빈 공간
            {
                tradeAmount.text = "";
                tradeTopicText.text = "";
                tradeTypeText.text = "";
            }
            else if (type == 0)
            {
                tradeAmount.text = "";
                tradeTopicText.text = "최근 거래 내역이\n없습니다.";
                tradeTypeText.text = "";
            }
            else if (type == 1)
            {
                tradeAmount.color = Color.blue;
                tradeAmount.text = "+"+AddCommas(amount.ToString())+"원";
                tradeTopicText.text = "월세 초과금";
                tradeTypeText.text = "입금";
            }
            else if (type == 2)
            {
                tradeAmount.color = Color.red;
                tradeAmount.text = "-"+AddCommas(amount.ToString()) + "원";
                tradeTopicText.text = "월세";
                tradeTypeText.text = "출금";
            }
        }

        public void BoxDataCopy(TradeHistoryBox target)
        {
            tradeTopicText.text = target.tradeTopicText.text;
            tradeTypeText.text = target.tradeTypeText.text;
            tradeAmount.text = target.tradeAmount.text;
            tradeAmount.color =(Color)target.tradeAmount.color;
        }
    }
}