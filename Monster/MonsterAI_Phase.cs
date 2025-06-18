using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterAI_Phase : MonoBehaviour
{
    private float TimeInt = 1;
    public int AiLevel;
    public int Phase;
    public int HideCount;
    public float Agression;
    public bool AtHallway;
    public bool isChecking;
    private bool isEnd;
    private float speed = 180;

    private MonsterManager otherEnemy;
    private PlayerCntrl playerCntrl;
    private DoorCntrl doorCntrl;

    private CanvaManager canvaManager;

    private LightCntrl lightCntrl;

    private Vector3 targetPoint;
    private Vector3 offTargetPoint;
    private Vector3 targetPointF;
    private Vector3 targetPointC;
    private Vector3 HeadOrigin;
    private Vector3 HeadOrgRotation;

    private AudioSource audioSource;

    [SerializeField] private Transform playerView;

    [SerializeField] private GameObject head;

    [SerializeField] private Image Black;

    [SerializeField] private AudioClip Jumpscare;
   // [SerializeField] private AudioClip Checking;

    private GameObject freeze;
    private Room CurrentRoom;
    // Start is called before the first frame update
    void Start()
    {
        isEnd = false;

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;

        CurrentEnemyState = EnemyState.Start;
        EnemyPhaseRoam();
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        otherEnemy = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        doorCntrl = GameObject.Find("Door").GetComponent<DoorCntrl>();
        canvaManager = GameObject.Find("CanvaManager").GetComponent<CanvaManager>();
        lightCntrl = GameObject.Find("LightPointer").GetComponent<LightCntrl>();

        transform.position = otherEnemy.Origin;
        HeadOrigin = head.transform.localPosition;
        HeadOrgRotation = head.transform.eulerAngles;

        freeze = FreezeScreen.Freeze;
    }


    public enum EnemyState
    {
        Start,
        Move,
        Attack
    }


    public EnemyState CurrentEnemyState;
    public void EnemyPhaseRoam()
    {
        switch (CurrentEnemyState)
        {
            case EnemyState.Start:

                AtHallway = false;
                CurrentEnemyState = EnemyState.Move;
                Phase = 0;
                Agression = 0;
                HideCount = 0;
                EnemyPhaseRoam();
                break;
                
            case EnemyState.Move:
                if (Phase < 4)
                {
                    StartCoroutine(AgressionUp());
                }
                else if(Phase >= 4)
                {
                    CurrentEnemyState = EnemyState.Attack;
                    EnemyPhaseRoam();
                }
                break;
            case EnemyState.Attack:

                if (!otherEnemy.HallWayUsed && !playerCntrl.DoorClose)
                {
                    audioSource.volume = 0.8f;
                    StartCoroutine(KillDetect());
                    AtHallway = true;
                }
                else
                {
                    Phase -= 1;
                    StartCoroutine(KillDelay());
                }

                break;
                
        }
    }

    IEnumerator AgressionUp()
    {
        Agression = 0;
        while (Agression < 100)
        {
            Agression += (Random.Range(0, AiLevel - 1 ));
            yield return new WaitForSeconds(TimeInt);
        }
        Phase += 1;
        audioSource.volume += 0.2f;
        EnemyPhaseRoam();
        yield break;
    }

    IEnumerator KillDetect()
    {
        Agression = 0;
        doorCntrl.doorKill = true;
        yield return StartCoroutine(doorCntrl.DoorAnimation());
        while (Agression < 100)
        {
            yield return new WaitForSeconds(TimeInt);
            if (playerCntrl.Hiding)
            {
                StartCoroutine(HideCheck());
                yield break ;
            }
            Agression += Random.Range(0, 15) ;
        }
        if (playerCntrl.isRight && !playerCntrl.InSub)
        {
            StartCoroutine(RunAnimation());
        }
        else 
        {
            isEnd = true;
            StartCoroutine(HeadSlide());
        }
        yield break;

    }

    IEnumerator KillDelay()
    {
        yield return new WaitUntil(() => !otherEnemy.HallWayUsed);
        CurrentEnemyState = EnemyState.Move;
        EnemyPhaseRoam();
        yield break;

    }


    IEnumerator HideCheck()
    {
        GameFreeze();
        audioSource.volume = 1;
        yield return StartCoroutine(EnterEffect());
        yield return StartCoroutine(HeadCheck());
        while (HideCount<2)
        {
            yield return new WaitForSeconds(1);
            HideCount+=1;
            if (lightCntrl.isLightOn)
            {
                Agression += (Random.Range(0, AiLevel - 1));
                HideCount -= 1;
                if (Agression >= 100)
                {
                    isEnd = true;
                    HeadKillDesk();
                    yield break;
                }

            }
        }
        StartCoroutine(HeadLeave());
        yield break;
    }

    void SaveMonsterKill()
    {
        PlayerPrefs.SetString("MonsterKill", "Door can't prevent him. Hide and close your flashlight!");
        PlayerPrefs.Save();
    }

    void EnemySpawnKill()
    {
        isEnd = true;
        transform.position = otherEnemy.DoorJumpPoint;
        StartCoroutine(DoorKillAnimation());
    }

    IEnumerator DoorKillAnimation()
    {
        float currentTime = 0f;
        Vector3 orgRotation = transform.eulerAngles;
        Vector3 endRotation = new Vector3(25, orgRotation.y, orgRotation.z);

        while (currentTime < 0.2f)
        {
            transform.eulerAngles  = Vector3.Lerp(orgRotation, endRotation, currentTime/0.2f);
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.eulerAngles = endRotation;
        HeadJump();
    }

    void HeadJump()
    {
        isEnd = true;
        head.transform.position = targetPoint;
        if (playerCntrl.atDoor)
        {
            head.transform.eulerAngles = new Vector3(-90, 0, -90);
        }
        audioSource.PlayOneShot(Jumpscare);
        SaveMonsterKill();
        StartCoroutine(canvaManager.DeathScreen());
        
    }

    IEnumerator HeadSlide()
    {
        isEnd = true;
        head.transform.eulerAngles = new Vector3(-180, 90, 90);
        if (playerCntrl.InSub)
        {
            head.transform.eulerAngles = new Vector3(-180, 0, 70);
        }
        while (true)
        {
            while (!playerCntrl.isLeft)
            {
                while (Vector3.Distance(head.transform.position, targetPointF) > 0.1f)
                {
                    head.transform.position = Vector3.MoveTowards(head.transform.position, targetPointF, speed * Time.deltaTime);
                    yield return null;
                }
                yield return new WaitForSeconds(0.5f);
                audioSource.PlayOneShot(Jumpscare);
                SaveMonsterKill();
                StartCoroutine(canvaManager.DeathScreen());
                yield break;
            }
            yield return new WaitUntil(() => !playerCntrl.isLeft);
        }
    }
    IEnumerator RunAnimation()
    {
        isEnd = true ;
        float currentTime = 0;
        transform.position = new Vector3(40, 9, 6);
        Vector3 orgPosition = transform.position;
        while (currentTime < 0.5f)
        {
            transform.position = Vector3.Lerp(orgPosition, offTargetPoint, currentTime / 0.5f);
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.position = offTargetPoint;
        HeadJump();
        yield break;
    }
    IEnumerator HeadCheck()
    {
        float currentTime = 0;
        head.transform.localPosition = new Vector3(-6, 0, 0.6f);
        head.transform.eulerAngles = new Vector3(-270, 0, -90);
        Vector3 orgPosition = head.transform.localPosition;
        Vector3 newPosition = new Vector3(orgPosition.x, -5.5f, orgPosition.z);
        while (currentTime < 3)
        {
            head.transform.localPosition = Vector3.Lerp(orgPosition, newPosition, currentTime / 3);
            currentTime += Time.deltaTime;
            yield return null;
        }
        head.transform.localPosition = newPosition;
        yield return null;
    }

    IEnumerator HeadLeave()
    {
        float currentTime = 0;
        Vector3 orgPosition = head.transform.localPosition;
        Vector3 newPosition = new Vector3(orgPosition.x, 5.5f, orgPosition.z);
        while (currentTime < 2)
        {
            head.transform.localPosition = Vector3.Lerp(orgPosition, newPosition, currentTime / 2);
            currentTime += Time.deltaTime;
            yield return null;
        }
        head.transform.localPosition = newPosition;
        StartCoroutine(LeaveEffect());
        yield break;
    }
    IEnumerator EnterEffect()
    {
        float currentTime = 0;
        float duration = 0.02f;
        Color OrgColor = Black.color;
        Color NewColor = new Color(0, 0, 0, 1);
        while (currentTime < duration)
        {
            Black.color = Color.Lerp(OrgColor, NewColor, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        Black.color = NewColor;
        transform.position = otherEnemy.DeskPoint;
        while (currentTime < duration)
        {
            Black.color = Color.Lerp(NewColor, OrgColor, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        Black.color = OrgColor;
        yield break;
    }

    IEnumerator LeaveEffect()
    {
        float currentTime = 0;
        float duration = 0.02f;
        Color OrgColor = Black.color;
        Color NewColor = new Color(0, 0, 0, 1);
        while (currentTime < duration)
        {
            Black.color = Color.Lerp(OrgColor, NewColor, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        Black.color = NewColor;
        EnemyLeave();
        while (currentTime < duration)
        {
            Black.color = Color.Lerp(NewColor, OrgColor, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        Black.color = OrgColor;
        yield break;
    }

    void HeadKillDesk()
    {
        isEnd = true;
        head.transform.eulerAngles = HeadOrgRotation;
        head.transform.localPosition = targetPoint;
        audioSource.PlayOneShot(Jumpscare);
        SaveMonsterKill();
        StartCoroutine(canvaManager.DeathScreen());

    }


    void GameFreeze()
    {
        freeze.SetActive(true);
    }

    void GameUnFreeze()
    {
        freeze.SetActive(false);
    }

    void EnemyLeave()
    {
        transform.position = otherEnemy.Origin;
        head.transform.localPosition = HeadOrigin;
        head.transform.eulerAngles = HeadOrgRotation;
        StartCoroutine(ClearSound());
        GameUnFreeze();
        playerCntrl.DoorForceOpen = true;
        doorCntrl.doorKill = false;
        AtHallway = false;
        CurrentEnemyState = EnemyState.Start;
        EnemyPhaseRoam();
    }

    IEnumerator ClearSound()
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = playerView.position + playerView.forward * 1.0f;
        offTargetPoint = playerView.position + playerView.forward * 4f;
        targetPointF = playerView.position + playerView.forward * 1.2f;
        targetPointC = playerView.position + playerView.forward * 0.8f;

        if (isEnd)
        {
           GameFreeze();
            audioSource.volume = 1.0f;
        }
    }
}
