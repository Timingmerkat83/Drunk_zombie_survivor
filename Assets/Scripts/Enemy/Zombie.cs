using System;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private enum State { Idle, Chasing, Attacking, Dead, Patrolling }
    private State currentState;

    private NavMeshAgent agent;
    public float health = 100f;
    private Animator anim;
    private Transform player;
    private PlayerHealth playerHealth;

    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public float attackDamageMin = 5f;
    public float attackDamageMax = 15f;
    public GameObject[] dismemberedParts;
    public GameObject bloodEffectPrefab;

    public event Action OnDeath;

    public AudioClip groanSound; // Zombie groan sound
    public AudioClip attackSound; // Attack sound
    public AudioClip deathSound; // Death sound
    private AudioSource audioSource;

    private Vector3 patrolPoint;
    private float patrolTimer;
    public float patrolDuration = 5f;

    private float attackCooldown = 1.5f; // Cooldown between attacks
    private float lastAttackTime;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        GetReferences();
        currentState = State.Patrolling; // Start in patrolling state
        SetNewPatrolPoint();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (currentState == State.Dead) return; // Prevent further processing if dead

        switch (currentState)
        {
            case State.Idle:
                CheckForPlayer();
                break;
            case State.Patrolling:
                Patrol();
                CheckForPlayer();
                break;
            case State.Chasing:
                MoveToPlayer();
                break;
            case State.Attacking:
                AttackPlayer();
                break;
        }
    }

    private void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= detectionRadius)
        {
            currentState = State.Chasing;
            PlaySound(groanSound);
        }
    }

    private void MoveToPlayer()
    {
        agent.SetDestination(player.position);
        anim.SetFloat("Speed", agent.velocity.magnitude);

        if (Vector3.Distance(player.position, transform.position) <= agent.stoppingDistance)
        {
            currentState = State.Attacking;
            anim.SetFloat("Speed", 0f);
        }
    }

    private void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolPoint) < 1f)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolDuration)
            {
                SetNewPatrolPoint();
                patrolTimer = 0f;
            }
        }
        else
        {
            agent.SetDestination(patrolPoint);
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void SetNewPatrolPoint()
    {
        patrolPoint = new Vector3(
            transform.position.x + UnityEngine.Random.Range(-5f, 5f),
            transform.position.y,
            transform.position.z + UnityEngine.Random.Range(-5f, 5f)
        );
    }

    private void GetReferences()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        ShowBloodEffect();
        PlaySound(attackSound); // Play damage sound
        anim.SetTrigger("Hit"); // Trigger hit animation

        if (health <= 0)
        {
            Die();
        }
        else if (damage >= 20f)
        {
            Dismember();
        }
    }

    private void Die()
    {
        currentState = State.Dead;
        PlaySound(deathSound); // Play death sound
        anim.SetTrigger("Die");
        GetComponent<Collider>().enabled = false;
        agent.isStopped = true; // Stop the NavMeshAgent
        agent.velocity = Vector3.zero; // Ensure velocity is zero
        OnDeath?.Invoke();
        Destroy(gameObject, 2f);
    }

    private void ShowBloodEffect()
    {
        if (bloodEffectPrefab != null)
        {
            GameObject bloodEffect = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            Destroy(bloodEffect, 2f);
        }
        else
        {
            Debug.LogWarning("Blood effect prefab not assigned.");
        }
    }

    private void Dismember()
    {
        int partIndex = UnityEngine.Random.Range(0, dismemberedParts.Length);
        GameObject dismemberedPart = Instantiate(dismemberedParts[partIndex], transform.position, Quaternion.identity);

        Rigidbody rb = dismemberedPart.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddExplosionForce(500f, transform.position, 5f);
        }

        GetComponent<Collider>().enabled = false;
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }

    private void AttackPlayer()
    {
        if (Vector3.Distance(player.position, transform.position) <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                float damage = UnityEngine.Random.Range(attackDamageMin, attackDamageMax);
                playerHealth.TakeDamage(damage);
                anim.SetTrigger("Attack");
                lastAttackTime = Time.time; // Reset the attack cooldown
            }
        }
        else
        {
            currentState = State.Chasing;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
