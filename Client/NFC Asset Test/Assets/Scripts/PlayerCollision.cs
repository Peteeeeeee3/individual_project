using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Handles collision with bullets
    /// </summary>
    /// <param name="other">Colliding object</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.ownerTag.Equals("Enemy") && !bullet.isGrenade)
            {
                player.TakeDamage(bullet.damage);
                Destroy(bullet.gameObject);
            }
        }
    }
}
