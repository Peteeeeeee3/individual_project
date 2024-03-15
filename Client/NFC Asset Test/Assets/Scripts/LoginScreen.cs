using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI UsernameInput;
    [SerializeField]
    private TextMeshProUGUI PasswordInput;
    [SerializeField]
    private Button LoginButton;
    [SerializeField]
    private Button RegisterButton;

    private bool changeScene;

    // Start is called before the first frame update
    void Start()
    {
        bool connSuccess = false;
        changeScene = false;
        while (!connSuccess)
        {
            Connection.Connect(out connSuccess);
        }
        Connection.Subscribe("LOGINSCREEN", this);
    }

    // Update is called once per frame
    void Update()
    {
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
        if (UsernameInput.text.Length > 0 && PasswordInput.text.Length > 0)
        {
            // validate user
            string message = "VALIDATE USER" + "\n" + UsernameInput.text + "\n" + PasswordInput.text;
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
