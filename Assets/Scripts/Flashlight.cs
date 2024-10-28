using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject light;
    private bool isLightOn = false;
    private bool showControls = true;
    private float controlDisplayTime = 15f; // Time in seconds
    private float timer;



    void Start()
    {
        timer = controlDisplayTime;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isLightOn = !isLightOn;
            light.SetActive(isLightOn);
        }

        if (showControls)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                showControls = false; // Close the controls window
            }
        }
    }

    void OnGUI()
    {


        if (showControls)
        {
            GUI.Window(0, new Rect(10, 10, 180, 190), ShowControls, "Controls");
        }
    }

    void ShowControls(int windowID)
    {
        GUI.Label(new Rect(10, 20, 160, 20), "F: Toggle Flashlight");
        GUI.Label(new Rect(10, 40, 160, 20), "WASD: Move");
        GUI.Label(new Rect(10, 60, 160, 20), "E: Interact");
        GUI.Label(new Rect(10, 80, 160, 20), "R: Reload");
        GUI.Label(new Rect(10, 100, 160, 20), "Clique Droit: Tirer");
        GUI.Label(new Rect(10, 120, 160, 20), "Clique Gauche: Scope");
        GUI.Label(new Rect(10, 140, 160, 20), "Trouve la bière!!");
        GUI.DragWindow();
    }

   
}