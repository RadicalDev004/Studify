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

public class ProfilePic : MonoBehaviour
{
    public RawImage ppImage;

    public int haspp;
    string profilePictureFolder;

    void Start()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        StartCoroutine(UpdateFirebase());

       
    }

    IEnumerator UpdateFirebase()
    {
        yield return new WaitUntil(() => DatabaseManager.IsReady);
        profilePictureFolder = DatabaseManager.User().UserId + "/picture";

        StartCoroutine(HasPictureOrnot());
    }

    public void SelectProfilePicToUpload()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }
    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
                Debug.Log(FileBrowser.Result[i]);
            Debug.Log("File Selected");

            
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
            long x = FileBrowserHelpers.GetFilesize(FileBrowser.Result[0]);

            Debug.LogError(bytes.Length);

            byte[] compressed = CompressionHelper.CompressBytes(bytes);

            Debug.Log("New size: " + compressed.Length);
            var newMetaData = new MetadataChange();

            newMetaData.ContentType = "image/jpeg";


            if (haspp == 1)
            {
                StartCoroutine(DeleteProfilePic());
            }

            

            StorageReference uploadRef = DatabaseManager.StReference().Child("profilePictures").Child(DatabaseManager.User().UserId).Child("picture");
            Debug.Log("File Upload started");
           


            uploadRef.PutBytesAsync(bytes, newMetaData).ContinueWithOnMainThread((task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception);
                }
                else
                {
                    Debug.Log("File uploaded succesfully");
                }
            });

            StorageReference image = DatabaseManager.StReference().Child("profilePictures").Child(DatabaseManager.User().UserId).Child("picture");
            image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    StartCoroutine(LoadImage(task.Result.ToString()));
                }
                else
                {
                    Debug.Log(task.Exception);
                }
            });

            Debug.Log("Finished Broswer dialogue");

        }
    }
    IEnumerator LoadImage(string mediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            ppImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

            float a = ppImage.GetComponent<RectTransform>().sizeDelta.x;

            ppImage.SetNativeSize();

            float x = ppImage.GetComponent<RectTransform>().sizeDelta.x / a;
            float y = ppImage.GetComponent<RectTransform>().sizeDelta.y / a;
            

            if (x >= y)
            {
                ppImage.GetComponent<RectTransform>().sizeDelta = new Vector2(x / y * a, y / y * a);

            }
            else if (x < y)
            {
                ppImage.GetComponent<RectTransform>().sizeDelta = new Vector2(x / x * a, y / x * a);
            }

            var task = DatabaseManager.DbReference().Child("Users").Child(DatabaseManager.User().UserId).Child("hasProfilePicture").SetValueAsync(1);
            //yield return new WaitUntil(predicate: () => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.Log(task.Exception);
            }
            else
            {
                haspp = 1;
                Debug.Log("Load profile image");
            }
        }
    }
    IEnumerator DeleteProfilePic()
    {
        var deleteTask = DatabaseManager.StReference().Child("profilePictures").Child(profilePictureFolder).Child("picture").DeleteAsync();
        yield return new WaitUntil(predicate: () => deleteTask.IsCompleted);

    }
    IEnumerator HasPictureOrnot()
    {
        var task = DatabaseManager.DbReference().Child("Users").Child(DatabaseManager.User().UserId).Child("hasProfilePicture").GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Exception != null)
        {
            haspp = 0;
            Debug.Log("No data");
        }
        else
        {
            DataSnapshot d = task.Result;

            if(d.Value == null)
            {
                haspp = 0;
            }
            else
            {
                haspp = int.Parse(d.Value.ToString());
            }
         

            if (haspp == 1)
            {
                StorageReference image = DatabaseManager.StReference().Child("profilePictures").Child(DatabaseManager.User().UserId).Child("picture");
                image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
                {
                    if (!task.IsCanceled && !task.IsFaulted)
                    {
                        StartCoroutine(LoadImage(task.Result.ToString()));
                    }
                    else
                    {
                        Debug.Log(task.Exception);
                    }
                });
            }
        }
    }
    IEnumerator CheckIFHasProfilePicturePeriodically()
    {
        while (true)
        {
            StartCoroutine(HasPictureOrnot());
            yield return new WaitForSeconds(3);
        }
    }
}
