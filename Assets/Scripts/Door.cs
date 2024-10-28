using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Door : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;
    public KeyCode openKey = KeyCode.E;
    private Transform player;


    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(openKey) && IsPlayerInRange())
        {
            ToggleDoor();
        }
    }

    private bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) < 2.0f;
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }
}
