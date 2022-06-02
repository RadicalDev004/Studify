using UnityEngine;
public class UI : MonoBehaviour
{
    public GameObject[] Pages;  // LogIn = 0, SignUp = 1, ChangePassword = 2, VerifyEmail = 3, FirstPage = 4;
    public GameObject RememberTick;
    public bool Remember { get { return FindObjectOfType<Auth>().Remember; } set { FindObjectOfType<Auth>().Remember = value; } }

    private void Awake()
    {
        LoadPage(4);
    }
    private void Start()
    {
        PrepareUi();
    }

    public void LoadPage(int page)
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            if (page == i)
                Pages[i].SetActive(true);
            else
                Pages[i].SetActive(false);
        }
    }

    public void RememberButton()
    {
        Remember = !Remember;

        if (Remember) {
            RememberTick.SetActive(true);
            PlayerPrefs.SetInt("Remember", 0);
        } else {
            RememberTick.SetActive(false); 
            PlayerPrefs.SetInt("Remember", 1); 
        }
    }


    private void PrepareUi()
    {
        if (Remember) RememberTick.SetActive(true);
        else RememberTick.SetActive(false);
    }


    /*public bool AnimationCooldown;

    private void OpenTab(Image image)
    {
        if (AnimationCooldown) return;

        AnimationCooldown = true;
        Tween.LocalScale(image.GetComponent<RectTransform>(), Vector3.one, 0.5f, 0, Tween.EaseInOut);
    }
    private void CloseTab(Image image)
    {

        Tween.LocalScale(image.GetComponent<RectTransform>(), Vector3.zero, 0.5f, 0, Tween.EaseInOut);
        Invoke(nameof(ResetAnimationCooldown), 0.5f);
    }

    private void ResetAnimationCooldown() => AnimationCooldown = false;*/
}
