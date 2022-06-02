using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrilaManager : MonoBehaviour
{
    public string Title;
    public TMP_Text TTitle;
    public GrilaTemplate[] Grile;
    public GameObject GrilaTemplate;

    public void OnSpawn()
    {
        TTitle.text = Title;
        for(int i = 0; i < Grile.Length; i++)
        {
            GameObject g = Instantiate(GrilaTemplate, transform);
            g.SetActive(true);
            g.GetComponent<Grila>().Question = Grile[i].Question;
            g.GetComponent<Grila>().Ans = Grile[i].Ans;
            g.GetComponent<Grila>().RightAnswer = Grile[i].RightAnswer;
            g.GetComponent<Grila>().enabled = true;
            g.GetComponent<Grila>().SetUI();
            Debug.LogWarning("Spawned grila");
        }
        StartCoroutine(ResetContentSizeFitter(gameObject.GetComponent<ContentSizeFitter>()));
    }
    public static IEnumerator ResetContentSizeFitter(ContentSizeFitter c)
    {
        yield return new WaitForSecondsRealtime(0.25f);
        c.enabled = false;
        yield return new WaitForSecondsRealtime(0.25f);
        c.enabled = true;
    }
}
