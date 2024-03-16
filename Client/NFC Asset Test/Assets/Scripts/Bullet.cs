using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float Velocity;
    [SerializeField]
    private Transform BulletTransform;
    [SerializeField]
    private GameObject ExplosionPrefab;
    [SerializeField]
    private float expAttStartSize;
    [SerializeField]
    private float expAttFinalSize;
    [SerializeField]
    private float expAttGrowthRate;
    [SerializeField]
    private float ttl; // Time To Live
    
    public Vector3 moveDir { get; set; }
    public float damage { get; set; }
    public string ownerTag { get; set; }
    public bool isGrenade { get; set; }

    private Transform explosion;
    private float ttlTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ttlTimer += Time.deltaTime;

        BulletTransform.position = BulletTransform.position + moveDir * Velocity * Time.deltaTime;

        if (explosion)
        {
            explosion.localScale = new Vector3(explosion.localScale.x + expAttGrowthRate * Time.deltaTime, 1,
                explosion.localScale.z + expAttGrowthRate * Time.deltaTime);

            if (explosion.localScale.x >= expAttFinalSize || explosion.localScale.z >= expAttFinalSize)
            {
                Destroy(explosion.gameObject);
                Destroy(gameObject);
            }
        }

        if (isGrenade && ttlTimer > ttl && !explosion)
        {
            Explode();
        }
    }

    /// <summary>
    /// Handles grenade explosion
    /// </summary>
    public void Explode()
    {
        explosion = Instantiate(ExplosionPrefab, BulletTransform.position, Quaternion.identity).GetComponent<Transform>();
        explosion.localScale = new Vector3(expAttStartSize, 1, expAttStartSize);
        explosion.GetComponent<RadialAttackAttributes>().damage = damage;

        BulletTransform.position = new Vector3(1000000, 1000000, 1000000);
    }
}
