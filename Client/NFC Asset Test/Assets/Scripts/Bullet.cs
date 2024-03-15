using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float Velocity;
    [SerializeField]
    private Transform BulletTransform;
    
    public Vector3 moveDir { get; set; }
    public float damage { get; set; }
    public string ownerTag { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BulletTransform.position = BulletTransform.position + moveDir * Velocity * Time.deltaTime;
    }
}
