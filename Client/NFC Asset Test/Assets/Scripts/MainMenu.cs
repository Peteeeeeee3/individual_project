using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup MainCanvas;
    [SerializeField]
    private CanvasGroup CharactersViewCanvas;
    [SerializeField]
    private ScrollView FiguresScrollView;

    private List<Figure> CVFigures = new List<Figure>();

    // Start is called before the first frame update
    void Start()
    {
        Connection.Subscribe("MAINMENU", this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Handle logout request
    /// </summary>
    public void OnLogoutClicked()
    {
        Globals.ACTIVE_USER_ID = "";
        SceneManager.LoadScene("Login");
    }

    /// <summary>
    /// Handles play request
    /// </summary>
    public void OnPlayClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// Handles characters view screen
    /// </summary>
    public void OnViewCharactersClicked()
    {
        MainCanvas.alpha = 0;
        MainCanvas.interactable = false;
        MainCanvas.blocksRaycasts = false;

        CharactersViewCanvas.alpha = 1;
        CharactersViewCanvas.interactable = true;
        CharactersViewCanvas.blocksRaycasts = true;

        // request info of all figures related to user
        string message = "GET FIGURES\n" + Globals.ACTIVE_USER_ID;
        Connection.QueueMessage(message);
    }

    /// <summary>
    /// Handles request on back button on Characters View Canvas
    /// </summary>
    public void OnCVCBackButtonClicked()
    {
        CharactersViewCanvas.alpha = 0;
        CharactersViewCanvas.interactable = false;
        CharactersViewCanvas.blocksRaycasts = false;

        MainCanvas.alpha = 1;
        MainCanvas.interactable = true;
        MainCanvas.blocksRaycasts = true;

        CVFigures.Clear(); // do not premanently store figures
    }

    /// <summary>
    /// Handles server response to info of all figures
    /// </summary>
    /// <param name="figuresInfo">Info about all of the account's figures</param>
    public void OnAllFiguresReceived(string[] figuresInfo)
    {
        if (figuresInfo[0].Equals("ERROR FIGURES"))
        {
            // toggle popup
        }
        else
        {
            int numAttributes = 2; // number of attributes be figure
            for (int i = 1; i < figuresInfo.Length; i += numAttributes)
            {
                string id = figuresInfo[i + 1];
                PlayerType type;
                if (figuresInfo[i + 2].Equals("BLUE"))
                {
                    type = PlayerType.BLUE;
                }
                else if (figuresInfo[i + 2].Equals("GREY"))
                {
                    type = PlayerType.GREY;
                }
                else if (figuresInfo[i + 2].Equals("CREAM"))
                {
                    type = PlayerType.CREAM;
                }
                else
                {
                    type = PlayerType.ERROR;
                }

                CVFigures.Add(new Figure(id, type));
            }

            FiguresScrollView.Add(new VisualElement());
        }
    }
}
