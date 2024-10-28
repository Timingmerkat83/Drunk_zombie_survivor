using System;
using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    public float health = 100f;
    public AudioClip deathSound; // Death sound

    private Animator anim;
    private AudioSource audioSource;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        anim.SetBool("TakeDamage", true);
        Debug.Log("Zombie took " + amount + " damage. Health left: " + health);

        if (health <= 0f)
        {
            Die();
        }

        StartCoroutine(ResetDamageState());
    }

    private IEnumerator ResetDamageState()
    {
        yield return new WaitForSeconds(0.3f); // Adjust delay as needed
        anim.SetBool("TakeDamage", false); // Reset damage state
    }

    void Die()
    {
        anim.SetTrigger("Die");
        PlaySound(deathSound); // Play death sound
        GetComponent<Collider>().enabled = false;
        agent.isStopped = true; // Stop the NavMeshAgent
        agent.velocity = Vector3.zero; // Ensure velocity is zero
        Destroy(gameObject, 1.09f);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
