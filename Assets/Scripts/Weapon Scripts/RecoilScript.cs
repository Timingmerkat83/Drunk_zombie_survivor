using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilScript : MonoBehaviour
{
    public float recoilAmount = 1.0f; // Amount of recoil
    public float recoilSpeed = 5.0f; // Speed of recoil return
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
        targetPosition = originalPosition;
    }

    void Update()
    {
        // Smoothly return to the original position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, recoilSpeed * Time.deltaTime);

    }

    public void Fire()
    {
        // Apply recoil when firing
        targetPosition = originalPosition - new Vector3(0, recoilAmount, 0);
        // Here you can also add a little side-to-side or up-and-down recoil for more realism
        // For example:
        // targetPosition += new Vector3(Random.Range(-0.1f, 0.1f), 0, 0);
    }
}