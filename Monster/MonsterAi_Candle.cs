using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterAi_Candle : MonoBehaviour
{
    public float Agression;
    private float DrainRate;
    public int AiLevel;
    public float DSrate;
    public int Phase;
    public int TimeInt;


    public PlayerCntrl playerCntrl;
    public MonsterManager otherEnemy;
    private CanvaManager canvaManager;

    [SerializeField]private GameObject armL;
    [SerializeField]private GameObject armR;

    private ArmAnimation armAniL;
    private ArmAnimation armAniR;

    public List<GameObject> CandleFlame;
    // Start is called before the first frame update
    void Start()
    {
        Agression = 0;
        DrainRate = 1;
        Phase = 0;
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        otherEnemy = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        canvaManager = GameObject.Find("CanvaManager").GetComponent<CanvaManager>();

        armAniL = armL.GetComponent<ArmAnimation>();
        armAniR = armR.GetComponent<ArmAnimation>();

        TimeInt = otherEnemy.DelayTime;
     
        if (otherEnemy.isNightmare)
        {
            DrainRate = 2f;
        }
     
        StartCoroutine(Delay());

    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(TimeInt);
        TimeInt = 0;
        while (true)
        {
            if (playerCntrl.isCasting && !otherEnemy.EnemyInRoom)
            {
                Agression -= DSrate;
                DSrate += 1;
            }
            yield return new WaitForSeconds(1);

            if (!playerCntrl.isCasting )
            {
                StartCoroutine(Attack());
                yield break;
            }
        }
    }

    IEnumerator Attack()
    {
        while (!playerCntrl.isCasting )
        {
            while (otherEnemy.EnemyInRoom)
            {
                Agression += 8;
                yield return new WaitForSeconds(0.5f);
                if (Agression >= 256)
                {
                    SaveMonsterKill();
                    StartCoroutine(KillAnimation());
                }

            }
            Agression += (AiLevel / 5) + DrainRate;
            yield return new WaitForSeconds(1);
            if (Agression >= 256)
            {
                SaveMonsterKill();
                StartCoroutine(KillAnimation());
            }

        }
        DSrate = 1;
        StartCoroutine(Delay());
        yield break;
    }

    IEnumerator KillAnimation()
    {
        armL.SetActive(true);
        armR.SetActive(true);

        StartCoroutine(armAniL.Animation());
        yield return StartCoroutine(armAniR.Animation());
        yield return new WaitForSeconds(1);
        canvaManager.GameOver();
    }

    void SaveMonsterKill()
    {
        PlayerPrefs.SetString("MonsterKill", "Don't forget to pray! Or else... ");
        PlayerPrefs.Save();
    }

    public void FlameCntrl()
    {
        int CandleToOff = Mathf.Clamp((Phase),0,Phase);

        for (int i = 0; i < CandleFlame.Count; i++)
        {
            if(i < CandleToOff)
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
        Agression = Mathf.Clamp(Agression, 0, 256);

        int CurrentPhase = Mathf.FloorToInt(Agression / 32f);

        CurrentPhase = Mathf.Clamp(CurrentPhase, 0, 8);

        if(Phase != CurrentPhase)
        {
            Phase = CurrentPhase;
            FlameCntrl();
        }
    }
}
