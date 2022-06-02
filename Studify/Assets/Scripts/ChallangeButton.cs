using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using RadicalKit;
public class ChallangeButton : MonoBehaviour
{
    public Challange challange;
    public int Number;

    public enum Type { Daily, Weekly }
    public Type type;

    private TMP_Text Title, Description, Reward;
    private GameObject DescriptionImg;
    private Button Info, CloseInfo;
    private Button Complete;

    private void Start()
    {
        DescriptionImg = transform.Find("DescriptionImg").gameObject;
        Title = transform.Find("Title").GetComponent<TMP_Text>();
        Description = DescriptionImg.transform.Find("Description").GetComponent<TMP_Text>();
        Reward = transform.Find("Reward").GetComponent<TMP_Text>();
        Info = transform.Find("Info").GetComponent<Button>();
        CloseInfo = DescriptionImg.transform.Find("Close").GetComponent<Button>();
        Complete = transform.Find("Complete").GetComponent<Button>();

        Info.onClick.AddListener(OpenInfoFunc);
        CloseInfo.onClick.AddListener(CloseInfoFunc);
        Complete.onClick.AddListener(OnComplete);

        Title.text = challange.Title;
        Description.text = challange.Description;
        Reward.text = "+" + challange.Reward.ToString();
    }

    public void OpenInfoFunc()
    {
        DescriptionImg.SetActive(true);
    }
    public void CloseInfoFunc()
    {
        DescriptionImg.SetActive(false);
    }

    public void OnComplete()
    {
        Prefs.IncreaseInt("Currency", challange.Reward);

        if (type == Type.Daily) { Prefs.DecreaseString("Daily", Number.ToString()); FindObjectOfType<ChallangeManager>().DailyCount--; }
        else {Prefs.DecreaseString("Weekly", Number.ToString()); FindObjectOfType<ChallangeManager>().WeeklyCount--; }

        Destroy(gameObject);
    }
}
