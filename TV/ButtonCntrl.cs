using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCntrl : MonoBehaviour
{
    [SerializeField]private VidCntrl videoCntrl;
    [SerializeField] private Material OrgMaterial;
    [SerializeField] private Material Highlight;
    private AudioSource audioSource;

    private Renderer MTrenderer;

    // Start is called before the first frame update
    void Start()
    {
        MTrenderer = GetComponent<Renderer>();
        MTrenderer.material = OrgMaterial;
        videoCntrl = GameObject.Find("Monitor").GetComponent<VidCntrl>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(ButtonTips());
    }

    //Control the television
    private void OnMouseDown()
    {
        if (videoCntrl.isOn) 
        {
            videoCntrl.isOn = false;
        }
        audioSource.Play();
    }

    //Highlight for a while so player notice the button
    IEnumerator ButtonTips()
    {
        int Count = 0;
        yield return new WaitForSeconds(1);
        while (Count <= 4)
        {
            MTrenderer.material = Highlight;
            yield return new WaitForSeconds(0.5f);
            MTrenderer.material = OrgMaterial;
            Count++;
            yield return new WaitForSeconds(0.5f);
        }
        MTrenderer.material = OrgMaterial;    
    }

}
