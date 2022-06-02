using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;

public class TextFile : MonoBehaviour
{
    public string FileName;
    public string FileText;

    private Image Instance;
    private TMP_Text title, text;
    private Button Close;

    public int Progress;
    public string FileKey;

    public bool HasFinishedFileLoad = false;

    private void Awake()
    {
        Instance = GetComponent<Image>();
        title = transform.Find("Title").GetComponent<TMP_Text>();
        text = transform.Find("Text").GetComponent<TMP_Text>();
        Close = transform.parent.Find("Close").GetComponent<Button>();

        

        Close.onClick.AddListener(CloseFile);
    }

    private void OnEnable()
    {
        StartCoroutine(ResetContentSizeFitter());       
    }

    private void Update()
    {
        SetFileAttributes();
     
        SetProgress();
    }

    public void CloseFile()
    {
        FileName = null;
        FileText = null;
        transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void SetProgress()
    {
        if (!HasFinishedFileLoad) return;
        Progress = (int)Instance.GetComponent<RectTransform>().anchoredPosition.y * 100 / (int)Instance.GetComponent<RectTransform>().sizeDelta.y;
        PlayerPrefs.SetInt(FileKey + "Progress", Progress);
    }

    private void SetFileAttributes()
    {
        title.text = FileName;
        text.text = FileText;
    }

    private void LoadProgress()
    {
        Debug.Log(PlayerPrefs.GetInt(FileKey + "Progress"));
        Debug.Log(PlayerPrefs.GetInt(FileKey + "Progress") * (int)Instance.GetComponent<RectTransform>().sizeDelta.y / 100);
        Tween.AnchoredPosition(Instance.GetComponent<RectTransform>(), new Vector2(0, PlayerPrefs.GetInt(FileKey + "Progress") * (int)Instance.GetComponent<RectTransform>().sizeDelta.y / 100), 0.5f, 0, Tween.EaseOut);
        //Instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, PlayerPrefs.GetInt(FileKey + "Progress") * (int)Instance.GetComponent<RectTransform>().sizeDelta.y /100);
    }

    IEnumerator ResetContentSizeFitter()
    {
        yield return new WaitUntil(() => title.text != "");
        Debug.Log("Resetting fitter");
        GetComponentInParent<ContentSizeFitter>().enabled = false;
        yield return new WaitForSecondsRealtime(0.5f);
        GetComponentInParent<ContentSizeFitter>().enabled = true;
        yield return new WaitForSecondsRealtime(0.1f);
        FileKey = FindObjectOfType<UniversityScene>().Name + FileName;
        LoadProgress();
        HasFinishedFileLoad = true;
    }
}
