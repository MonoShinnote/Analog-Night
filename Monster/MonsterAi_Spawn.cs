using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAi_Spawn : MonoBehaviour
{
    public int AiLevel;
    public bool AtWindow;
    private int TimeInt;
    private int RandomArea;
    private int recTime;
    private int enemySpawn;
    public int enemyCount;
    public float killPhase;

    private PlayerCntrl playerCntrl;
    private MonsterManager otherEnemy;
    private CanvaManager canvaManager;
    private LightCntrl lightCntrl;
    private WindowCntrl windowCntrl;

    private AudioSource SelfAudio;
    [SerializeField] private AudioClip jumpscare;

    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private Transform[] spWindow;
    [SerializeField] private Transform[] spWall;
    [SerializeField] private Transform[] spCloset;
    [SerializeField] private Transform[] spDesk;

    [SerializeField] private GameObject armL;
    [SerializeField] private GameObject armR;

    private ArmAnimation armAniL;
    private ArmAnimation armAniR;

    public void SpawnWhere()
    {
        switch (enemySpawn)
        {
            case 0:
                if (!playerCntrl.isLeft)
                {
                    transform.position = spWindow[Random.Range(0, 2)].position;
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
        AtWindow = false;
        killPhase = 0;
        enemyCount = 0;
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        otherEnemy = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        canvaManager = GameObject.Find("CanvaManager").GetComponent<CanvaManager>();
        lightCntrl = GameObject.Find("LightPointer").GetComponent<LightCntrl>();
        windowCntrl = GameObject.Find("Window").GetComponent<WindowCntrl>();

        SelfAudio = GetComponent<AudioSource>();

        armAniL = armL.GetComponent<ArmAnimation>();
        armAniR = armR.GetComponent<ArmAnimation>();

        TimeInt = otherEnemy.DelayTime;
        StartCoroutine(Roll());
    }

    void Spawn()
    {
        Instantiate(enemyPrefab[0], transform.position, transform.rotation);

    }


    IEnumerator Roll()
    {
        yield return new WaitForSeconds(TimeInt);
        TimeInt = 15;
        while (enemyCount < 5)
        {
            yield return new WaitForSeconds(8.5f);
            if (Random.Range(1, 31) <= AiLevel)
            {
                enemySpawn = Random.Range(0, 4);
                if (playerCntrl.Hiding) { enemySpawn = 1; }
                SpawnWhere();
                enemyCount += 1;
            }
        }
         StartCoroutine(KillProgress());
        yield break;

    }

    IEnumerator KillProgress()
    {
        ArmEnter();
        while (true)
        {
            if (enemyCount <= 2)
            {
                StartCoroutine(Roll());
                yield break;
            }
            yield return null;
        }
    }

    public void Death()
    {
        SaveMonsterKill();
        canvaManager.GameOver();
    }

    void SaveMonsterKill()
    {
        PlayerPrefs.SetString("MonsterKill", "They will sometime watching you from the dark space.\nUse Flashlight to hold them off ");
        PlayerPrefs.Save();
    }

    void ArmEnter()
    {
        StartCoroutine(armAniL.Appearing());
        StartCoroutine(armAniR.Appearing());
    }

    void ArmLeave()
    {
        StartCoroutine(armAniL.Leaving());
        StartCoroutine(armAniR.Leaving());

    }

    // Update is called once per frame
    void Update()
    {

    }

}
