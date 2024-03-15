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
    private EnemyType enemyType;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform enemyTransform;
    [SerializeField]
    private float activityRange;
    [SerializeField]
    private float distanceToKeep;
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float damageDealt;
    [SerializeField]
    private float health;

    private List<Vector3> path = new List<Vector3>();
    private float radialAttackImmunityTimer = 0;
    private bool isTimerCounting = false;
    private float radialAttackImmunityDuration = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdateMovement();

        switch (enemyType)
        {
            case EnemyType.GREEN:
                OnUpdateGreen();
                break;
            case EnemyType.RED:
                OnUpdateRed();
                break;
            case EnemyType.VIOLET:
                OnUpdateViolet();
                break;
            default:
                Debug.Log("Unsupported enemy type.");
                break;
        }

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }

        if (isTimerCounting)
        {
            radialAttackImmunityTimer += Time.deltaTime;
            
            if (radialAttackImmunityTimer > radialAttackImmunityDuration)
            {
                radialAttackImmunityTimer = 0;
                isTimerCounting = false;
            }
        }
    }

    /// <summary>
    /// Updates green enemy
    /// </summary>
    private void OnUpdateGreen()
    {

    }

    /// <summary>
    /// Updates red enemy
    /// </summary>
    private void OnUpdateRed()
    {

    }

    /// <summary>
    /// Updates violet enemy
    /// </summary>
    private void OnUpdateViolet()
    {

    }

    /// <summary>
    /// Updates enemy's movement
    /// </summary>
    private void OnUpdateMovement()
    {
        var distance = GetDistance(player.GetComponent<PlayerController>().activePlayerModel.position, enemyTransform.position);
        if (distance < activityRange) 
        {
            var moveVec = player.GetComponent<PlayerController>().activePlayerModel.position - enemyTransform.position;
            moveVec.y = 0;
            moveVec.Normalize();

            if (distance < distanceToKeep)
            {
                enemyTransform.GetComponent<CharacterController>().Move(-moveVec * movementSpeed * Time.deltaTime);
            }
            else
            {
                enemyTransform.GetComponent<CharacterController>().Move(moveVec * movementSpeed * Time.deltaTime);
            }
        }

        if (path.Count > 0)
        {
            transform.position = path[0];
            path.RemoveAt(0);
        }
    }

    /// <summary>
    /// Handles when colliding with special object
    /// </summary>
    /// <param name="other">Object colliding with</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            // deal damage
        }
        else if (other.gameObject.tag.Equals("Bullet"))
        {
            if (other.GetComponent<Bullet>().ownerTag.Equals("Player"))
            {
                health -= other.GetComponent<Bullet>().damage;
                Destroy(other.gameObject);
            }
        }
        else if (other.gameObject.tag.Equals("Radial Attack"))
        {
            health -= other.GetComponent<RadialAttackAttributes>().damage;
            isTimerCounting = true;
        }
    }

    /// <summary>
    /// Gets the distance between two vectors
    /// </summary>
    /// <param name="A">Start</param>
    /// <param name="B">End</param>
    /// <returns>Distance</returns>
    private float GetDistance(Vector3 A, Vector3 B)
    {
        return (A - B).magnitude;
    }
}
