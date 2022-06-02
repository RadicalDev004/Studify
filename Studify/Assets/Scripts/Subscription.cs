using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class Subscription : MonoBehaviour
{
    public GameObject SubscriprionPop, SubsPopAnimation;

    private void Awake()
    {
        StartCoroutine(AwaitDependecies());
    }

    IEnumerator AwaitDependecies()
    {
        yield return new WaitUntil(() => DatabaseManager.IsReady);

        Ref<string> IsSubscribed = new Ref<string>("");
        DatabaseManager.RecieveFromDatabase("IsSubscribed", IsSubscribed);
        yield return new WaitUntil(() => IsSubscribed.Value != "");

        if(IsSubscribed.Value == "0" || IsSubscribed.Value == "-1")
            PopUpSubscribe();
    }

    public void PopUpSubscribe()
    {
        SubscriprionPop.SetActive(true);
        Tween.LocalScale(SubsPopAnimation.transform, Vector3.one, 0.3f, 0, Tween.EaseIn);
    }
    public void PopDownSubscribe()
    {
        StartCoroutine(IPopDownSubscribe());
    }

    private IEnumerator IPopDownSubscribe()
    {
        Tween.LocalScale(SubsPopAnimation.transform, Vector3.zero, 0.3f, 0, Tween.EaseIn);
        yield return new WaitForSecondsRealtime(0.3f);
        SubscriprionPop.SetActive(false);
    }
    public void Refuse()
    {
        Application.Quit();
    }
    public void Subscribe()
    {
        StartCoroutine(IUpdateDatabase("IsSubscribed", "1"));
        StartCoroutine(IPopDownSubscribe());
    }

    private IEnumerator IUpdateDatabase(string path, string key)
    {
        Debug.Log("Trying to update database");
        DatabaseManager.DbReference().Child("Users").Child(DatabaseManager.User().UserId).Child(path).SetValueAsync(key);
        //yield return new WaitUntil(predicate: () => task.IsCompleted);
        yield return null;
        //Debug.LogError(key + " " + DatabaseManager.DbReference().Child("Users").Child(DatabaseManager.User().UserId).Child(path).GetValueAsync().Result.Value.ToString());
    }
}
