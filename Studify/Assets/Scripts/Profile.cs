using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Profile : MonoBehaviour
{
    public TMP_Text Username;

    public GameObject UniversityList;
    //public static 

    public bool IsUniversity = false;

    private void Awake()
    {
       
        StartCoroutine(UpdateProfile());
    }

    IEnumerator UpdateProfile()
    {
        yield return new WaitUntil(() => DatabaseManager.IsReady);
        Username.text = DatabaseManager.Username();

        Ref<string> IsUniv = new Ref<string>("");
        DatabaseManager.RecieveFromDatabase("IsUniversity", IsUniv);
        yield return new WaitUntil(() => IsUniv.Value != "");

        Debug.Log(IsUniv.Value);

        if(IsUniv.Value == "1")
            IsUniversity = true;
    }

    private void PrepareFeed()
    {
        if(IsUniversity) UniversityList.SetActive(false);
    }
}
