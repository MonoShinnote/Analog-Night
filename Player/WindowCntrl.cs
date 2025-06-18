using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCntrl : MonoBehaviour
{
    private CurtainAnimation CurtainAnimationL;
    private CurtainAnimation CurtainAnimationR;

    private AudioSource CurtainSound;

    private PlayerCntrl playerCntrl;

    private GameObject Light;
    // Start is called before the first frame update
    void Start()
    {
        CurtainAnimationL = GameObject.Find("CurtainL").GetComponent<CurtainAnimation>();
        CurtainAnimationR = GameObject.Find("CurtainR").GetComponent<CurtainAnimation>();
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        Light = GameObject.Find("WindowLight");
        playerCntrl.CurtainClose = false;
        
        CurtainSound = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        if (!playerCntrl.CurtainClose)
        {
            CurtainAnimationL.Close();
            CurtainAnimationR.Close();
            Light.SetActive(false);
            playerCntrl.CurtainClose = true;
            CurtainSound.Play();
        }
        else if (playerCntrl.CurtainClose)
        {
            CurtainAnimationL.Open();
            CurtainAnimationR.Open();
            Light.SetActive(true);
            playerCntrl.CurtainClose = false;
            CurtainSound.Play();
        }
    }
}
