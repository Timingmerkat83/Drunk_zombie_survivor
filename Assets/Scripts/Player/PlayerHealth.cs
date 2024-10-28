using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{   
    [Header("Health Bar")]
    public float currentHealth; // Current health of the player
    private float lerpTimer;
    public float maxHealth = 100f; // Maximum health of the player
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;

    [Header("Damage overlay")]
    public Image overlay; // Damage overlay gameobject
    public float duration;
    public float fadeSpeed;
    private float durationTimer;

    public AudioClip damageSound; // Sound played when taking damage
    public AudioClip healSound; // Sound played when healing
    public float invincibilityDuration = 1f; // Time the player is invincible after dying
    public float healthRegenerationRate = 1f; // Health per second
    public Camera mainCamera; // Reference to the main camera
    public Image healthBarFill; // Reference to the fill image of the health bar
    public float lowHealthThreshold = 30f; // Health threshold for low health effects

    private bool isDead = false;
    private bool isInvincible = false;
    private AudioSource audioSource;
    private Coroutine healthRegenerationCoroutine;
    private bool lowHealthWarningShown = false;

    public GameOverManager gameOverManager; // Reference to the Game Over manager

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health
        UpdateHealthBar();
        audioSource = GetComponent<AudioSource>();
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        lerpTimer = 0f;
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0
        UpdateHealthBar();

        PlaySound(damageSound);

        if (currentHealth <= lowHealthThreshold && !lowHealthWarningShown)
        {
            lowHealthWarningShown = true;
            Debug.Log("Warning: Low Health!");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityAfterDamage()); // Start invincibility coroutine
        }
    }

    private IEnumerator InvincibilityAfterDamage()
    {
        isInvincible = true; // Set invincible state
        yield return new WaitForSeconds(invincibilityDuration); // Wait for invincibility duration
        isInvincible = false; // Reset invincibility state
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't exceed max health
        UpdateHealthBar();

        PlaySound(healSound);
    }

    private IEnumerator HealthRegeneration()
    {
        while (currentHealth < maxHealth)
        {
            Heal(healthRegenerationRate * Time.deltaTime);
            yield return null; // Wait for the next frame
        }
    }

    void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        if (overlay.color.a > 0)
        {
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    void UpdateHealthBar()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = currentHealth / maxHealth;
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player has died!");
        gameOverManager.ShowGameOver(); // Show Game Over screen
        StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void StartHealthRegeneration()
    {
        if (healthRegenerationCoroutine == null)
        {
            healthRegenerationCoroutine = StartCoroutine(HealthRegeneration());
        }
    }

    public void StopHealthRegeneration()
    {
        if (healthRegenerationCoroutine != null)
        {
            StopCoroutine(healthRegenerationCoroutine);
            healthRegenerationCoroutine = null;
        }
    }

    public void Revive()
    {
        if (isDead)
        {
            currentHealth = maxHealth / 2; // Revive with half health
            isDead = false;
            UpdateHealthBar();
            StartHealthRegeneration(); // Start health regeneration on revive
        }
    }
}
