using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Assign your zombie prefab in the inspector
    public int initialZombieCount = 5; // Initial number of zombies to spawn
    public float minSpawnInterval = 1f; // Minimum time between spawns
    public float maxSpawnInterval = 3f; // Maximum time between spawns
    public int maxZombies = 20; // Maximum number of zombies that can be active
    public Transform[] spawnPoints; // Array of spawn points
    public float fadeDuration = 1f; // Duration of the fade-in effect
    public float spawnHeightOffset = 1f; // Vertical offset for spawn height
    public AudioClip[] spawnAudioClips; // Array for different spawn sounds

    private int currentZombieCount = 0;

    private void Start()
    {
        if (zombiePrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Zombie prefab or spawn points not set.");
            return;
        }

        // Spawn initial zombies
        for (int i = 0; i < initialZombieCount; i++)
        {
            SpawnZombie();
        }

        // Start the coroutine for continuous spawning
        StartCoroutine(SpawnZombiesContinuously());
    }

    private IEnumerator SpawnZombiesContinuously()
    {
        while (true)
        {
            if (currentZombieCount < maxZombies)
            {
                float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
                yield return new WaitForSeconds(spawnInterval);
                SpawnZombie();
            }
            else
            {
                yield return null; // Wait one frame if max zombies are active
            }
        }
    }

    private void SpawnZombie()
    {
        // Randomly select a spawn point from the array
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Generate a spawn position using the selected spawn point
        Vector3 spawnPosition = randomSpawnPoint.position + new Vector3(0, Random.Range(-spawnHeightOffset, spawnHeightOffset), 0);
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, randomRotation);
        currentZombieCount++;

        Zombie zombieScript = zombie.GetComponent<Zombie>();
        if (zombieScript != null)
        {
            zombieScript.OnDeath += ZombieDied; // Subscribe to the death event
        }
        else
        {
            Debug.LogWarning("Zombie prefab does not have a Zombie component.");
        }

        // Start fade-in effect
        StartCoroutine(FadeInZombie(zombie));

        // Play random spawn audio
        PlaySpawnSound();
    }

    private IEnumerator FadeInZombie(GameObject zombie)
    {
        Renderer renderer = zombie.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color color = renderer.material.color;
            color.a = 0; // Start fully transparent
            renderer.material.color = color;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                color.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                renderer.material.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            color.a = 1; // Ensure it ends fully opaque
            renderer.material.color = color;
        }
        else
        {
            Debug.LogWarning("Zombie prefab does not have a Renderer component.");
        }
    }

    private void ZombieDied()
    {
        currentZombieCount--;
        // Additional cleanup can be added here if needed
    }

    private void PlaySpawnSound()
    {
        if (spawnAudioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnAudioClips.Length);
            AudioSource.PlayClipAtPoint(spawnAudioClips[randomIndex], transform.position);
        }
    }
}
