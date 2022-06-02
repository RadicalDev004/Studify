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

public class Auth : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependecyStatus;
    public FirebaseUser user;
    public FirebaseAuth auth;
    public DatabaseReference DBreference;

    [Header("LogIn")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    [Header("Register")]
    public TMP_InputField usernameRegisterFiled;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegiserField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;
    public TMP_Text confirmRegisterText;

    [Header("ChangePassword")]
    public TMP_InputField emailForChangePassword;
    public TMP_Text EmailChangePasswordText;

    [Header("Verify Email")]
    public TMP_Text EmailVerificationtext;

    [Header("Other")]
    public bool Remember;
    public GameObject LoadingScreen;
    public GameObject LoadingCircle;


    private void Awake()
    {
        Remember = LoadRemember();
        StartCoroutine(CheckAndFixDependancies());
        LoadingScreen.SetActive(false);
        LoadingCircle.SetActive(false);
    }


    private void InitializeFirebase()
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;

        if (Remember && PlayerPrefs.GetInt("LoggedIn") == 1)
        {
            StartCoroutine(Login(PlayerPrefs.GetString("InternalEmail"), PlayerPrefs.GetString("InternalPassword")));
            LoadingScreen.SetActive(true);
        }
    }

    public void LoginFromVerified()
    {
        StartCoroutine(Login(PlayerPrefs.GetString("InternalEmail"), PlayerPrefs.GetString("InternalPassword")));
    }

    public void Login()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    public void Register()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegiserField.text, usernameRegisterFiled.text));
    }
    private bool LoadRemember() => PlayerPrefs.GetInt("Remember") == 0;


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
 

    private IEnumerator Login(string _email, string _password)
    {
        Debug.Log("Staring Login");
        LoadingCircle.SetActive(true);

        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogError(message: $"failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = (FirebaseException)LoginTask.Exception.GetBaseException();
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Autentificarea a esuat";

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Lipsa Email";
                    break;

                case AuthError.MissingPassword:
                    message = "Lipsa Parola";
                    break;

                case AuthError.WrongPassword:
                    message = "Parola Gresita";
                    break;

                case AuthError.InvalidEmail:
                    message = "Email Invalid";
                    break;


                case AuthError.UserNotFound:
                    message = "Utilizator inexistent";
                    break;

                case AuthError.RetryPhoneAuth:
                    message = "Restartati aplicatia";
                    break;

                case AuthError.NetworkRequestFailed:
                    message = "Fara conexiune";
                    break;

                case AuthError.UserDisabled:
                    message = "Utilizator dezactivat";
                    break;
            }
            LoadingCircle.SetActive(false);
            warningLoginText.text = message;
        }
        else
        {
            user = LoginTask.Result;


            PlayerPrefs.SetString("InternalPassword", _password);
            PlayerPrefs.SetString("InternalEmail", _email);

            if (user.IsEmailVerified == false)
            {
                LoadingCircle.SetActive(false);
                FindObjectOfType<UI>().LoadPage(3);
                StartCoroutine(SendVerificationEmail());
                yield break;
            }

            PlayerPrefs.SetInt("LoggedIn", 1);
            LoadingCircle.SetActive(false);
            LoadingScreen.SetActive(true);
            yield return new WaitForSecondsRealtime(1);
            SceneManager.LoadScene("Menu");
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        LoadingCircle.SetActive(true);

        if (_username == "")
        {
            warningRegisterText.text = "Lipsa Nume";
        }
        else if (passwordRegiserField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Parolele nu sunt la fel!";
        }
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Creare Cont Esuata";

                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Lipsa Email";
                        break;

                    case AuthError.MissingPassword:
                        message = "Lipsa Parola";
                        break;

                    case AuthError.WeakPassword:
                        message = "Parola Slaba";
                        break;

                    case AuthError.EmailAlreadyInUse:
                        message = "Email deja folosit";
                        break;

                    case AuthError.InvalidEmail:
                        message = "Email Invalid";
                        break;

                }
                LoadingCircle.SetActive(false);
                warningRegisterText.text = message;
            }
            else
            {
                user = RegisterTask.Result;
                confirmRegisterText.text = "Cont Creat";

                if (user != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    var ProfileTask = user.UpdateUserProfileAsync(profile);


                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        warningRegisterText.text = "Username Set Failed";
                    }
                    else
                    {

                        DBreference.Child("Users").Child(user.UserId).Child("username").SetValueAsync(user.DisplayName);
                        warningRegisterText.text = "";
                    }
                }
                yield return new WaitForSecondsRealtime(1);
                FindObjectOfType<UI>().LoadPage(0);
                LoadingCircle.SetActive(false);
            }

        }
    }

    private IEnumerator SendVerificationEmail()
    {

        if (user != null)
        {
            var emailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

            if (emailTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();

                AuthError error = (AuthError)firebaseException.ErrorCode;

                string output = "Eroare nestiuta. Incercati iar!";

                switch (error)
                {
                    case AuthError.Cancelled:
                        output = "Verificare oprita";
                        break;

                    case AuthError.InvalidRecipientEmail:
                        output = "Email Invalid";
                        break;

                    case AuthError.TooManyRequests:
                        output = "Prea multe incercari";
                        break;

                }
                AwaitVerification(false, user.Email, output);
            }
            else
            {
                AwaitVerification(true, user.Email, null);

            }
        }
    }


    void AwaitVerification(bool _emailsent, string _email, string _output)
    {
        if (_emailsent)
        {
            EmailVerificationtext.text = "Am trimis un email la" + "\"" + $"{_email}" + "\"." + " Va rugam deschide-ti aplcatia de Email si verificati contul!";
        }
        else
        {
            EmailVerificationtext.text = $"Email netrimis: {_output}. Va rugam verificati " + "\"" + $"{_email}" + "\"." + " si incercati iar.";
        }
    }

    public void ChanegPassword()
    {
        StartCoroutine(SendChangePasswordEmail());
    }
    private IEnumerator SendChangePasswordEmail()
    {

        var emailTask = auth.SendPasswordResetEmailAsync(emailForChangePassword.text);
        yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

        if (emailTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();

            AuthError error = (AuthError)firebaseException.ErrorCode;

            string output = "Eroare nestiuta, incercati iar!";

            switch (error)
            {
                case AuthError.Cancelled:
                    output = "Resetarea de parola a fost oprita.";
                    break;

                case AuthError.InvalidRecipientEmail:
                    output = "Email Invalid";
                    break;

                case AuthError.TooManyRequests:
                    output = "Prea multe incercari!";
                    break;

                case AuthError.UserNotFound:
                    output = "Niciun utilizator cu acest Email nu a fost gasit";
                    break;




            }
            AwaitPasswordChange(false, emailForChangePassword.text, output);
        }
        else
        {
            AwaitPasswordChange(true, emailForChangePassword.text, null);
        }
    }
    void AwaitPasswordChange(bool _emailsent, string _email, string _output)
    {
        if (_emailsent)
        {
            EmailChangePasswordText.text = $"Email trimis! Va rugam sa va schimbati parola la <i>{_email}</i> si apoi sa va autentificati!";
        }
        else
        {
            EmailChangePasswordText.text = $"Email netrimis la <i>{_email}</i>: {_output}. Incercati iar.";
        }
    }
}
