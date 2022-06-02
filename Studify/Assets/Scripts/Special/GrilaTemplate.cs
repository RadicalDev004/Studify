using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrilaTemplate
{
    [TextArea(3, 5)]
    public string Question;
    public string[] Ans = new string[4];
    public int RightAnswer;
}
