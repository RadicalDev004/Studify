using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyFile : MonoBehaviour
{
    public string FileName;
    public string FileContent;
    public FileType Type;
    public GameObject Choice;

    public enum FileType { Text, Choice }
        
}
