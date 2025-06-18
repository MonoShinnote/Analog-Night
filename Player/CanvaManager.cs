using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CanvaManager : MonoBehaviour
{
    public GameObject LeftToggle;
    public GameObject RightToggle;
    public GameObject TopToggle;
    public GameObject BottomToggle;

    public Image Fade;
    public Image Death;

    private Toggle LToggle;
    private Toggle RToggle;

    public float FadeTime = 1;
    private float Seconds;
    private int Minutes;
    private int RealSeconds;
    private bool isEnd;

    public TextMeshProUGUI TimerText;

    private PlayerCntrl PlayerCntrlCs;
    private MonsterManager monsterManager;
    // Start is called before the first frame update
    void Start()
    {
        PlayerCntrlCs = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        LToggle = GameObject.Find("LeftToggle").GetComponent<Toggle>();
        RToggle = GameObject.Find("RightToggle").GetComponent <Toggle>();
        TopToggle.SetActive(false);

        isEnd = false;    

        monsterManager =GameObject.Find("MonsterManager").GetComponent<MonsterManager>();

        Death.color = new Color(1, 1, 1, 0);
        Fade.color = new Color(0, 0, 0, 0);
        //Test Color
       // StartCoroutine(DeathScreen());
    }

    
    public IEnumerator FadingBlack()
    {
        float CurrentTime = 0;
        Color OrgColor = Fade.color;
        Color NewColor = new Color(0,0,0,1);
        while (CurrentTime < FadeTime)
        {
            CurrentTime += Time.deltaTime;
            Fade.color = Color.Lerp(OrgColor, NewColor, CurrentTime / FadeTime);
            yield return null;
        }
        Fade.color = NewColor;
    }

    public IEnumerator FadingBack()
    {
        float CurrentTime = 0;
        Color OrgColor = Fade.color;
        Color NewColor = new Color(0, 0, 0, 0);
        while (CurrentTime < FadeTime)
        {
            CurrentTime += Time.deltaTime;
            Fade.color = Color.Lerp(OrgColor, NewColor, CurrentTime / FadeTime);
            yield return null;
        }
        Fade.color = NewColor;
    }

    public IEnumerator DeathScreen()
    {
        isEnd = true;
        float CurrentTime = 0;
        Color OrgColor = Death.color;
        Color NewColor = new Color(1, 0, 0, 1);
        while (CurrentTime < 0.5f)
        {
            CurrentTime += Time.deltaTime;
            Death.color = Color.Lerp(OrgColor, NewColor, CurrentTime / 0.5f);
            yield return null;
        }
        Death.color = NewColor;
        yield return new WaitForSeconds(1);
        StartCoroutine(FadingBlack());
        yield return new WaitForSeconds(2);
        GameOver();
        yield break;
    }

    public void GameOver()
    {
        PlayerPrefs.SetInt("Minutes", Minutes);
        PlayerPrefs.SetInt("Seconds", RealSeconds);
        SceneManager.LoadScene(3);

    }

    void SaveWin()
    {
        if (monsterManager.isNightmare)
        {
            PlayerPrefs.SetString("MonsterKill", "Congratulation! Now Try Infinity Mode! ");
        }
        else
        {
            PlayerPrefs.SetString("MonsterKill", "Try Nightmare Mode! It is easier,I promised");
        }
        PlayerPrefs.Save();
    }

    public IEnumerator GameWin()
    {
        isEnd = true;
        SaveWin();
        yield return StartCoroutine(FadingBlack());
        PlayerPrefs.SetInt("Minutes", Minutes);
        PlayerPrefs.SetInt("Seconds", RealSeconds);
        SceneManager.LoadScene(4);
    }


    void UpdateTimer()
    {
        RealSeconds = Mathf.FloorToInt(Seconds);
        TimerText.text = string.Format("{0:00}:{1:00}", Minutes, RealSeconds);
    }


    // Update is called once per frame
    void Update()
    {
       
        if (PlayerCntrlCs.isMoving)
        {
            LeftToggle.SetActive(false);
            RightToggle.SetActive(false);
            BottomToggle.SetActive(false);
            TopToggle.SetActive(false);
        }
        else
        {
            if (PlayerCntrlCs.isFront == true)
            {
                LeftToggle.SetActive(true);
                RightToggle.SetActive(true);
                BottomToggle.SetActive(true);
                TopToggle.SetActive(false);

                if (PlayerCntrlCs.Hiding)
                {
                    LeftToggle.SetActive(false);
                    RightToggle.SetActive(false);
                    BottomToggle.SetActive(false);
                    TopToggle.SetActive(true);
                }
            }
            else if (PlayerCntrlCs.isLeft == true)
            {
                LeftToggle.SetActive(false);
                RightToggle.SetActive(true);
                BottomToggle.SetActive(false);


            }
            else if (PlayerCntrlCs.isRight == true)
            {
                LeftToggle.SetActive(true);
                RightToggle.SetActive(false);
                BottomToggle.SetActive(false);
                if (PlayerCntrlCs.atDoor)
                {
                    LeftToggle.SetActive(false);
                    RightToggle.SetActive(true);
                    BottomToggle.SetActive(true);
                    if (PlayerCntrlCs.InSub)
                    {
                        LeftToggle.SetActive(false);
                        RightToggle.SetActive(true);
                        BottomToggle.SetActive(false);
                    }
                }
            }
        }
        if (!isEnd)
        {
            Seconds += Time.deltaTime;
            if (Seconds >= 60)
            {
                Minutes += 1;
                Seconds = 0;
            }
            if (!monsterManager.isInfi)
            {
                if (!monsterManager.isNightmare)
                {
                    if (Minutes >= 6)
                    {
                        StartCoroutine(GameWin());
                    }
                }
                if (Minutes >= 9)
                {
                    StartCoroutine(GameWin());
                }
            }
            UpdateTimer();
        }
    }
}
