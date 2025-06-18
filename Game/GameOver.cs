using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameOver : MonoBehaviour
{
    [SerializeField]private Image Fade;

    [SerializeField]private TextMeshProUGUI TimeRecord;
    [SerializeField]private TextMeshProUGUI Tips;
    

    // Start is called before the first frame update
    void Start()
    {
        Fade.color = new Color(0, 0, 0, 1);
        StartCoroutine(FadingBack());
        int Minutes = PlayerPrefs.GetInt("Minutes", 0);
        int Seconds = PlayerPrefs.GetInt("Seconds", 0);
        String LoadTips = PlayerPrefs.GetString("MonsterKill", "Good Luck");
        Tips.text = LoadTips;
        TimeRecord.text =(string.Format("{0:00}:{1:00}", Minutes, Seconds));
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
    }

    public IEnumerator Restart()
    {
        float CurrentTime = 0;
        Color OrgColor = Fade.color;
        Color NewColor = new Color(0, 0, 0, 1);
        while (CurrentTime < 1)
        {
            CurrentTime += Time.deltaTime;
            Fade.color = Color.Lerp(OrgColor, NewColor, CurrentTime / 1);
            yield return null;
        }
        Fade.color = NewColor;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(2);
    }
    public IEnumerator GiveUp()
    {
        float CurrentTime = 0;
        Color OrgColor = Fade.color;
        Color NewColor = new Color(0, 0, 0, 1);
        while (CurrentTime < 1)
        {
            CurrentTime += Time.deltaTime;
            Fade.color = Color.Lerp(OrgColor, NewColor, CurrentTime / 1);
            yield return null;
        }
        Fade.color = NewColor;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        StartCoroutine(Restart());
    }

    public void MainMenu()
    {
        StartCoroutine(GiveUp());
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
