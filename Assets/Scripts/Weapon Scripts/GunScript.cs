using System.Collections;
using UnityEngine;
using TMPro; // Add this for TextMesh Pro

public class GunScript : MonoBehaviour
{

    [Header("Damage")]
    public float damage = 25f;
    public float range = 100f;
    public float impactForce = 30f;
    public float fireRate = 5f;

     [Header("Ammo")]
    public int maxAmmo = 15;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("Bullet")]
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private GameObject bulletPoint;
    [SerializeField]
    private float bulletSpeed = 600;
     [SerializeField]
    private GameObject bloodPS;


 

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextTimeToFire = 0f;

    public Animator animator;

    public GameObject scopeOverlay;
    public GameObject crosshairOverlay;

    private bool isScoped = false;

    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource reloadSound;

    // Reference to the ammo text UI
    [SerializeField] private TMP_Text ammoText; // Change to TMP if using TextMeshPro

     public float zoomedFOV = 30f;
    public float normalFOV = 60f;
    public float zoomSpeed = 5f;

    public enum FireMode { SemiAuto, Burst, Automatic }
    public FireMode fireMode = FireMode.Automatic;
    private int burstCount = 3;


    // Movement and sway settings
    public float swayAmount = 0.1f;
    public float swaySpeed = 2.0f;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoDisplay(); // Update ammo display at start
       
    }

    void Update()
    {
        if (isReloading) return;

        HandleInput();
        HandleZoom();
         HandleSway();

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

         if (fireMode == FireMode.Automatic && Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            FireWeapon();
        }
    }

    void HandleInput()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            FireWeapon();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            ToggleScope();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

void FireWeapon()
{
    if (fireMode == FireMode.Burst)
    {
        StartCoroutine(BurstFire());
    }
    else
    {
        nextTimeToFire = Time.time + 1f / fireRate;
        Shoot();
        

        if (isScoped)
            animator.SetTrigger("RecoilScope");

        
    }
}
    void ToggleScope()
    {
        isScoped = !isScoped;
        animator.SetBool("Scoped", isScoped);

        if (isScoped)
            StartCoroutine(OnScoped());
        else
            OnUnscoped();
    }

    void OnUnscoped()
    {
        scopeOverlay.SetActive(false);
        crosshairOverlay.SetActive(true);
    }

    private IEnumerator BurstFire()
{
    for (int i = 0; i < burstCount; i++)
    {
        if (currentAmmo <= 0) break;

        nextTimeToFire = Time.time + 1f / fireRate;
        Shoot();
        animator.SetTrigger("Recoil");

        if (isScoped)
            animator.SetTrigger("RecoilScope");

        yield return new WaitForSeconds(1f / fireRate);
    }
}

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(.15f);
        scopeOverlay.SetActive(true);
        crosshairOverlay.SetActive(false);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

          if (reloadSound != null)
        {
            reloadSound.Play();
        }


        animator.SetBool("Reload", true);
        yield return new WaitForSeconds(reloadTime - .25f);
        animator.SetBool("Reload", false);
        yield return new WaitForSeconds(.25f);

      
        currentAmmo = maxAmmo;
        UpdateAmmoDisplay(); // Update ammo display after reload
        isReloading = false;
    }

   

    void Shoot()
    {
        currentAmmo--;

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (shootSound != null)
        {
            shootSound.Play();
        }

         // Set the recoil trigger
    animator.SetTrigger("Recoil");

        Debug.Log("Shoot!");
        GameObject bullet = Instantiate(bulletPrefab, bulletPoint.transform.position, transform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        rb.useGravity = true;
        Destroy(bullet, 2f); // Bullet lifetime

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
                
  
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }

        UpdateAmmoDisplay(); // Update ammo display after shooting
    }

       void ShowHitMarker(Vector3 position)
    {
        // Here, you can implement your hitmarker logic (e.g., UI element).
        Debug.Log("Hit at: " + position);
        // You could instantiate a hitmarker prefab here
    }


      void HandleZoom()
    {
        if (isScoped)
        {
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
            crosshairOverlay.transform.localScale = Vector3.one * 0.5f; // Smaller crosshair when scoped
        }
        else
        {
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
            crosshairOverlay.transform.localScale = Vector3.one; // Normal size crosshair
        }
    }

       void HandleSway()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayY = Mathf.Cos(Time.time * swaySpeed) * swayAmount;
        transform.localPosition = new Vector3(swayX, swayY, 0);
    }
  

    void UpdateAmmoDisplay()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + currentAmmo.ToString(); // Update the text with current ammo
        }
    }
}
