using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

//Script Joueur selon les consignes de Lora

public class PlayerScript : MonoBehaviour
{  

    public GameObject ampoule;
    public VideoPlayer ecran;

    public Animator spotlight;

    public int count;
   
   private void OnTriggerEnter(Collider other)
   {
    if(other.tag == "Cuisine")
    {
        ampoule.SetActive(true);
    } else if (other.tag == "Salon")
    {
        ecran.enabled = true;
        Debug.Log("Écran Allumé!");
    }else if(other.tag == "Entree")
    {
        spotlight.Play("FadeIn");
    }
   }

    private void OnTriggerExit(Collider other)
   {
    if(other.tag == "Cusine")
    {
        ampoule.SetActive(false);
    } 
    else if(other.tag == "Salon")
    {
        ecran.enabled = false;
    }
    else if(other.tag == "Entree")
    {
        spotlight.Play("FadeOut");
    }
    else if(other.tag == "Food")
    {
        other.gameObject.SetActive(false);
        count++;
        Debug.Log("Collecté");
    }
   }
}
