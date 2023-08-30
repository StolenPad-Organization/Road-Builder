using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineIconController : MonoBehaviour
{
    [SerializeField] Material material;

    [SerializeField] int fadePropertyID;
    [SerializeField] float fadeValue;
    [SerializeField] bool canFade;
    [SerializeField] GameObject xSign;
    [SerializeField] GameObject cursorSign;

    void Start()
    {
        //Get material reference
        material = GetComponent<SpriteRenderer>().material;

        //Convert property name to id (improves performance).
        //You can see property names by hovering over them in the material inspector.
        fadePropertyID = Shader.PropertyToID("_FullGlowDissolveFade");

        //Set fade value to zero at start.
        fadeValue = 1;
    }

    void Update()
    {
        //Update while fade value is less than 1.
        if (!canFade) return;
        if (fadeValue > 0)
        {
            //Increase fade value over time.
            fadeValue -= Time.deltaTime;
            if (fadeValue <= 0)
            {
                fadeValue = 0;
                gameObject.SetActive(false);
            }


            //Update value in material.
            material.SetFloat(fadePropertyID, fadeValue);
        }
    }

    public void Fade()
    {
        if (canFade) return;
        canFade = true;
        xSign.SetActive(false);
        cursorSign.SetActive(false);
    }

    public void OnSpawn()
    {
        xSign.SetActive(false);
        cursorSign.SetActive(true);
    }
}
