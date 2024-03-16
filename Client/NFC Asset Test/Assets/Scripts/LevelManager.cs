using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private List<Enemy> enemies = new List<Enemy>();
    [SerializeField]
    private Material outline;
    [SerializeField]
    private int highlightedEnemyCount;

    private List<Enemy> highlightedEnemies = new List<Enemy>();

    // Start is called before the first frame update
    void Start()
    {
        System.Random random = new System.Random();
         
        for (int i = 0; i < highlightedEnemyCount; i++)
        {
            int pos = random.Next(0, enemies.Count - 1);

            while (!enemies[pos])
            {
                pos = random.Next(0, enemies.Count - 1);
            }    

            Enemy enemy = enemies[pos];
            AddHighlight(enemy);
            highlightedEnemies.Add(enemy);

            if (enemies.Count > 1)
            {
                enemies.Remove(enemy);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
                meshRenderer.material.color = new Color(0, 255, 83, 255);
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
}
