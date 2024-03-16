using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Handles when a triggering object enters collision area
    /// </summary>
    /// <param name="other">Object this instance is colliding with</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Bullet"))
        {
            if (!other.GetComponent<Bullet>().isGrenade)
            {
                Destroy(other.gameObject);
            }
            else
            {
                other.GetComponent<Bullet>().Explode();
            }
        }
    }
}
