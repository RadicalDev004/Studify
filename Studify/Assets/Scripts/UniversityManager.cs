using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class UniversityManager : MonoBehaviour
{
    public GameObject UniversityTemplate;

    private void Awake()
    {
        StartCoroutine(AwaitDependacies());
    }

    private IEnumerator AwaitDependacies()
    {
        yield return new WaitUntil(() => DatabaseManager.IsReady);

        DatabaseManager.DbReference().Child("Universities").ChildAdded += HandleChildAdded;
    }
    void HandleChildAdded(object o, ChildChangedEventArgs args)
    {
        DataSnapshot d = args.Snapshot;

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        GameObject Univ = Instantiate(UniversityTemplate, transform);

        Univ.SetActive(true);
        Univ.GetComponent<University>().enabled = true;
        Univ.GetComponent<University>().Name = d.Key;
        Univ.GetComponent<University>().Description = d.Child("Description").Value.ToString();
    }
}
