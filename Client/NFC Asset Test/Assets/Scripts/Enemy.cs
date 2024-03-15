using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    GREEN,
    RED,
    VIOLET,
    ERROR
}

public class Enemy : MonoBehaviour
{
    [SerializeField]
    EnemyType enemyType;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyType)
        {
            case EnemyType.GREEN:
                break;
            case EnemyType.RED:
                break;
            case EnemyType.VIOLET:
                break;
            default:
                Debug.Log("Unsupported enemy type.");
                break;
        }
    }

    /// <summary>
    /// Updates Green Enemy
    /// </summary>
    private void onUpdateGreen()
    {

    }

    /// <summary>
    /// Updates Red Enemy
    /// </summary>
    private void onUpdateRed()
    {

    }

    /// <summary>
    /// Updates Violet Enemy
    /// </summary>
    private void onUpdateViolet()
    {

    }
}
