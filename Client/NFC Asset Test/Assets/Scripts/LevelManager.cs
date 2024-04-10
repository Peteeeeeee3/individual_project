using System;
using System.Collections.Generic;
using TMPro;
using UI.Dialogs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private List<Enemy> enemies = new List<Enemy>();
    [SerializeField]
    private Material outline;
    [SerializeField]
    private int highlightedEnemyCount;
    [SerializeField]
    private TextMeshProUGUI EnemyCountText;
    [SerializeField]
    private GameObject exitPortalPrefab;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private uDialog menuDialog;

    private string ConnectionIdString = "LEVELMANAGER";
    private List<Enemy> highlightedEnemies = new List<Enemy>();
    private GameObject exitPortal;
    private bool messageSent = false;
    private bool levelComplete = false;
    private bool playerDied = false;

    // Start is called before the first frame update
    void Start()
    {
        Connection.Subscribe(ConnectionIdString, this);

        // add this instance to all enemies
        foreach (var enemy in enemies)
        {
            enemy.levelManager = this;
        }

        // highlight random enemies
        System.Random random = new System.Random();
         
        for (int i = 0; i < highlightedEnemyCount; i++)
        {
            int pos = random.Next(0, enemies.Count - 1);

            Enemy enemy = enemies[pos];
            AddHighlight(enemy);
            highlightedEnemies.Add(enemy);
            enemies.Remove(enemy);
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyCountText.SetText("Required Enemies Left: " + highlightedEnemies.Count + "       Total Enemies Left: " + enemies.Count + highlightedEnemies.Count);
    
        if (highlightedEnemies.Count == 0 && !exitPortal)
        {
            exitPortal = Instantiate(exitPortalPrefab);
            exitPortal.transform.position = Vector3.zero;
            exitPortal.transform.localScale = new Vector3(15, 15, 15);
        }

        // ignore y coordinates when checking distance between player and portal
        if (exitPortal && 
            !messageSent &&
            GetDistance(
                new Vector2(playerController.activePlayerModel.position.x, playerController.activePlayerModel.position.z), 
                new Vector2(exitPortal.transform.position.x, exitPortal.transform.position.z)
                ) <= 10)
        {
            string message = "GET FIGURE\n" +
                ConnectionIdString + "\n" +
                Globals.ACTIVE_USER_ID + "\n" +
                playerController.GetComponent<PlayerController>().activePlayer.id;

            Connection.QueueMessage(message);

            messageSent = true;
        }

        if (levelComplete)
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (playerController.health <= 0 &&
            playerController.activePlayerModel &&
            !playerDied)
        {
            HandlePlayerDeath();
            playerDied = true;
        }
    }

    /// <summary>
    /// Adds highlight material to game object
    /// </summary>
    private void AddHighlight(Enemy enemy)
    {
        MeshRenderer meshRenderer = enemy.GetComponentInChildren<MeshRenderer>();
        
        meshRenderer.material = outline;

        switch (enemy.GetEnemyType())
        {
            case EnemyType.GREEN:
                meshRenderer.material.color = new Color(0, 255, 0, 255);
                break;
            case EnemyType.RED:
                meshRenderer.material.color = new Color(255, 0, 0, 255);
                break;
            case EnemyType.VIOLET:
                meshRenderer.material.color = new Color(143, 0, 255, 255);
                break;
            default:
                Debug.Log("Error enemy type");
                return;
        }
    }

    /// <summary>
    /// Removes enemy from lists
    /// </summary>
    public void removeRequiredEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy)) { enemies.Remove(enemy); } 
        if (highlightedEnemies.Contains(enemy)) { highlightedEnemies.Remove(enemy); }
    }

    /// <summary>
    /// Handles the completion of the game scene
    /// </summary>
    public void OnCompleteLevel(string[] figureData, int dataOffset)
    {
        int level = Int32.Parse(figureData[dataOffset + 2]);
        int exp = Int32.Parse(figureData[dataOffset + 3]);

        exp += playerController.expCollected;
        
        while (exp >= Globals.LEVEL_EXP_REQUIREMENTS[level])
        {
            exp -= Globals.LEVEL_EXP_REQUIREMENTS[level++];
        }

        string message = "UPDATE FIGURE\n" +
            Globals.ACTIVE_USER_ID + "\n" +
            figureData[dataOffset] + "\n" +
            figureData[dataOffset + 1] + "\n" +
            level.ToString() + "\n" +
            exp.ToString() + "\n" +
            figureData[dataOffset + 4] + "\n" +
            figureData[dataOffset + 5] + "\n" +
            figureData[dataOffset + 6] + "\n" +
            figureData[dataOffset + 7];

        Connection.QueueMessage(message);

        levelComplete = true;
    }

    /// <summary>
    /// Handles when the player's health reaches zero or below
    /// </summary>
    private void HandlePlayerDeath()
    {
        Time.timeScale = 0;

        uDialog.NewDialog().
                SetContentText("You died...").
                AddButton(":(", (dialog) => {
                    dialog.Close();
                    Time.timeScale = 1;
                    SceneManager.LoadScene("MainMenu");
                    });
    }

    /// <summary>
    /// Handles game pause
    /// </summary>
    public void OnGamePaused()
    {
        menuDialog.Show();
        Time.timeScale = 0;
    }

    /// <summary>
    /// Handles resuming of game
    /// </summary>
    public void OnGameResumed()
    {
        menuDialog.Close();
        Time.timeScale = 1;
    }

    /// <summary>
    /// Handles quitting of game scene
    /// </summary>
    public void OnGameQuit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }    

    /// <summary>
    /// Gets the distance between two vectors
    /// </summary>
    /// <param name="A">Start</param>
    /// <param name="B">End</param>
    /// <returns>Distance</returns>
    private float GetDistance(Vector2 A, Vector2 B)
    {
        return (A - B).magnitude;
    }

    /// <summary>
    /// Handles the destruction of this MonoBehaviour class
    /// </summary>
    private void OnDestroy()
    {
        Connection.Unsubscribe("LEVELMANAGER");
    }
}
