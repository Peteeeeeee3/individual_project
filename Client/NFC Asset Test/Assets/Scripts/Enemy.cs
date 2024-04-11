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
    [SerializeField]
    private float attackFrequency;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    public int expValue;

    public LevelManager levelManager { get; set; }
    private float radialAttackImmunityTimer = 0;
    private bool isImmunityTimerCounting = false;
    private float radialAttackImmunityDuration = 1;
    private float attackTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdateMovement();

        attackTimer += Time.deltaTime;

        if (player.GetComponent<PlayerController>().activePlayerModel != null)
        {
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
        }

        if (health <= 0)
        {
            player.GetComponent<PlayerController>().GainExp(expValue);
            if (levelManager)
            {
                levelManager.removeRequiredEnemy(this);
            }
            Destroy(this.gameObject);
        }

        if (isImmunityTimerCounting)
        {
            radialAttackImmunityTimer += Time.deltaTime;
            
            if (radialAttackImmunityTimer > radialAttackImmunityDuration)
            {
                radialAttackImmunityTimer = 0;
                isImmunityTimerCounting = false;
            }
        }

        if (attackTimer >= attackFrequency)
        {
            attackTimer = 0;
        }
    }

    /// <summary>
    /// Updates green enemy
    /// </summary>
    private void OnUpdateGreen()
    {
        if (GetDistance(enemyTransform.position, player.GetComponent<PlayerController>().activePlayerModel.position) <= attackRange && 
            attackTimer >= attackFrequency)
        {
            player.GetComponent<PlayerController>().TakeDamage(damageDealt);
        }
    }

    /// <summary>
    /// Updates red enemy
    /// </summary>
    private void OnUpdateRed()
    {
        if (GetDistance(enemyTransform.position, player.GetComponent<PlayerController>().activePlayerModel.position) <= attackRange && 
            attackTimer >= attackFrequency)
        {
            GameObject bulletGO = Instantiate(bulletPrefab, new Vector3(enemyTransform.position.x, 5, enemyTransform.position.z), Quaternion.identity);
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            var tempMoveDir = (player.GetComponent<PlayerController>().activePlayerModel.position - enemyTransform.position).normalized;
            tempMoveDir.y = 0;
            bullet.moveDir = tempMoveDir;
            bullet.damage = damageDealt;
            bullet.ownerTag = "Enemy";
            bullet.isGrenade = false;
            bulletGO.transform.localScale *= 3;
        }
    }

    /// <summary>
    /// Updates violet enemy
    /// </summary>
    private void OnUpdateViolet()
    {
        if (GetDistance(enemyTransform.position, player.GetComponent<PlayerController>().activePlayerModel.position) <= attackRange &&
            attackTimer >= attackFrequency)
        {
            int radius = 15;
            int numBullets = 16;
            float angleInterval = 360 / numBullets;
            float angle = 0;

            for (int i = 0; i < numBullets; i++)
            {
                float x = enemyTransform.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float z = enemyTransform.position.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

                Vector3 bulletPos = new Vector3(x, 5, z); // as always, height 5
                GameObject bulletGO = Instantiate(bulletPrefab, bulletPos, Quaternion.identity);
                Bullet bullet = bulletGO.GetComponent<Bullet>();
                bullet.moveDir = (bulletPos - new Vector3(enemyTransform.position.x, 5, enemyTransform.position.z)).normalized;
                bullet.damage = damageDealt;
                bullet.ownerTag = "Enemy";
                bullet.isGrenade = false;

                angle += angleInterval;
            }
        }
    }

    /// <summary>
    /// Updates enemy's movement
    /// </summary>
    private void OnUpdateMovement()
    {
        Transform activePlayerModel = player.GetComponent<PlayerController>().activePlayerModel;
        if (activePlayerModel)
            {
            var distance = GetDistance(activePlayerModel.position, enemyTransform.position);
            if (distance < activityRange)
            {
                var moveVec = activePlayerModel.position - enemyTransform.position;
                moveVec.y = 0;
                moveVec.Normalize();

                if (distance < distanceToKeep)
                {
                    enemyTransform.GetComponent<CharacterController>().Move(-moveVec * movementSpeed * Time.deltaTime);
                }
                else /*if (GetDistance(enemyTransform.position, enemyTransform.position + moveVec * movementSpeed * Time.deltaTime) > distanceToKeep)*/
                {
                    enemyTransform.GetComponent<CharacterController>().Move(moveVec * movementSpeed * Time.deltaTime);
                }

                if (enemyTransform.position.y > 0)
                {
                    enemyTransform.position = new Vector3(enemyTransform.position.x, 0, enemyTransform.position.z);
                }
            }
        }
    }

    /// <summary>
    /// Handles when colliding with special object
    /// </summary>
    /// <param name="other">Object colliding with</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.ownerTag.Equals("Player"))
            {
                if (!bullet.isGrenade)
                {
                    health -= other.GetComponent<Bullet>().damage;
                    Destroy(other.gameObject);
                }
                else
                {
                    bullet.Explode();
                }
            }
        }
        else if (other.gameObject.tag.Equals("Radial Attack"))
        {
            health -= other.GetComponent<RadialAttackAttributes>().damage;
            isImmunityTimerCounting = true;
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

    /// <summary>
    /// Returns enemy type
    /// </summary>
    /// <returns>Enemy type</returns>
    public EnemyType GetEnemyType()
    {
        return enemyType;
    }
}
