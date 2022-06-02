using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GrilaFileOpener : MonoBehaviour
{
    public string GrilaTitle;
    public GameObject GrilaFileOpenerRect, GrileFile;
    private Button OpenButton;

    private void Awake()
    {
        OpenButton = GetComponentInChildren<Button>();
        OpenButton.onClick.AddListener(OpenTextFileWithTitleAndContent);
    }

    public void OpenTextFileWithTitleAndContent()
    {
        Debug.Log("Open File");
        GrilaFileOpenerRect.SetActive(true);
        GrileFile.SetActive(true);
        GrileFile.GetComponent<GrilaManager>().Title = GrilaTitle;
        GrileFile.GetComponent<GrilaManager>().OnSpawn();
    }

    public void CloseGrila()
    {
        for(int i = 2; i<GrileFile.transform.childCount; i++)
        {
            Destroy(GrileFile.transform.GetChild(i).gameObject);
        }

        GrileFile.SetActive(false);
        GrilaFileOpenerRect.SetActive(false);
    }
}
