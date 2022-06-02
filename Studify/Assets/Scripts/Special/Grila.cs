using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Grila : MonoBehaviour
{
    public string Question;
    public string[] Ans;
    public int RightAnswer;

    public TMP_Text TQuestion;
    public TMP_Text[] TAns = new TMP_Text[4];
    public Button[] AnswersB = new Button[4];

    public void SetUI()
    {
        TQuestion = transform.Find("Question").GetComponent<TMP_Text>();
        TQuestion.text = Question;

        for (int i = 0; i < 4; i++)
        {
            AnswersB[i] = transform.Find("AnswerButton" + i).GetComponent<Button>();
            int a = i;
            AnswersB[i].onClick.AddListener(delegate
            {
                ChooseOption(a);
            });
        }

        for (int i =0; i<4; i++)
        {
            TAns[i] = AnswersB[i].transform.Find("Answer" + i).GetComponent<TMP_Text>();
            TAns[i].text = Ans[i];
        }
    }

    public void ChooseOption(int option)
    {
        Debug.LogWarning(option);
        if (option == RightAnswer) {
            AnswersB[option].GetComponent<Image>().color = Color.green;

            foreach (Button button in AnswersB)
                button.onClick.RemoveAllListeners();
        } else {
            AnswersB[option].GetComponent<Image>().color = Color.red;
            AnswersB[RightAnswer].GetComponent<Image>().color = Color.green;

            foreach (Button button in AnswersB)
                button.onClick.RemoveAllListeners();
        }
    }
}
