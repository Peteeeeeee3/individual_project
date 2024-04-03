using TMPro;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UI.Dialogs;

enum RegisterStatus
{
    NONE,
    FAILED,
    SUCCESS
}

public class LoginScreen : MonoBehaviour
{
    [SerializeField]
    private RectTransform LoginUsernameInput;
    [SerializeField]
    private Vector3 LoginUsernameInputPos;
    [SerializeField]
    private RectTransform LoginPasswordInput;
    [SerializeField]
    private Vector3 LoginPasswordInputPos;
    [SerializeField]
    private Button LoginButton;
    [SerializeField]
    private Vector3 LoginButtonPos;
    [SerializeField]
    private RectTransform RegisterUsernameInput;
    [SerializeField]
    private Vector3 RegisterUsernameInputPos;
    [SerializeField]
    private RectTransform RegisterPasswordInput;
    [SerializeField]
    private Vector3 RegisterPasswordInputPos;
    [SerializeField]
    private Button RegisterButton;
    [SerializeField]
    private Vector3 RegisterButtonPos;
    [SerializeField]
    private TextMeshProUGUI ConnectingText;

    private bool changeScene;
    private bool connSuccess;
    private bool subscribed;
    private bool invalidLogin;
    private RegisterStatus regStatus = RegisterStatus.NONE;
    private Vector3 outOfBoundPos = new Vector3(1000000, 1000000, 1000000);

    // Start is called before the first frame update
    void Start()
    {
        connSuccess = false;
        changeScene = false;
        subscribed = false;
        invalidLogin = false;
        LoginUsernameInput.GetComponent<RectTransform>().position = outOfBoundPos;
        LoginPasswordInput.GetComponent<RectTransform>().position = outOfBoundPos;
        LoginButton.GetComponent<RectTransform>().position = outOfBoundPos;
        RegisterUsernameInput.GetComponent<RectTransform>().position = outOfBoundPos;
        RegisterPasswordInput.GetComponent<RectTransform>().position = outOfBoundPos;
        RegisterButton.GetComponent<RectTransform>().position = outOfBoundPos;
        Thread thread = new Thread(() => Connection.Connect(out connSuccess));
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!subscribed && connSuccess)
        {
            Connection.Subscribe("LOGINSCREEN", this);

            LoginUsernameInput.GetComponent<RectTransform>().localPosition = LoginUsernameInputPos;
            LoginPasswordInput.GetComponent<RectTransform>().localPosition = LoginPasswordInputPos;
            LoginButton.GetComponent<RectTransform>().localPosition = LoginButtonPos;
            RegisterUsernameInput.GetComponent<RectTransform>().localPosition = RegisterUsernameInputPos;
            RegisterPasswordInput.GetComponent<RectTransform>().localPosition = RegisterPasswordInputPos;
            RegisterButton.GetComponent<RectTransform>().localPosition = RegisterButtonPos;
            ConnectingText.GetComponent<RectTransform>().localPosition = outOfBoundPos;

            subscribed = true;
        }

        if (changeScene)
        {
            SceneManager.LoadScene("MainMenu"); // change to main menu, set to game scene for testing
        }

        if (regStatus == RegisterStatus.SUCCESS)
        {
            uDialog.NewDialog().
                SetContentText("Thank you for registering! Please login.").
                AddButton("Close", (dialog) => dialog.Close());

            regStatus = RegisterStatus.NONE;
        }
        else if (regStatus == RegisterStatus.FAILED)
        {
            uDialog.NewDialog().
                SetContentText("Username already in use. Please try a different one.").
                AddButton("Close", (dialog) => dialog.Close());

            regStatus = RegisterStatus.NONE;
        }

        if (invalidLogin)
        {
            uDialog.NewDialog().
                SetContentText("Login failed. Please try again.").
                AddButton("Close", (dialog) => dialog.Close());

            invalidLogin = false;
        }
    }

    /// <summary>
    /// Called when login button is pressed
    /// </summary>
    public void OnLoginClicked()
    {
        TextMeshProUGUI[] UsernameTexts = LoginUsernameInput.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] PasswordTexts = LoginPasswordInput.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI usernameText = null;
        TextMeshProUGUI passwordText = null;
        
        foreach (var text in UsernameTexts)
        {
            if (text.name.Equals("Text"))
            {
                usernameText = text;
            }
        }

        foreach (var text in PasswordTexts)
        {
            if (text.name.Equals("Text"))
            {
                passwordText = text;
            }
        }

        if (usernameText &&
            passwordText &&
            usernameText.text.Length > 0 && 
            passwordText.text.Length > 0)
        {
            // validate user
            string message = "VALIDATE USER\n" + 
                usernameText.text + "\n" + 
                passwordText.text;
            Connection.QueueMessage(message);
        }
    }

    /// <summary>
    /// Called when register button in clicked
    /// </summary>
    public void OnRegisterClicked()
    {
        TextMeshProUGUI[] UsernameTexts = RegisterUsernameInput.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] PasswordTexts = RegisterPasswordInput.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI usernameText = null;
        TextMeshProUGUI passwordText = null;

        foreach (var text in UsernameTexts)
        {
            if (text.name.Equals("Text"))
            {
                usernameText = text;
            }
        }

        foreach (var text in PasswordTexts)
        {
            if (text.name.Equals("Text"))
            {
                passwordText = text;
            }
        }

        if (usernameText &&
            passwordText &&
            usernameText.text.Length > 0 &&
            passwordText.text.Length > 0)
        {
            // validate user
            string message = "REGISTER USER\n" + 
                usernameText.text + "\n" + 
                passwordText.text;
            Connection.QueueMessage(message);
        }
    }

    /// <summary>
    /// Handles user validation response from server
    /// </summary>
    /// <param name="success">Whether user was successfully validated</param>
    /// <param name="userId">User's ID</param>
    public void OnUserValidated(bool success, string userId)
    {
        if (success)
        {
            Globals.ACTIVE_USER_ID = userId;
            changeScene = true;
        }
        else
        {
            invalidLogin = true;   
        }
    }

    /// <summary>
    /// Handles the un-/successful registration of a user
    /// </summary>
    /// <param name="success"></param>
    public void OnUserRegistered(bool success)
    {
        regStatus = success ? RegisterStatus.SUCCESS : RegisterStatus.FAILED;
    }

    /// <summary>
    /// Handles the destruction of this MonoBehaviour class
    /// </summary>
    private void OnDestroy()
    {
        Connection.Unsubscribe("LOGINSCREEN");
    }
}
