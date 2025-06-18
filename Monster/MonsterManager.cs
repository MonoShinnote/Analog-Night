using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Room
{
    FrontDoor,
    BackDoor,
    /*Bedroom,
    Laundry,
    LivingRoom01,
    LivingRoom02,
    Kitchen,
    DiningRoom,*/
    Other,
    Stair,
    Hallway,
    Ready,
}

public class MonsterManager : MonoBehaviour
{
    public MonsterAI_Roaming monsterA;
    public MonsterAI_Phase monsterB;
    public MonsterAI_Window monsterC;
    public MonsterAi_Spawn monsterD;
    public MonsterAI_Candle_Re monsterE;
    public MonsterAi_AntiCam monsterF;

    public bool HallWayUsed;
    public bool WindowUsed;
    public bool EnemyInRoom;
    public bool isNightmare;
    public bool isInfi;
    public int difficulty;
    public int TimerRate;
    public int[] AiList ;
    public int Scale;
    public int Counter;
    public int DelayTime;

    public Vector3 Origin = new Vector3(68, 8, 6);
    public Vector3 SkyPoint = new Vector3(-18, 30, 1);
    public Vector3 UnderWindow = new Vector3(-13, 1, 2);
    public Vector3 HallwayPoint = new Vector3(40, 8, 16);
    public Vector3 DeskPoint = new Vector3(10, 9, 7);
    public Vector3 WindowPoint = new Vector3(-13, 11, 2);
    public Vector3 DoorPoint = new Vector3(40, 8, 6);
    public Vector3 DoorJumpPoint = new Vector3(40, 6, 0);
    public Vector3 TvPoint = new Vector3(-4.45f, 8.3f, 6.85f);

    public Room room;

    // Start is called before the first frame update
    void Awake()
    {
        difficulty = -1;
        Counter = 0;
        TimerRate = 35;
        DelayTime = 30;
        Scale = 1;

        isNightmare = PlayerPrefs.GetInt("HardMode", 0) == 1;
        isInfi = PlayerPrefs.GetInt("InfiMode", 0) == 1;

        AiList = new int[6];
        monsterA = GameObject.Find("MonsterA").GetComponent<MonsterAI_Roaming>();
        monsterB = GameObject.Find("MonsterB").GetComponent<MonsterAI_Phase>();  
        monsterC = GameObject.Find("MonsterC").GetComponent<MonsterAI_Window>();
        monsterD = GameObject.Find("MonsterD").GetComponent<MonsterAi_Spawn>();
        monsterE = GameObject.Find("MonsterE").GetComponent<MonsterAI_Candle_Re>();
        monsterF = GameObject.Find("MonsterF").GetComponent<MonsterAi_AntiCam>();

        if (isNightmare)
        {
            DelayTime = 0;
            TimerRate = 30;
        }
        DifficultyChange();
        StartCoroutine(SpawnDelay());

    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(DelayTime);
        difficulty = 0;
        StartCoroutine(DifficultTimer());
        yield break;
    }

    IEnumerator DifficultTimer()
    {
        while (true)
        {
            DifficultyChange();
            yield return new WaitForSeconds(TimerRate);
            difficulty += 1;
            if (difficulty > 15)
            {
                Scale += 1;
                if (difficulty >= 50)
                {
                    difficulty = 50;
                }
            }
            yield return null;
        }
    }

    public void DifficultyChange()
    { 
        if (difficulty == -1)
        {
            AiList = new int[] { 0, 0, 0, 0, 0, 0 };

        }
        else if (difficulty == 0)
        {
            AiList = new int[] { 1, 10, 1, 5, 0, 1 };
        }
        else if (difficulty == 2)
        {
            AiList = new int[] { 10, 10, 10, 10, 0, 10 };
        }
        else if (difficulty == 6)
        {
            AiList = new int[] { 15, 20, 10, 10, 0, 10 };
        }
        else if (difficulty >= 8 )
        {
            AddScale();
        }
        AiList[4] = 10;
        if (difficulty > 10) { AiList[4] = 15; }
        else if (difficulty > 20) {  AiList[4] = 20;}
    }
    public void AddScale()
    {
        for (int i = 0; i < AiList.Length; i++) 
        {
            if (difficulty >= 15)
            {
                AiList[i] += Scale;
            }
            else
            {
                AiList[i] += 1;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (monsterA.AtHallway||monsterB.AtHallway) { HallWayUsed = true; }
        else { HallWayUsed = false; }
        if (monsterC.AtWindow || monsterD.AtWindow) { WindowUsed = true; }
        else {  WindowUsed = false; }
        monsterA.AiLevel = AiList[0];
        monsterB.AiLevel = AiList[1];
        monsterC.AiLevel = AiList[2];
        monsterD.AiLevel = AiList[3];
        monsterE.AiLevel = AiList[4];
        monsterF.AiLevel = AiList[5];

    }
}
