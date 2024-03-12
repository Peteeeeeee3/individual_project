using System.IO;
using TMPro;
using UnityEngine;
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

    // Start is called before the first frame update
    void Start()
    {
        bool connSuccess = false;
        while (!connSuccess)
        {
            Connection.Connect(out connSuccess);
        }
        Connection.Subscribe("LOGINSCREEN", this);
    }

    // Update is called once per frame
    void Update()
    {

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
