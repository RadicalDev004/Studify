using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;
using UnityEngine.Events;
using RadicalKit;
using TMPro;
using UnityEngine.SceneManagement;
public class MenuUi : MonoBehaviour
{
    public bool AnimationCooldown;
    public Image Side;
    private void Awake()
    {

    }

    private void Update()
    {

    }

    public void SlideTab(Image Image, Vector3 pos)
    {
        Tween.AnchoredPosition(Image.GetComponent<RectTransform>(), pos, 0.25f, 0, Tween.EaseIn);
    }

    public void OpenSideTab()
    {
        Tween.AnchoredPosition(Side.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0, Tween.EaseIn);
    }
    public void CloseSideTab()
    {
        Tween.AnchoredPosition(Side.GetComponent<RectTransform>(), new Vector2(0, -1920), 0.5f, 0, Tween.EaseIn);
    }

    public void LogOut()
    {
        PlayerPrefs.SetInt("LoggedIn", 0);
        SceneManager.LoadScene("LogIn");
    }
}
