using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
[System.Serializable]
public class Challange
{
    public string Title;
    [TextArea(5, 5)]
    public string Description;
    public int Reward;
}
