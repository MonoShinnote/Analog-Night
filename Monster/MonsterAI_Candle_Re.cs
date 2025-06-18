using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;


public class MonsterAI_Candle_Re : MonoBehaviour
{
    public float Sanity;
    private float DrainRate;
    public int AiLevel;
    public float DSrate;
    public int Phase;
    public int TimeInt;

    public bool isDelay;

    [SerializeField] private PostProcessVolume processVolume;
    private Vignette vignette;

    public PlayerCntrl playerCntrl;
    public MonsterManager otherEnemy;
    private CanvaManager canvaManager;
    private LightCntrl lightCntrl;


    public List<GameObject> CandleFlame;

    private AudioSource SelfAudio;
    private AudioSource[] AllAudio;
    private AudioSource[] enemyAudio;

    private GameObject[] Enemies;

    [SerializeField] private Image Black;
    [SerializeField] private RawImage Static;
    [SerializeField] private RawImage Eye;


    // Start is called before the first frame update
    void Start()
    {
        Sanity = 320;
        DrainRate = 1f;
        Phase = 0;
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        otherEnemy = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        canvaManager = GameObject.Find("CanvaManager").GetComponent<CanvaManager>();
        lightCntrl = GameObject.Find("LightPointer").GetComponent<LightCntrl>();

        Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        TimeInt = otherEnemy.DelayTime;

        SelfAudio = GetComponent<AudioSource>();
        AllAudio = FindObjectsOfType<AudioSource>();
        enemyAudio = new AudioSource[Enemies.Length];

        for (int i = 0; i < Enemies.Length; i++)
        {
            enemyAudio[i] = Enemies[i].GetComponent<AudioSource>();
        }

        if (otherEnemy.isNightmare)
        {
            DrainRate = 2;
        }

        StartCoroutine(Delay());
        StartCoroutine(Hallucination());

        if (processVolume.profile.TryGetSettings(out vignette))
        {
            return;
        }

    }

    IEnumerator Delay()
    {
        isDelay = true;
        yield return new WaitForSeconds(TimeInt);
        TimeInt = 0;
        isDelay = false;
        while (true)
        {
            if (playerCntrl.isCasting)
            {
                Sanity += DSrate;
                DSrate += 4;
                if (lightCntrl.isLightOn)
                {
                    DSrate += 1;
                }
                if (playerCntrl.CurtainClose)
                {
                    DSrate /= 2;
                }
                DSrate = Mathf.Clamp(DSrate, 0, 40);
            }
            yield return new WaitForSeconds(1);

            if (!playerCntrl.isCasting)
            {
                StartCoroutine(Attack());
                yield break;
            }
        }
    }

    IEnumerator Hallucination()
    {
        Color OrgColor = Eye.color;
        Color NewColor = new Color(1, 1, 1, 0.01f);
        Color OrgFade = Static.color;
        Color NewFade = new Color(1, 1, 1, 0.1f);
        while (true)
        {
            if (Sanity < 160)
            {
                NewColor = new Color(1, 1, 1, 0.1f);
            }
            else
            {
                NewColor = new Color(1, 1, 1, 0.01f);
            }
            Static.color = Color.Lerp(OrgFade, NewFade, 1 - (Sanity / 320));
            Eye.color = Color.Lerp(OrgColor, NewColor, 1 - (Sanity / 320));
            if (Sanity < 80)
            {
                Mute();
            }
            yield return new WaitForSeconds(0.03f);

        }
    }

    void Mute()
    {
        float CurrentVolume = SelfAudio.volume;
        float currentRatio = (Sanity / 320);
        for (int i = 0; i < AllAudio.Length; i++)
        {
            AllAudio[i].volume = currentRatio;
        }
        EnemyVoice();
        SelfAudio.volume = CurrentVolume;
    }

    void EnemyVoice()
    {
        for (int i = 0; i < enemyAudio.Length; i++)
        {
            enemyAudio[i].volume = 1;
        }
    }

    IEnumerator Attack()
    {
        while (!playerCntrl.isCasting)
        {
            Sanity -= (AiLevel / 5) + DrainRate;
            if (playerCntrl.CurtainClose)
            {
                Sanity -= 2;
            }
            yield return new WaitForSeconds(1);
        }
        DSrate = 0;
        StartCoroutine(Delay());
        yield break;
    }

    void SaveMonsterKill()
    {
        PlayerPrefs.SetString("MonsterKill", "Don't forget to pray! Or else... ");
        PlayerPrefs.Save();
    }

    private void FlameCntrl()
    {
        int CandleToOff = Mathf.Clamp((Phase), 0, Phase);

        for (int i = 0; i < CandleFlame.Count; i++)
        {
            if (i < CandleToOff)
            {
                CandleFlame[i].SetActive(false);
            }
            else
            {
                if (!CandleFlame[i].activeSelf)
                {
                    CandleFlame[i].SetActive(true);
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        Sanity = Mathf.Clamp(Sanity, 0, 320);

        int CurrentPhase = Mathf.FloorToInt (8-(Sanity / 40));

        CurrentPhase = Mathf.Clamp(CurrentPhase, 0, 8);

        if (Phase != CurrentPhase)
        {
            Phase = CurrentPhase;
            FlameCntrl();
        }

        SelfAudio.volume = Mathf.Lerp(1, 0, Sanity / 320);
        vignette.roundness.value = Mathf.Lerp(0.8f, 1, 1 - (Sanity / 320));

    }

}
