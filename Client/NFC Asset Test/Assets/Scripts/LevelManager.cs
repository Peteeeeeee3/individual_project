using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private List<Enemy> enemies = new List<Enemy>();
    [SerializeField]
    private Material outline;
    [SerializeField]
    private int highlightedEnemyCount;
    [SerializeField]
    private TextMeshProUGUI DebugText4;

    private List<Enemy> highlightedEnemies = new List<Enemy>();

    // Start is called before the first frame update
    void Start()
    {
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
        DebugText4.SetText("Required Enemies Left: " + highlightedEnemies.Count + "       Enemies Left: " + enemies.Count);
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
}
