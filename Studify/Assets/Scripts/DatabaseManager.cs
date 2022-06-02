using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Firebase.Storage;
using System;
using System.IO;
using SimpleFileBrowser;

public class DatabaseManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependecyStatus;
    public FirebaseUser user;
    public FirebaseAuth auth;
    public DatabaseReference DBreference;
    public FirebaseStorage storage;
    public StorageReference STreference;

    [Header("Other")]
    public static DatabaseManager Instance;
    public string Result;
    public static bool IsReady;

    void Awake()
    {
        IsReady = false;
        Instance = this;
        StartCoroutine(CheckAndFixDependancies());
    }

    public static string Username() => Instance.user.DisplayName;

    public static FirebaseUser User() => Instance.user;
    public static FirebaseAuth Auth() => Instance.auth;
    public static FirebaseStorage Storage() => Instance.storage;
    public static DatabaseReference DbReference() => Instance.DBreference;
    public static StorageReference StReference() => Instance.STreference;


    public static void RecieveFromDatabase(string path, Ref<string> reff)
    {
        Instance.StartCoroutine(Instance.IRecieveFromDatabase(path, reff));
    }

    public static void UpdateDatabase(string path, string key) => Instance.StartCoroutine(Instance.IUpdateDatabase(path, key));

    public IEnumerator IUpdateDatabase(string path, string key)
    {
        Debug.Log("Trying to update database");
        DBreference.Child("Users").Child(user.UserId).Child(path).SetValueAsync(key);
        //yield return new WaitUntil(predicate: () => task.IsCompleted);
        
        Debug.LogError(key + " " + DBreference.Child("Users").Child(user.UserId).Child(path).GetValueAsync().Result.Value.ToString());
        yield return null;
    }

    public IEnumerator IRecieveFromDatabase(string path, Ref<string> reff)
    {
        var task = Instance.DBreference.Child("Users").Child(Instance.user.UserId).Child(path).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
        }
        else if (task.Result == null)
        {
            Debug.LogWarning("No data can be found");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            if(!snapshot.Exists)
            {
                reff.Value = "-1"; 
                yield break;
            }
            reff.Value = snapshot.Value.ToString();
        }
    }

    private IEnumerator CheckAndFixDependancies()
    {
        var checkAndFixDependanciesTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixDependanciesTask.IsCompleted);

        var dependancyResult = checkAndFixDependanciesTask.Result;

        if (dependancyResult == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            Debug.LogError("Error on Initialize");
        }
    }
    private void InitializeFirebase()
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        storage = FirebaseStorage.DefaultInstance;

        STreference = storage.GetReferenceFromUrl("gs://studify-6ddd5.appspot.com");

        IsReady = true;
    }
}

public class Ref<T>
{
    private T backing;
    public T Value { get { return backing; } set { backing = value; } }
    public Ref(T reference)
    {
        backing = reference;
    }
}

