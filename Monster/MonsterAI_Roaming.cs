using System.Collections;
using UnityEngine;
public class MonsterAI_Roaming : MonoBehaviour
{
    private float TimeInt;
    public int AiLevel ;
    private int blockCount;
    public bool AtHallway;
    private bool isEnd;

    public float Speed;

    public Transform PlayerView;
    private Vector3 TargetPoint;

    private PlayerCntrl playerCntrl;
    private MonsterManager otherEnemy;
    private DoorCntrl doorCntrl;
    private CanvaManager canvaManager;

    private AudioSource audioSource;
    private GameObject freeze;

    [SerializeField] private AudioClip Jumpscare;

    public Room CurrentRoom;
    // Start is called before the first frame update
    void Start()
    {
        isEnd = false;
        blockCount = 0;
        Speed = 80;
        TimeInt = 10;
        CurrentEnemyState = EnemyState.Start;
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        otherEnemy = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        doorCntrl = GameObject.Find("Door").GetComponent<DoorCntrl>();
        canvaManager = GameObject.Find("CanvaManager").GetComponent<CanvaManager>();
        audioSource = GetComponent<AudioSource>();
        freeze = FreezeScreen.Freeze;

        EnemyRandomRoam();
    }


    public enum EnemyState
    {
        Start,
        Waiting,
        Move,
        Attack
    }

    
    public EnemyState CurrentEnemyState;
    public void EnemyRandomRoam()
    {
        switch (CurrentEnemyState)
        {
            case EnemyState.Start:
                transform.position = otherEnemy.Origin;
                CurrentRoom = Room.FrontDoor;
                AtHallway=false;
                CurrentEnemyState = EnemyState.Waiting;
                EnemyRandomRoam();
                break;
            case EnemyState.Waiting:
                StartCoroutine(MoveRoll());
                break;
            case EnemyState.Move:
                switch (CurrentRoom)
                {
                    case Room.FrontDoor:
                        if (Random.Range(0, 40) <= AiLevel)
                        {
                            CurrentRoom = Room.Stair;
                        }
                        else { CurrentRoom = Room.Other; }
                        CurrentEnemyState = EnemyState.Waiting;
                        break;

                    case Room.Other:
                        if (Random.Range(0, 3) == 0)
                        {
                            CurrentRoom = Room.Stair;
                        }
                        else { CurrentRoom = Room.Other; }
                        
                        CurrentEnemyState = EnemyState.Waiting;
                        break;
                        
                    case Room.Stair:
                        if (otherEnemy.HallWayUsed)
                        {
                            CurrentEnemyState = EnemyState.Waiting;
                        }
                        else
                        {
                            CurrentRoom = Room.Hallway;
                            AtHallway = true;
                            CurrentEnemyState = EnemyState.Waiting;
                        }
                        break;

                    case Room.Hallway:
                        CurrentRoom = Room.Ready;
                        CurrentEnemyState = EnemyState.Waiting;
                        break;

                    case Room.Ready:
                        CurrentEnemyState =EnemyState.Attack;
                        break;
                }
                EnemyRandomRoam();
                break;
            case EnemyState.Attack:

                StartCoroutine(KillDetect());
                
                break;
        }
    }

    IEnumerator MoveRoll()
    {
        TimeInt = 10;
        while (true)
        {
            yield return new WaitForSeconds(TimeInt);
            if (Random.Range(1, 31) <= AiLevel)
            {
                TimeInt = 10;
                CurrentEnemyState = EnemyState.Move;
                EnemyRandomRoam();
                yield break;
            }
            else
            {
                TimeInt /= 2;
                TimeInt = Mathf.Max(0.5f,TimeInt);
            }
        }
    }
   
    IEnumerator KillDetect()
    {
        float killTime = 0;
        blockCount = 0;
        transform.position = otherEnemy.HallwayPoint;
        while(killTime < 15)
        {
            yield return new WaitForSeconds(1);
            if (playerCntrl.DoorForceOpen)
            {
                StartCoroutine(InstaKillAnimation());
                yield break;
            }
            if (playerCntrl.DoorClose)
            {
                blockCount += 1;
                if (blockCount >= 5)
                {
                    CurrentEnemyState = EnemyState.Start;
                    EnemyRandomRoam();
                    yield break;

                }
            }
            else
            {
                killTime += 1;

            }
            if (Random.Range(0, 2) == 0)
            { doorCntrl.DoorKnocking(); }
        }
        yield return new WaitUntil(() => !audioSource.isPlaying);
        yield return StartCoroutine(doorCntrl.DoorDestroy());
        StartCoroutine(KillAnimation());
        yield break;
    }

    IEnumerator KillAnimation()
    {
        isEnd = true;
        while (true)
        {
            transform.position = otherEnemy.DoorPoint;
            while (Vector3.Distance(transform.position, TargetPoint) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, TargetPoint, Speed * Time.deltaTime);
                yield return null;
            }
            audioSource.PlayOneShot(Jumpscare, 1);
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(canvaManager.DeathScreen());
            SaveMonsterKill();
            yield break;
        }

    }


    IEnumerator InstaKillAnimation()
    {
        isEnd = true;
        while (true)
        {
            while (Vector3.Distance(transform.position, otherEnemy.DoorPoint) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, otherEnemy.DoorPoint, Speed * Time.deltaTime);
                yield return null;
            }
            while (Vector3.Distance(transform.position, TargetPoint) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, TargetPoint, Speed * Time.deltaTime);
                yield return null;
            }
            audioSource.PlayOneShot(Jumpscare, 1);
            yield return new WaitForSeconds(0.5f);
            SaveMonsterKill();
            StartCoroutine(canvaManager.DeathScreen());
            yield break;

        }
    }

    void SaveMonsterKill()
    {
        PlayerPrefs.SetString("MonsterKill", "Hold the door long enough and enventually it will give up");
        PlayerPrefs.Save();
    }
    void FaceCamera()
    {
        Vector3 lookDirection = PlayerView.position - transform.position;
        lookDirection.y = -360;
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
