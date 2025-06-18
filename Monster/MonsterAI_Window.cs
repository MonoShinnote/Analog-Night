using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterAI_Window : MonoBehaviour
{
    public float rate;
    public float TimeInt;
    public int AiLevel;
    public float Agression;
    public int Phase;
    public int BlockCount;
    public bool AtWindow;
    private float speed = 60f;
    private float KillLimit;
    private bool isEnd;

    public EnemyState CurrentEnemyState;

    [SerializeField]private Transform PlayerView;
    [SerializeField]private AudioClip Jumpscare;

    private PlayerCntrl playerCntrl;
    private MonsterManager otherEnemy;
    private CanvaManager canvaManager;

    private AudioSource audioSource;

    private GameObject freeze;

    private Vector3 TargetPoint;


    public enum EnemyState
    {
        Start,
        Attack,
    }

    // Start is called before the first frame update
    void Start()
    {
        isEnd = false;
        rate = 5;
        BlockCount = 0;
        Phase = 1;
        AtWindow = false;
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        otherEnemy = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        canvaManager = GameObject.Find("CanvaManager").GetComponent<CanvaManager>();

        audioSource = GetComponent<AudioSource>();  

        freeze = FreezeScreen.Freeze;

        TimeInt = otherEnemy.DelayTime ;

        EnemyWindow();
        StartCoroutine(RateUp());
    }

    public void EnemyWindow()
    {
        switch (CurrentEnemyState)
        {
            case EnemyState.Start:
                Phase = 0;
                rate = 5;
                transform.position = otherEnemy.UnderWindow;
                StartCoroutine(AgressionScale()); 
                break;
            case EnemyState.Attack:
                if (!otherEnemy.WindowUsed)
                {
                    EnemyEnter();
                }
                else if(otherEnemy.WindowUsed)
                { 
                    CurrentEnemyState = EnemyState.Start;
                    EnemyWindow();
                }
                break;
        }

    }
    IEnumerator RateUp()
    {
        yield return new WaitForSeconds(TimeInt);
        while (true)
        {
            rate += Random.Range(1, AiLevel / 2) + 1;
            yield return new WaitForSeconds(6);
        }

    }

    IEnumerator AgressionScale()
    {
        yield return new WaitForSeconds(TimeInt);
        TimeInt = 0;
        while (Phase < 7)
        {
            while (Agression < 60)
            {
                Agression += rate;
                yield return new WaitForSeconds(1);
            }
            rate = 1;
            Agression = 0;
            Phase += 1;
        }
        CurrentEnemyState = EnemyState.Attack;
        EnemyWindow();
    }

    IEnumerator EnemyEnterAnimation()
    {
        while (true)
        {
            transform.position += Vector3.up;
            yield return new WaitForSeconds(0.1f);
            if (transform.position == otherEnemy.WindowPoint)
            {
                StartCoroutine(KillDetect());
                yield break;
            }
        }
    }

    IEnumerator KillDetect()
    {
        KillLimit = 0;
        BlockCount = 0;
        AtWindow = true;
        while (KillLimit < 7)
        {
            yield return new WaitForSeconds(1);

            if (playerCntrl.CurtainClose || playerCntrl.Hiding)
            {
                BlockCount += 1;
                if (BlockCount >= 3)
                {
                    EnemyLeave();
                    yield break;
                }
            }
            else
            {
                KillLimit += 1;
            }
        }
        StartCoroutine(KillAnimation());
        yield break;
    }
    IEnumerator KillAnimation()
    {
        while (true)
        {
            if (!playerCntrl.isRight)
            {
                isEnd = true;
                while (Vector3.Distance(transform.position, TargetPoint) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, TargetPoint, speed * Time.deltaTime);
                    yield return null;
                }
                audioSource.PlayOneShot(Jumpscare);
                yield return new WaitForSeconds(0.5f);
                SaveMonsterKill();
                StartCoroutine(canvaManager.DeathScreen());
                yield break;
            }
            else { yield return new WaitUntil(() => !playerCntrl.isRight); }
        }
    }
    void SaveMonsterKill()
    {
        PlayerPrefs.SetString("MonsterKill", "Close the curtain or hide, don't let it see your face!");
        PlayerPrefs.Save();
    }

    public void EnemyEnter()
    {
        StartCoroutine(EnemyEnterAnimation());
    }
    public void EnemyLeave()
    {
        transform.position = otherEnemy.UnderWindow;
        CurrentEnemyState = EnemyState.Start;
        EnemyWindow();
        AtWindow = false;
        BlockCount = 0;
    }

    void FaceCamera()
    {
        Vector3 lookDirection = PlayerView.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    void GameFreeze()
    {
        freeze.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        TargetPoint = PlayerView.position + PlayerView.forward * 4;
        FaceCamera();
        if (isEnd)
        {
            GameFreeze();
        }
    }

}