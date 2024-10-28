using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 25f; // Damage dealt by the bullet
    public float speed = 20f;   // Speed of the bullet
    public float lifetime = 2f; // Lifetime before the bullet is destroyed
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        // Destroy the bullet after a set time
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a Target component
        Target target = collision.transform.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage); // Deal damage to the target
        }

        // Destroy the bullet upon collision
        Destroy(gameObject);
    }
}
