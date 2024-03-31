using distriqt.plugins.nfc;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Dialogs;
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
    private CanvasGroup RegisterFigureCanvas;
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
    private int CurrentlyDisplayedFigureId = -1;
    private bool FigureRegistered = false;

    // Start is called before the first frame update
    void Start()
    {
        Connection.Subscribe("MAINMENU", this);

        CharacterInfoArea.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (FigureInfoReady)
        {
            SetupFiguresListItems();
            FigureInfoReady = false;
        }

        if (FigureRegistered)
        {
            uDialog.NewDialog().
                SetContentText("The figure has successfully been added to your account!").
                AddButton("Close", (dialog) => dialog.Close());

            FigureRegistered = false;
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
    /// Handles character registeration screen
    /// </summary>
    public void OnRegisterFigureClicked()
    {
        MainCanvas.alpha = 0;
        MainCanvas.interactable = false;
        MainCanvas.blocksRaycasts = false;

        RegisterFigureCanvas.alpha = 1;
        RegisterFigureCanvas.interactable = true;
        RegisterFigureCanvas.blocksRaycasts = true;

        gameObject.AddComponent<RegisterFigureNFCHandler>();
    }

    /// <summary>
    /// Handles request on back button on Characters View Canvas
    /// </summary>
    public void OnCVCBackButtonClicked()
    {
        CharacterInfoArea.SetActive(false);

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
    /// Handles request on back button on Register Figure Canvas
    /// </summary>
    public void OnRFBackButtonClicked()
    {
        RegisterFigureCanvas.alpha = 0;
        RegisterFigureCanvas.interactable = false;
        RegisterFigureCanvas.blocksRaycasts = false;

        MainCanvas.alpha = 1;
        MainCanvas.interactable = true;
        MainCanvas.blocksRaycasts = true;

        Destroy(gameObject.GetComponent<RegisterFigureNFCHandler>());
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
    /// Handles successful registration of a figure
    /// </summary>
    public void OnFigureRegistered()
    {
        // do everyhing the backbutton does
        OnRFBackButtonClicked();

        FigureRegistered = true;
    }

    /// <summary>
    /// Displays figures in scroll view
    /// </summary>
    private void SetupFiguresListItems()
    {
        int numAttributes = 9; // number of attributes be figure
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
            Int32.TryParse(FiguresInfo[i + 3], out int availableUpgrades);
            Int32.TryParse(FiguresInfo[i + 4], out int exp);
            float.TryParse(FiguresInfo[i + 5], out float moveSpeed);
            float.TryParse(FiguresInfo[i + 6], out float damage);
            float.TryParse(FiguresInfo[i + 7], out float attackRate);
            float.TryParse(FiguresInfo[i + 8], out float attackRange);
            string figureName = type.ToString() + "-" + id.Substring(15);

            CVFigures.Add(new Figure(id, type, level + 1, availableUpgrades, exp, moveSpeed, damage, attackRate, attackRange, figureName));
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
        CharacterInfoArea.SetActive(true);
        Figure selectedFigure = CVFigures[id];

        // display figure state & info
        CharacterNameText.SetText(selectedFigure.name);
        LevelText.SetText("Level: " + selectedFigure.level.ToString());
        AvailableUpgradesText.SetText("Available Upgrades: " + selectedFigure.availableUpgrades.ToString());
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

        // make buttons inactive if no further updating is possible
        if (selectedFigure.availableUpgrades > 0)
        {
            MoveSpeedUpgradeButton.interactable = true;
            DamageUpgradeButton.interactable = true;
            AttackRateUpgradeButton.interactable = true;
            AttackRangeUpgradeButton.interactable = true;

            if (FindCurrentMoveSpeedUpgrade(selectedFigure) == Globals.MOVE_SPEED_UPGRADE_VALUES.Count - 1)
            {
                MoveSpeedUpgradeButton.interactable = false;
            }

            if (FindCurrentMoveSpeedUpgrade(selectedFigure) == Globals.DAMAGE_UPGRADE_VALUES.Count - 1)
            {
                DamageUpgradeButton.interactable = false;
            }

            if (FindCurrentMoveSpeedUpgrade(selectedFigure) == Globals.ATTACK_RATE_UPGRADE_VALUES.Count - 1)
            {
                AttackRateUpgradeButton.interactable = false;
            }

            if (FindCurrentMoveSpeedUpgrade(selectedFigure) == Globals.ATTACK_RANGE_UPGRADE_VALUES.Count - 1)
            {
                AttackRangeUpgradeButton.interactable = false;
            }
        }
        else
        {
            MoveSpeedUpgradeButton.interactable = false;
            DamageUpgradeButton.interactable = false;
            AttackRateUpgradeButton.interactable = false;
            AttackRangeUpgradeButton.interactable = false;
        }

        // store displayed figure's ID
        CurrentlyDisplayedFigureId = id;
    }

    /// <summary>
    /// Finds the index of the current move speed upgrade level
    /// </summary>
    /// <param name="figure">Figure to check upgrade level for</param>
    /// <returns>Index of upgrade level</returns>
    private int FindCurrentMoveSpeedUpgrade(Figure figure)
    {
        for (int i = 0; i < Globals.MOVE_SPEED_UPGRADE_VALUES.Count; i++)
        {
            if (figure.moveSpeed == Globals.MOVE_SPEED_UPGRADE_VALUES[i])
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Finds the index of the current damage upgrade level
    /// </summary>
    /// <param name="figure">Figure to check upgrade level for</param>
    /// <returns>Index of upgrade level</returns>
    private int FindCurrentDamageUpgrade(Figure figure)
    {
        for (int i = 0; i < Globals.DAMAGE_UPGRADE_VALUES[figure.type].Count; i++)
        {
            if (figure.damage == Globals.DAMAGE_UPGRADE_VALUES[figure.type][i])
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Finds the index of the current attack rate upgrade level
    /// </summary>
    /// <param name="figure">Figure to check upgrade level for</param>
    /// <returns>Index of upgrade level</returns>
    private int FindCurrentAttackRateUpgrade(Figure figure)
    {
        for (int i = 0; i < Globals.ATTACK_RATE_UPGRADE_VALUES.Count; i++)
        {
            if (figure.moveSpeed == Globals.ATTACK_RATE_UPGRADE_VALUES[i])
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Finds the index of the current move speed upgrade level
    /// </summary>
    /// <param name="figure">Figure to check upgrade level for</param>
    /// <returns>Index of upgrade level</returns>
    private int FindCurrentAttackRangeUpgrade(Figure figure)
    {
        for (int i = 0; i < Globals.ATTACK_RANGE_UPGRADE_VALUES.Count; i++)
        {
            if (figure.moveSpeed == Globals.ATTACK_RANGE_UPGRADE_VALUES[i])
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Updates figure's move speed stat
    /// </summary>
    public void UpdateMoveSpeed()
    {
        Figure figure = CVFigures[CurrentlyDisplayedFigureId];
        int figureStatLevelId = FindCurrentMoveSpeedUpgrade(figure);
        if (figureStatLevelId != -1)
        {
            figure.moveSpeed = Globals.MOVE_SPEED_UPGRADE_VALUES[figureStatLevelId + 1];

            string message = "UPDATE STAT\n" +
                Globals.ACTIVE_USER_ID + "\n" +
                figure.id + "\n" +
                "MOVE SPEED" + "\n" +
                figure.moveSpeed;

            Connection.QueueMessage(message);
        }
    }
    
    /// <summary>
    /// Updates figure's damage stat
    /// </summary>
    public void UpdateDamage()
    {
        Figure figure = CVFigures[CurrentlyDisplayedFigureId];
        int figureStatLevelId = FindCurrentDamageUpgrade(figure);
        if (figureStatLevelId != -1)
        {
            figure.damage = Globals.DAMAGE_UPGRADE_VALUES[figure.type][figureStatLevelId + 1];

            string message = "UPDATE STAT\n" +
                Globals.ACTIVE_USER_ID + "\n" +
                figure.id + "\n" +
                "DAMAGE" + "\n" +
                figure.damage;

            Connection.QueueMessage(message);
        }
    }

    /// <summary>
    /// Updates figure's attack rate stat
    /// </summary>
    public void UpdateAttackRate()
    {
        Figure figure = CVFigures[CurrentlyDisplayedFigureId];
        int figureStatLevelId = FindCurrentAttackRateUpgrade(figure);
        if (figureStatLevelId != -1)
        {
            figure.attackRate = Globals.ATTACK_RATE_UPGRADE_VALUES[figureStatLevelId + 1];

            string message = "UPDATE STAT\n" +
                Globals.ACTIVE_USER_ID + "\n" +
                figure.id + "\n" +
                "ATTACK RATE" + "\n" +
                figure.attackRate;

            Connection.QueueMessage(message);
        }
    }

    /// <summary>
    /// Updates figure's attack range
    /// </summary>
    public void UpdateAttackRange()
    {
        Figure figure = CVFigures[CurrentlyDisplayedFigureId];
        int figureStatLevelId = FindCurrentAttackRangeUpgrade(figure);
        if (figureStatLevelId != -1)
        {
            figure.attackRange = Globals.ATTACK_RANGE_UPGRADE_VALUES[figureStatLevelId + 1];

            string message = "UPDATE STAT\n" +
                Globals.ACTIVE_USER_ID + "\n" +
                figure.id + "\n" +
                "ATTACK RANGE" + "\n" +
                figure.attackRange;

            Connection.QueueMessage(message);
        }
    }

    /// <summary>
    /// Handles the destruction of this MonoBehaviour class
    /// </summary>
    private void OnDestroy()
    {
        Connection.Unsubscribe("MAINMENU");
    }
}
