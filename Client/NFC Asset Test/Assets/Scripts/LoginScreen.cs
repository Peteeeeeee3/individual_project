using TMPro;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class LoginScreen : MonoBehaviour
{
    [SerializeField]
    private RectTransform UsernameInput;
    [SerializeField]
    private Vector3 UsernameInputPos;
    [SerializeField]
    private RectTransform PasswordInput;
    [SerializeField]
    private Vector3 PasswordInputPos;
    [SerializeField]
    private Button LoginButton;
    [SerializeField]
    private Vector3 LoginButtonPos;
    [SerializeField]
    private Button RegisterButton;
    [SerializeField]
    private Vector3 RegisterButtonPos;
    [SerializeField]
    private TextMeshProUGUI ConnectingText;

    private bool changeScene;
    private bool connSuccess;
    private Vector3 outOfBoundPos = new Vector3(1000000, 1000000, 1000000);

    // Start is called before the first frame update
    void Start()
    {
        connSuccess = false;
        changeScene = false;
        UsernameInput.GetComponent<RectTransform>().position = outOfBoundPos;
        PasswordInput.GetComponent<RectTransform>().position = outOfBoundPos;
        LoginButton.GetComponent<RectTransform>().position = outOfBoundPos;
        RegisterButton.GetComponent<RectTransform>().position = outOfBoundPos;
        Thread thread = new Thread(() => Connection.Connect(out connSuccess));
    }

    // Update is called once per frame
    void Update()
    {
        if (connSuccess)
        {
            Connection.Subscribe("LOGINSCREEN", this);

            UsernameInput.GetComponent<RectTransform>().position = UsernameInputPos;
            PasswordInput.GetComponent<RectTransform>().position = PasswordInputPos;
            LoginButton.GetComponent<RectTransform>().position = LoginButtonPos;
            RegisterButton.GetComponent<RectTransform>().position = RegisterButtonPos;
            ConnectingText.GetComponent<RectTransform>().position = outOfBoundPos;
        }

        if (changeScene)
        {
            SceneManager.LoadScene("GameScene"); // change to main menu, set to game scene for testing
        }
    }

    /// <summary>
    /// Called when login button is pressed
    /// </summary>
    public void OnLoginClicked()
    {
        if (UsernameInput.GetComponent<TextMeshProUGUI>().text.Length > 0 && PasswordInput.GetComponent<TextMeshProUGUI>().text.Length > 0)
        {
            // validate user
            string message = "VALIDATE USER" + "\n" + UsernameInput.GetComponent<TextMeshProUGUI>().text + "\n" + PasswordInput.GetComponent<TextMeshProUGUI>().text;
            Connection.QueueMessage(message);
        }
    }


    public void OnUserValidated(bool success, string userId)
    {
        if (success)
        {
            Globals.ACTIVE_USER_ID = userId;
            changeScene = true;
        }
        else
        {
            // prompt an error popup
        }
    }

    /// <summary>
    /// Called when register button in clicked
    /// </summary>
    public void OnRegisterClicked()
    {

    }
    /*
    private void CreateTempFile()
    {
        FileStream fileStream = null;
        if (!File.Exists(FileLoc))
        {
            using (fileStream = File.Create(FileLoc))
            {

            }
        }

        // check again to guarantee that no errors occur if 
        if (File.Exists(FileLoc))
        {
            using (StreamWriter writer = new StreamWriter(FileLoc))
            {
                string fileContent = UsernameInput.text + "\n" + PasswordInput.text;
            }
        }
    }*/
}
