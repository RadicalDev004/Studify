using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Networking;
using Firebase.Storage;
using System;
using System.IO;
public class University : MonoBehaviour
{
    public string Name;
    public string Description;

    private RawImage Logo;
    private TMP_Text tName, tDescription;
    private Button AccesButton;

    private void Start()
    {
        Logo = GetComponentInChildren<RawImage>();
        tName = transform.Find("Name").GetComponentInChildren<TMP_Text>();
        tDescription = transform.Find("Description").GetComponentInChildren<TMP_Text>();
        AccesButton = transform.Find("AccesButton").GetComponent<Button>();

        AccesButton.onClick.AddListener(LoadUniversityScene);

        tName.text = Name;
        name = Name;
        tDescription.text = Description;

        StartCoroutine(WaitForDependacies());
    }

    private void LoadUniversityScene()
    {
        PlayerPrefs.SetString("UniversityScene", Name);
        SceneManager.LoadScene("University");
    }

    private IEnumerator WaitForDependacies()
    {
        yield return new WaitUntil(() => DatabaseManager.IsReady);

        RecieveUniversityLogo();
    }

    private void RecieveUniversityLogo()
    {
        string name = UrifyName(Name);
        StorageReference image = DatabaseManager.StReference().Child("Universities").Child(name).Child("Logo.png");
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCanceled && !task.IsFaulted)
            {
                StartCoroutine(LoadImage(task.Result.ToString()));
            }
            else
            {
                Debug.Log(task.Exception);
            }
        });
    }

    IEnumerator LoadImage(string mediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Logo.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

            float a = Logo.GetComponent<RectTransform>().sizeDelta.x;

            Logo.SetNativeSize();

            float x = Logo.GetComponent<RectTransform>().sizeDelta.x / a;
            float y = Logo.GetComponent<RectTransform>().sizeDelta.y / a;


            if (x >= y)
            {
                Logo.GetComponent<RectTransform>().sizeDelta = new Vector2(x / y * a, y / y * a);

            }
            else if (x < y)
            {
                Logo.GetComponent<RectTransform>().sizeDelta = new Vector2(x / x * a, y / x * a);
            }
        }
    }

    public string UrifyName(string s)
    {
        return s.Replace(" ", "");
    }
}



