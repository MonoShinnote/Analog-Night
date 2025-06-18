using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    public Image Fade;
    private VideoPlayer videoPlayer;
    private HardMode hardMode;
    [SerializeField] private VideoClip BlueScreen;
    [SerializeField] private VideoClip EAS;
    [SerializeField] private Image Skip;
    private GameObject PauseScreen;
    private GameObject SkipButton;

    private bool isPlaying;
    private bool isPressStart;

    private void Awake()
    {
        isPressStart = false;
        hardMode = GetComponent<HardMode>();
        videoPlayer = GameObject.Find("BlueScreen").GetComponent<VideoPlayer>();
        PauseScreen = GameObject.Find("PauseScreen");
        SkipButton = GameObject.Find("Skip");
        SkipButton.SetActive(false);
        videoPlayer.clip = BlueScreen;
        videoPlayer.Play();
        PauseScreen.SetActive(false);
        Fade.color = new Color(0, 0, 0, 1);
        hardMode.LoadSetting();
        StartCoroutine(FadingBack());
    }
    public IEnumerator FadingBack()
    {
        float CurrentTime = 0;
        Color OrgColor = Fade.color;
        Color NewColor = new Color(0, 0, 0, 0);
        while (CurrentTime < 2)
        {
            CurrentTime += Time.deltaTime;
            Fade.color = Color.Lerp(OrgColor, NewColor, CurrentTime / 2);
            yield return null;
        }
        Fade.color = NewColor;
        yield break;
    }
    public IEnumerator Starting()
    {
        videoPlayer.loopPointReached -= StartGame;

        float CurrentTime = 0;
        Color OrgColor = Fade.color;
        Color NewColor = new Color(0, 0, 0, 1);
        while (CurrentTime < 2)
        {
            CurrentTime += Time.deltaTime;
            Fade.color = Color.Lerp(OrgColor, NewColor, CurrentTime / 2);
            yield return null;
        }
        Fade.color = NewColor;
        yield return new WaitForSeconds(1);
        if (hardMode.isHidden)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
    private IEnumerator SkipAppear()
    {
        float currentTime = 0;
        Color color = Skip.color;
        yield return new WaitForSeconds(5);
        while (currentTime < 10)
        {
            currentTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, currentTime / 10);
            Skip.color = color;
            yield return null;
        }
        color.a = 1;
        Skip.color = color;
        yield break;
    }
    public void StartVid()
    {
        isPressStart = true;
        if(hardMode.isHard || hardMode.isInfi)
        {
            StartCoroutine(Starting());
        }
        else
        {
            SkipButton.SetActive(true);
            StartCoroutine(SkipAppear());
            videoPlayer.clip = EAS;
            videoPlayer.Play();
            videoPlayer.loopPointReached += StartGame;
        }
    }

    public void StartGame(VideoPlayer vp)
    {
        videoPlayer.Stop();
        StartCoroutine(Starting());
    }

    public void SkipGame()
    {
        StartCoroutine(Starting());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) && isPressStart))
        {
            if (isPlaying)
            {
                videoPlayer.frame = videoPlayer.frame;
                videoPlayer.Pause();
                PauseScreen.SetActive(true);
                isPlaying = false;
            }
            else if (!isPlaying)
            {
                videoPlayer.Play();
                PauseScreen.SetActive(false);
                isPlaying = true;
            }
        }
    }
}
