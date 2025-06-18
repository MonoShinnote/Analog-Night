//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;


public class SpawnManager : MonoBehaviour
{
    public int AiLevel;
    public float Agression;
    public bool AtWindow;
    private bool windowSpawn;
    private int TimeInt;
    private int RandomArea;
    private int recTime;
    private int enemySpawn;
    public int enemyCount;

    private PlayerCntrl playerCntrl;
    private MonsterManager otherEnemy;
    private CanvaManager canvaManager;
    private LightCntrl lightCntrl;
    private WindowCntrl windowCntrl;

    [SerializeField] private PostProcessVolume processVolume;
    private Vignette vignette;
    private Grain grain;

    private AudioSource SelfAudio;
    private AudioSource[] AllAudio;
    [SerializeField] private AudioClip jumpscare;

    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private Transform[] spWindow;
    [SerializeField] private Transform[] spWall;
    [SerializeField] private Transform[] spCloset;
    [SerializeField] private Transform[] spDesk;

    [SerializeField] private Image Black;
    [SerializeField] private RawImage Static;
    [SerializeField] private RawImage Eye;
    private float FadeTime = 2;

    public void SpawnWhere()
    {
        switch (enemySpawn)
        {
            case 0:
                if (!playerCntrl.isLeft)
                {
                    if (playerCntrl.CurtainClose && !otherEnemy.WindowUsed)
                    {
                        transform.position = spWindow[2].position;
                        windowSpawn = true;
                    }
                    else
                    {
                        transform.position = spWindow[Random.Range(0, 2)].position;
                    }
                }
                else
                {
                    enemySpawn = 3;
                    SpawnWhere();
                }
                break;
            case 1:
                if (!playerCntrl.isFront || playerCntrl.Hiding)
                {
                    transform.position = spWall[Random.Range(0, spWall.Length)].position;
                }
                else
                {
                    enemySpawn = 2;
                    SpawnWhere();
                }
                break;
            case 2:
                if (!playerCntrl.isRight)
                {
                    transform.position = spCloset[Random.Range(0, spCloset.Length)].position;
                }
                else
                {
                    enemySpawn = 0;
                    SpawnWhere();
                }
                break;
            case 3:
                if (!playerCntrl.Hiding)
                {
                    transform.position = spDesk[Random.Range(0, spDesk.Length)].position;
                }
                else
                {
                    enemySpawn = 1;
                    SpawnWhere();
                }
                break;
        }
        Spawn();
    }
    // Start is called before the first frame update
    void Start()
    {
        Agression = 0;
        windowSpawn = false;
        AtWindow = false;
        enemyCount = 0;
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        otherEnemy = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        canvaManager = GameObject.Find("CanvaManager").GetComponent<CanvaManager>();
        lightCntrl = GameObject.Find("LightPointer").GetComponent<LightCntrl>();
        windowCntrl = GameObject.Find("Window").GetComponent<WindowCntrl>();

        SelfAudio = GetComponent<AudioSource>();
        AllAudio = FindObjectsOfType<AudioSource>();

        TimeInt = otherEnemy.DelayTime;
        if (otherEnemy.isNightmare)
        {
            Agression = 30;
        }


        StartCoroutine(Recovery());
        StartCoroutine(ErrorEffect());
        StartCoroutine(Roll());

        if (processVolume.profile.TryGetSettings(out vignette))
        {
            return;
        }
    }

    IEnumerator Roll()
    {
        while (true)
        {
            while (enemyCount < 5)
            {
                yield return new WaitForSeconds(5);
                if (Random.Range(1, 31) <= AiLevel)
                {
                    enemySpawn = Random.Range(0, 4);
                    if (playerCntrl.Hiding) { enemySpawn = 1; }
                    SpawnWhere();
                    enemyCount++;
                }
                else
                {
                    yield return null;
                }
            }
            yield return new WaitUntil(() => enemyCount < 5);
        }
    }

    IEnumerator Recovery()
    {
        while (true)
        {
            if (!playerCntrl.CurtainClose)
            {
                Agression -= 1;
            }
            if (playerCntrl.InSub)
            {
                if (Random.value > 0.5f)
                {
                    Agression -= 1;
                }
            }
            yield return new WaitForSeconds(recTime);
            if (Agression >= 150)
            {
                KillAnimation();
                yield break;
            }
        }
    }

    IEnumerator LightSpark()
    {
        while (true)
        {
            if (Random.Range(0, 10) <= enemyCount)
            {
                lightCntrl.LightError(3);
            }

            yield return null;
        }
    }

    IEnumerator ErrorEffect()
    {
        Color OrgColor = Eye.color;
        Color NewColor = new Color(1, 1, 1, 0.1f);
        Color OrgFade = Static.color;
        Color NewFade = new Color(1, 1, 1, 0.1f);
        while (Agression < 150)
        {
            if (Agression >= 60)
            {
                Static.color = Color.Lerp(OrgFade, NewFade, (Agression - 60) / 150);
                Eye.color = Color.Lerp(OrgColor, NewColor, (Agression - 60) / 150);
                Mute();
                yield return null;
            }
            yield return null;
        }
    }
    void Mute()
    {
        float currentRatio = 1 - ((Agression - 60) / 150);
        for (int i = 0; i < AllAudio.Length; i++)
        {
            AllAudio[i].volume = currentRatio;
        }
        SelfAudio.volume = 1;
    }
    void Spawn()
    {
        if (windowSpawn)
        {
            Instantiate(enemyPrefab[1], transform.position, transform.rotation);
            windowSpawn = false;
            AtWindow = true;
        }
        else
        {
            Instantiate(enemyPrefab[0], transform.position, transform.rotation);
        }
    }

    void SaveMonsterKill()
    {
        PlayerPrefs.SetString("MonsterKill", "They will sometime watching you from the dark space.\nUse Flashlight to hold them off ");
        PlayerPrefs.Save();
    }

    void KillAnimation()
    {
        Color newColor = Black.color;
        newColor.a = 1;
        Black.color = newColor;
        SelfAudio.PlayOneShot(jumpscare, 1);
        StartCoroutine(Effect());
        StartCoroutine(Jumpscare());
    }

    public IEnumerator Effect()
    {
        float CurrentTime = 0;
        Color OrgColor = Static.color;
        Color NewColor = new Color(1, 1, 1, 0.5f);
        while (CurrentTime < FadeTime)
        {
            CurrentTime += Time.deltaTime;
            Static.color = Color.Lerp(OrgColor, NewColor, CurrentTime / FadeTime);
            yield return null;
        }
        Static.color = NewColor;
    }

    public IEnumerator Jumpscare()
    {
        float CurrentTime = 0;
        Color OrgColor = Eye.color;
        Color NewColor = new Color(1, 1, 1, 1);
        while (CurrentTime < FadeTime)
        {
            CurrentTime += Time.deltaTime;
            Eye.color = Color.Lerp(OrgColor, NewColor, CurrentTime / FadeTime);
            yield return null;
        }
        Eye.color = NewColor;
        yield return new WaitForSeconds(2);
        SaveMonsterKill();
        canvaManager.GameOver();
    }


    // Update is called once per frame
    void Update()
    {
        if (!lightCntrl.isLightOn)
        {
            recTime = 2;
        }
        else { recTime = 1; }

        Agression = Mathf.Clamp(Agression, 0, 150);
        SelfAudio.volume = Mathf.InverseLerp(0, 120, Agression);

        vignette.roundness.value = Mathf.Lerp(0.8f, 1, Agression / 120);


    }

}