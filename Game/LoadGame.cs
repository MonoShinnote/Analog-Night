using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    [SerializeField] private Image Fade;
    [SerializeField] private TextMeshProUGUI Continue;
    private Color OrgColor;

    private float Duration;
    private bool isStarting;

    // Start is called before the first frame update
    void Start()
    {
        isStarting = true;
        Fade.color = new Color(0, 0, 0, 1);
        OrgColor = Continue.color;
        OrgColor.a = 1;
        Continue.color = OrgColor;
        Duration = 0.5f;
        StartCoroutine(FadingBack());
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            yield return StartCoroutine(Shining(1f));

            yield return StartCoroutine(Shining(0.25f));
        }
    }

    private IEnumerator Shining(float TargetAlpha)
    {
        Color TextColor = Continue.color;
        float time = 0;
        float OrgAlpha = TextColor.a;
        while (time < Duration)
        {
            time+= Time.deltaTime;
            float NewAlpha = Mathf.Lerp(OrgAlpha, TargetAlpha, time / Duration);
            TextColor.a = NewAlpha;
            Continue.color = TextColor;
            yield return null;
        }
        TextColor.a = TargetAlpha;
        Continue.color= TextColor;
    }
    public IEnumerator FadingBack()
    {
        float CurrentTime = 0;
        Color OrgColor = Fade.color;
        Color NewColor = new Color(0, 0, 0, 0);
        while (CurrentTime < 4)
        {
            CurrentTime += Time.deltaTime;
            Fade.color = Color.Lerp(OrgColor, NewColor, CurrentTime / 4);
            yield return null;
        }
        Fade.color = NewColor;
        isStarting = false;
    }

    public IEnumerator PlayGame()
    {
        isStarting= true;
        float CurrentTime = 0;
        Color OrgColor = Fade.color;
        Color NewColor = new Color(0, 0, 0, 1);
        while (CurrentTime < 3)
        {
            CurrentTime += Time.deltaTime;
            Fade.color = Color.Lerp(OrgColor, NewColor, CurrentTime / 3);
            yield return null;
        }
        Fade.color = NewColor;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isStarting)
        {
            StartCoroutine(PlayGame());
        }
    }
}
