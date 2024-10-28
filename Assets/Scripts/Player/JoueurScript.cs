using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using TMPro;

public class JoueurScript : MonoBehaviour
{  

    public GameObject ampoule;
    public VideoPlayer ecran;

    public Animator spotlight;

    public int count;
    [SerializeField] private AudioSource beerCollected;

    public TextMeshProUGUI pointage;

    public GameObject bravo;
    public GameObject bouttonRecommencer;
    public GameObject bouttonQuitter;

    public GameObject beer;
   
   private void OnTriggerEnter(Collider other)
   {
    if(other.tag == "Cuisine")
    {
        ampoule.SetActive(true);
    } else if (other.tag == "Salon")
    {
        ecran.enabled = true;
    }   else if (other.tag == "Entree")
    {
        spotlight.Play("FadeIn");
        Debug.Log("Lumiere Allumé!");
    }
    else if(other.tag == "Food")
    {
        other.gameObject.SetActive(false);
        count++;
        pointage.text = count.ToString();
        Debug.Log("Collecté");
        if(count >=  5)
        {
            StartCoroutine(Reussite());
        }
        if(beerCollected != null)
        {
        beerCollected.Play();
        }
    }
   }

    private void OnTriggerExit(Collider other)
   {
    if(other.tag == "Cusine")
    {
        ampoule.SetActive(false);
    } else if(other.tag == "Salon")
    {
        ecran.enabled = false;
    }
    else if (other.tag == "Entree")
    {
        spotlight.Play("FadeOut");
        Debug.Log("Lumiere Éteinte!!");
    } 
   }

   private IEnumerator Reussite()
    {
       bravo.SetActive(true);
       Time.timeScale = 0f;
       yield return new WaitForSeconds(5f);
       bouttonRecommencer.SetActive(true);
       bouttonQuitter.SetActive(true);
       Cursor.lockState = CursorLockMode.None;

   }

   public void miseAZero()
   {
       bravo.SetActive(false);
       bouttonRecommencer.SetActive(false);
       count = 0;
       pointage.text = count.ToString();

       for(int i=0; i < beer.transform.childCount; i++)
       {
           beer.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
       }
   }
}
