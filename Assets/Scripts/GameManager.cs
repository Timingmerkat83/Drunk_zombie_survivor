using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public Animator weaponAnimator;

    void Start()
    {
        StartCoroutine(CockWeapon());
    }

    private IEnumerator CockWeapon()
    {
        // Trigger the cocking animation
        weaponAnimator.SetTrigger("Cock"); // Ensure you have a trigger parameter named "Cock"

        // Wait for the animation to finish (adjust the time according to your animation length)
        yield return new WaitForSeconds(1.0f); // Replace with actual duration if needed

        // Optionally, proceed to other game initialization logic here
    }
}
