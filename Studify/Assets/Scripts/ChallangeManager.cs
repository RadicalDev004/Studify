using RadicalKit;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class ChallangeManager : MonoBehaviour
{
    public List<Challange> DailyChallanges, WeeklyChallanges;

    public GameObject Daily, Weekly, ChallangeButtonTemplate;

    public int HoursLeftUntilNextDailyRefresh;
    public int DaysLeftUntilNextWeeklyRefresh;

    public int DailyCount, WeeklyCount;

    private int LastRefreshedDay, LastRefreshedWeek;
    private bool HasFinishedInitialization;

    public string a;

    private void Start()
    {

        LastRefreshedDay = PlayerPrefs.GetInt("LRD");
        LastRefreshedWeek = PlayerPrefs.GetInt("LRW");

       
    }

    private void Update()
    {
        Debug.LogWarning(a);
        if (!HasFinishedInitialization) return;

        if (LastRefreshedDay != DateTime.UtcNow.Day)
        {
            RefreshDaily();
        }
        else HoursLeftUntilNextDailyRefresh = 24 - DateTime.UtcNow.Hour;


        if (LastRefreshedWeek != DateTimeExtensions.GetWeekOfMonth(DateTime.UtcNow))
        {
            RefreshWeekly();
        }
        else DaysLeftUntilNextWeeklyRefresh = 7 - (int)DateTime.UtcNow.DayOfWeek;

        //Debug.Log(PlayerPrefs.GetString("Daily") + " " + PlayerPrefs.GetString("Weekly"));
    }

    private void OnApplicationQuit()
    {
        DatabaseManager.UpdateDatabase("Daily", PlayerPrefs.GetString("Daily"));
        DatabaseManager.UpdateDatabase("Weekly", PlayerPrefs.GetString("Weekly"));
        DatabaseManager.UpdateDatabase("Currency", PlayerPrefs.GetInt("Currency").ToString());

        Debug.LogWarning("Saved Progress");

    }

    private void RefreshDaily()
    {
        LastRefreshedDay = DateTime.UtcNow.Day;
        PlayerPrefs.SetInt("LRD", LastRefreshedDay);

        for (int i = 0; i < DailyCount; i++)
        {
            int r = UnityEngine.Random.Range(0, DailyChallanges.Count);

            while (PlayerPrefs.GetString("Daily").Contains(r.ToString()))
            {
                r = UnityEngine.Random.Range(0, DailyChallanges.Count);
            }

            Prefs.IncreaseString("Daily", r.ToString());
        }

        DeleteAllChildren(Daily);
        SpawnDailyChallanges(Daily, "Daily");

        //Debug.Log(PlayerPrefs.GetString("Daily"));
        Debug.Log("Refreshed Daily");
    }

    private void RefreshWeekly()
    {
        LastRefreshedWeek = DateTimeExtensions.GetWeekOfMonth(DateTime.UtcNow);
        PlayerPrefs.SetInt("LRW", LastRefreshedWeek);



        for (int i = 0; i < WeeklyCount; i++)
        {
            int r = UnityEngine.Random.Range(0, WeeklyChallanges.Count);

            while (PlayerPrefs.GetString("Weekly").Contains(r.ToString()))
            {
                r = UnityEngine.Random.Range(0, WeeklyChallanges.Count);
            }

            Prefs.IncreaseString("Weekly", r.ToString());
        }

        DeleteAllChildren(Weekly);
        SpawnWeeklyChallanges(Weekly, "Weekly");

        //Debug.Log(PlayerPrefs.GetString("Weekly"));
        Debug.Log("Refreshed Weekly");
    }

    private void SpawnDailyChallanges(GameObject parent, string PPname)
    {
        string ch = PlayerPrefs.GetString(PPname);
        int count = ch.Length;

        for (int i = 0; i < count; i++)
        {
            int ChNr = int.Parse(ch[i].ToString());

            GameObject g = Instantiate(ChallangeButtonTemplate, parent.transform);
            g.GetComponent<ChallangeButton>().challange = DailyChallanges[ChNr];
            g.GetComponent<ChallangeButton>().Number = ChNr;
            g.GetComponent<ChallangeButton>().type = ChallangeButton.Type.Daily;
        }
    }

    private void SpawnWeeklyChallanges(GameObject parent, string PPname)
    {
        string ch = PlayerPrefs.GetString(PPname);
        int count = ch.Length;

        for (int i = 0; i < count; i++)
        {
            int ChNr = int.Parse(ch[i].ToString());

            GameObject g = Instantiate(ChallangeButtonTemplate, parent.transform);
            g.GetComponent<ChallangeButton>().challange = WeeklyChallanges[ChNr];
            g.GetComponent<ChallangeButton>().Number = ChNr;
            g.GetComponent<ChallangeButton>().type = ChallangeButton.Type.Weekly;
        }
    }

    private void DeleteAllChildren(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Destroy(obj.transform.GetChild(i).gameObject);
        }
    }

}

static class DateTimeExtensions
{
    static readonly GregorianCalendar _gc = new GregorianCalendar();
    public static int GetWeekOfMonth(this DateTime time)
    {
        DateTime first = new DateTime(time.Year, time.Month, 1);
        return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
    }

    static int GetWeekOfYear(this DateTime time)
    {
        return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    }
}
