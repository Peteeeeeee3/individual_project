using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup MainCanvas;
    [SerializeField]
    private CanvasGroup CharactersViewCanvas;
    [SerializeField]
    private UnityEngine.UIElements.ScrollView FiguresScrollView;
    [SerializeField]
    private Transform ContentContainer;
    [SerializeField]
    private GameObject CVSVItemPrefab;
    [SerializeField]
    private GameObject CharacterInfoArea;
    [SerializeField]
    private TextMeshProUGUI CharacterNameText;
    [SerializeField]
    private TextMeshProUGUI LevelText;
    [SerializeField]
    private Slider ExpSlider;
    [SerializeField]
    private TextMeshProUGUI AvailableUpgradesText;
    [SerializeField]
    private TextMeshProUGUI MoveSpeedText;
    [SerializeField]
    private Button MoveSpeedUpgradeButton;
    [SerializeField]
    private TextMeshProUGUI DamageText;
    [SerializeField]
    private Button DamageUpgradeButton;
    [SerializeField]
    private TextMeshProUGUI AttackRateText;
    [SerializeField]
    private Button AttackRateUpgradeButton;
    [SerializeField]
    private TextMeshProUGUI AttackRangeText;
    [SerializeField]
    private Button AttackRangeUpgradeButton;


    private List<Figure> CVFigures = new List<Figure>();
    private List<string> FiguresInfo = new List<string>();
    private bool FigureInfoReady = false;

    // Start is called before the first frame update
    void Start()
    {
        Connection.Subscribe("MAINMENU", this);
    }

    // Update is called once per frame
    void Update()
    {
        if (FigureInfoReady)
        {
            DisplayFigureInfo();
            FigureInfoReady = false;
        }
    }

    /// <summary>
    /// Handle logout request
    /// </summary>
    public void OnLogoutClicked()
    {
        Globals.ACTIVE_USER_ID = "";
        Connection.CloseConnection();
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
        foreach (Transform item in ContentContainer)
        {
            Destroy(item.gameObject);
        }
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
            // do not add first and last line (first is command, last is a blank)
            for (int i = 1;  i < figuresInfo.Length - 1; i++)
            {
                FiguresInfo.Add(figuresInfo[i]);
            }
            FigureInfoReady = true;
        }
    }

    /// <summary>
    /// Displays figures in scroll view
    /// </summary>
    private void DisplayFigureInfo()
    {
        int numAttributes = 8; // number of attributes be figure
        int buttonId = 0;
        for (int i = 0; i < FiguresInfo.Count; i += numAttributes)
        {
            string id = FiguresInfo[i];
            PlayerType type;
            if (FiguresInfo[i + 1].Equals("BLUE"))
            {
                type = PlayerType.BLUE;
            }
            else if (FiguresInfo[i + 1].Equals("GREY"))
            {
                type = PlayerType.GREY;
            }
            else if (FiguresInfo[i + 1].Equals("CREAM"))
            {
                type = PlayerType.CREAM;
            }
            else
            {
                type = PlayerType.ERROR;
            }

            Int32.TryParse(FiguresInfo[i + 2], out int level);
            Int32.TryParse(FiguresInfo[i + 3], out int exp);
            float.TryParse(FiguresInfo[i + 4], out float moveSpeed);
            float.TryParse(FiguresInfo[i + 5], out float damage);
            float.TryParse(FiguresInfo[i + 6], out float attackRate);
            float.TryParse(FiguresInfo[i + 7], out float attackRange);
            string figureName = type.ToString() + "-" + id.Substring(15);

            CVFigures.Add(new Figure(id, type, level, exp, moveSpeed, damage, attackRate, attackRange, figureName));
            var item = Instantiate(CVSVItemPrefab);
            item.GetComponentInChildren<TextMeshProUGUI>().text = figureName;
            int temp = buttonId; // needs to be here to avoid listener storing location of buttonId and using incorrect value
            item.GetComponent<Button>().onClick.AddListener(() => OnValueChanged(temp));
            item.transform.SetParent(ContentContainer);
            
            buttonId++;
        }

        // don't let any lingering values interfere with later displaying of the lsit
        FiguresInfo.Clear();
    }

    /// <summary>
    /// Displays characters attributes and upgrade options
    /// </summary>
    public void OnValueChanged(int id)
    {
        Figure selectedFigure = CVFigures[id];

        // display figure state & info
        CharacterNameText.SetText(selectedFigure.name);
        LevelText.SetText("Level: " + selectedFigure.level.ToString());
        if (selectedFigure.level >= 20)
        {
            ExpSlider.maxValue = 0;
        }
        else
        {
            ExpSlider.maxValue = Globals.LEVEL_EXP_REQUIREMENTS[selectedFigure.level];
        }
        ExpSlider.value = selectedFigure.exp;
        MoveSpeedText.SetText("Move Speed: " + selectedFigure.moveSpeed.ToString());
        DamageText.SetText("Attack Damage: " + selectedFigure.damage.ToString());
        AttackRateText.SetText("Attack Speed: " + selectedFigure.attackRate.ToString());
        AttackRangeText.SetText("Explosion Size: " + selectedFigure.attackRange.ToString());
    }

    /// <summary>
    /// Handles the destruction of this MonoBehaviour class
    /// </summary>
    private void OnDestroy()
    {
        Connection.Unsubscribe("MAINMENU");
    }
}
