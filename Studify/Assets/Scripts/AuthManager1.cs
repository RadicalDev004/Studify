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
public class AuthManager1 : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependecyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    [Header("LogIn")]
    [SerializeField]
    public TMP_InputField emailLoginField;
    [SerializeField]
    public TMP_InputField passwordLoginField;
    [SerializeField]
    public Text warningLoginText;
    [SerializeField]
    public Text confirmLoginText;

    [Header("Register")]
    [SerializeField]
    public TMP_InputField usernameRegisterFiled;
    [SerializeField]
    public TMP_InputField emailRegisterField;
    [SerializeField]
    public TMP_InputField passwordRegiserField;
    [SerializeField]
    public TMP_InputField passwordRegisterVerifyField;
    [SerializeField]
    public Text warningRegisterText;

    [Header("ChangePassword")]

    public TMP_InputField emailForChangePassword;
    public TMP_Text EmailChangePasswordtext;


    [Header("OtherStuff")]

    public string b;

    public GameObject loadBar;
    public string E, P;
    public int aa;

    public GameObject Loading;
    public int r = 0;
    public Text EmailVerificationtext;
    public GameObject verifyEmailButton;
    public GameObject Retry;
    public bool isCon;
    public int m = 0, l;
    int u;

    public void AAAAA(GameObject g)
    {
        g.SetActive(false);
    }

    private void Update()
    {
        E = PlayerPrefs.GetString("E");
        P = PlayerPrefs.GetString("P");
        aa = PlayerPrefs.GetInt("IsLogged");



        if(User!=null )
        {
            User = FirebaseAuth.DefaultInstance.CurrentUser;
            Debug.Log(User.IsEmailVerified);
        }
    }
    IEnumerator  CheckConnection()
    {
        DatabaseReference connectedRef = FirebaseDatabase.DefaultInstance.GetReference(".info/connected");
        connectedRef.ValueChanged += (object sender, ValueChangedEventArgs a) => {
            bool isConnected = (bool)a.Snapshot.Value;
            isCon = isConnected;
            Debug.Log("isConnected" + isConnected);
        };
        yield return null;
    }

    private IEnumerator CheckAndFixDependancies()
    {
        var checkAndFixDependanciesTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixDependanciesTask.IsCompleted);

        var dependancyResult = checkAndFixDependanciesTask.Result;

        if (dependancyResult == DependencyStatus.Available)
        {

            verifyEmailButton.SetActive(false);
            auth = FirebaseAuth.DefaultInstance;
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;

            if (PlayerPrefs.GetInt("IsLogged") == 1)
            {
                if(PlayerPrefs.GetInt("rember")==0)
                {

                }
                else
                {
                    m = 1;
                    Loading.gameObject.SetActive(true);
                    StartCoroutine(Login(PlayerPrefs.GetString("E"), PlayerPrefs.GetString("P")));
                }

            }
            else
            {
                Loading.gameObject.SetActive(false);
            }

            loadBar.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Error on Initialize");
        }
    }
    private void Start()
    {
        if(!PlayerPrefs.HasKey("rember"))
        {
            PlayerPrefs.SetInt("rember", 1);
        }
        if (PlayerPrefs.GetInt("IsLogged") == 1)
        {
            if (PlayerPrefs.GetInt("rember") == 0)
            {

            }
            else
            {
                m = 1;
                Loading.gameObject.SetActive(true);
            }

        }
        else
        {
            Loading.gameObject.SetActive(false);
        }
        StartCoroutine(CheckConnection());
        StartCoroutine(CheckAndFixDependancies());
        Retry.SetActive(false);


    }
    public void RetryConnection()
    {
        Retry.SetActive(false);
        StartCoroutine(Login(PlayerPrefs.GetString("E"), PlayerPrefs.GetString("P")));
        Loading.gameObject.SetActive(true);
    }
    private void InitializeFirebase()
    {
        Debug.Log("Setting Up firebase");
    }



    public void LogInButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    public void Register()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegiserField.text, usernameRegisterFiled.text));
    }
    private IEnumerator Login(string _email, string _password)
    {
        Debug.Log("Staring Login");
        confirmLoginText.text = "";

        loadBar.gameObject.SetActive(true);

        LeanTween.rotate(loadBar.GetComponent<RectTransform>(), -1800f, 10);

        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogError(message: $"failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = (FirebaseException)LoginTask.Exception.GetBaseException();
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed";


            loadBar.gameObject.SetActive(false);
            LeanTween.cancelAll();

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;

                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;

                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;

                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;


                case AuthError.UserNotFound:
                    message = "User Not Found";
                    break;

                case AuthError.RetryPhoneAuth:
                    message = "Restart the app";
                    break;

                case AuthError.NetworkRequestFailed:
                    Debug.Log("No Network");
                    Loading.gameObject.SetActive(false);                   
                    message = "No network";
                    break;

                case AuthError.UserDisabled:
                    message = "User has been disabled";
                    break;
            }

            warningLoginText.text = message;

            if(m==1)
            {
                Loading.gameObject.SetActive(false);
                PlayerPrefs.DeleteKey("E");
                PlayerPrefs.DeleteKey("P");
                PlayerPrefs.SetInt("IsLogged", 0);
            }
            if(isCon == false && m == 1 )
            {
                Retry.SetActive(true);
            }

        }
        else
        {


            User = LoginTask.Result;

            warningLoginText.text = "";
            confirmLoginText.text = "Logged in";


            if (User.IsEmailVerified == false)
            {
                StartCoroutine(SendVerificationEmail());

                //GameObject.Find("LoginUI").GetComponent<LogIn>().Verify();

                warningLoginText.text = "Email not yet verified";

                confirmLoginText.text = "";

                Loading.gameObject.SetActive(false);
                loadBar.gameObject.SetActive(false);
                yield break;
            }

            PlayerPrefs.SetString("E", _email);
            PlayerPrefs.SetString("P", _password);


            yield return new WaitForSeconds(3);


            PlayerPrefs.SetInt("IsLogged", 1);

            SceneManager.LoadScene("MainMenu");

        }
    }
    private IEnumerator Register(string _email, string _password, string _username)
    {


        if (_username == "")
        {
            warningRegisterText.text = "Missing Username";


        }
        else if (passwordRegiserField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password Does Not Match!";


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

                string message = "Register Failed!";

                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;

                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;

                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;

                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;

                    case AuthError.InvalidEmail:
                        message = "Invalid Email";
                        break;

                }
                warningRegisterText.text = message;
            }
            else
            {
                User = RegisterTask.Result;
                confirmLoginText.text = "Account made";

                if (User != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                  
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        warningRegisterText.text = "Username Set Failed";
                    }
                    else
                    {
                        DBreference.Child("Users").Child(User.UserId).Child("username").SetValueAsync(User.DisplayName);
                        //GameObject.Find("LoginUI").GetComponent<LogIn>().Login();
                        warningRegisterText.text = "";
                    }


                }

            }

        }
    }
    public void VerifyYourEmail()
    {
        StartCoroutine(SendVerificationEmail());
    }
  
    private IEnumerator SendVerificationEmail()
    {
        
        if(User!=null)
        {
            var emailTask = User.SendEmailVerificationAsync();
            yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

            if(emailTask.Exception !=null)
            {
                FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();

                AuthError error = (AuthError)firebaseException.ErrorCode;

                string output = "Unkown Error, Try Again";

                switch(error)
                {
                    case AuthError.Cancelled:
                        output = "Verificaton Task Was Canceled";
                        break;

                    case AuthError.InvalidRecipientEmail:
                        output = "Invalid Email";
                        break;

                    case AuthError.TooManyRequests:
                        output = "Too Many Requests";
                        break;

                        



                }
                AwaitVerification(false, User.Email, output);
            }
            else
            {
                AwaitVerification(true, User.Email, null);
                
            }
        }
    }
    
    void AwaitVerification(bool _emailsent, string _email, string _output)
    {
        if (_emailsent)
        {
            EmailVerificationtext.text = "We've sent a verification email to" +  "\"" + $"{_email}" + "\"." + " Please open your email app and verify your account";
        }
        else
        {
            EmailVerificationtext.text = $"Email not Sent: {_output}. Please Verify " + "\"" + $"{_email}" + "\"." + "  and Try Again";
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

            string output = "Unkown Error, Try Again!";

            switch (error)
            {
                case AuthError.Cancelled:
                    output = "Reset Email Task Was Canceled";
                    break;

                case AuthError.InvalidRecipientEmail:
                    output = "Invalid Email";
                    break;

                case AuthError.TooManyRequests:
                    output = "Too Many Requests";
                    break;

                case AuthError.UserNotFound:
                    output = "No user with this email adress found";
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
            EmailChangePasswordtext.text = $"Email Sent! Please Change Your Password at <i>{_email}</i> and Log In!";
        }
        else
        {
            EmailChangePasswordtext.text = $"Email Was Not Sent at <i>{_email}</i>: {_output}. Try Again";
        }
    }

}
