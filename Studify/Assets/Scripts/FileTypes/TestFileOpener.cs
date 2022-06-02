using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestFileOpener : MonoBehaviour
{
    public string FileTitle, FileContent;
    public GameObject TextFileOpenerRect, TextFile;
    private Button OpenButton;

    private void Awake()
    {
        OpenButton = GetComponentInChildren<Button>();
        OpenButton.onClick.AddListener(OpenTextFileWithTitleAndContent);
    }

    public void OpenTextFileWithTitleAndContent()
    {
        Debug.Log("Open File");
        TextFileOpenerRect.SetActive(true);
        TextFile.SetActive(true);
        TextFile.GetComponent<TextFile>().FileName = FileTitle;
        TextFile.GetComponent<TextFile>().FileText = FileContent;
    }
}
