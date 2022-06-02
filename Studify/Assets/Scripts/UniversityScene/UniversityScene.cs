using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Firebase.Storage;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class UniversityScene : MonoBehaviour
{
    public string Name;
    public string Description;
    public TMP_Text tName;
    public RawImage Logo;

    private void Awake()
    {
        Name = PlayerPrefs.GetString("UniversityScene");
        tName.text = Name;
    }

    private void Start()
    {
        RecieveUniversityLogo();
    }

    public void Exit()
    {
        Debug.Log("Test message");
        SceneManager.LoadScene("Menu");
    }

    private void RecieveUniversityLogo()
    {
        StorageReference image = DatabaseManager.StReference().Child("Universities").Child(Name.Replace(" ", "")).Child("Logo.png");
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
}

