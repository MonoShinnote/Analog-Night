using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VidCntrl : MonoBehaviour
{
    public bool isOn;

    public VideoClip CamHacked;
    public VideoClip CamClose;
    public VideoClip CamBlack;

    public VideoPlayer vidPlayer;

    // Start is called before the first frame update
    void Start()
    {
        isOn = false;
        vidPlayer = GetComponent<VideoPlayer>();
        vidPlayer.clip = CamBlack;
        vidPlayer.Play();

    }
    public void CamHacking()
    {
        isOn = true;
        vidPlayer.clip = CamHacked;
        vidPlayer.Play();
    }

    public void CamClosing()
    {
        vidPlayer.Stop();
        vidPlayer.clip = CamClose;
        vidPlayer.Play();
        isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
      /*  if (!isOn)
        {
            vidPlayer.clip = CamBlack;
            vidPlayer.Play();
        } */
    }
}
