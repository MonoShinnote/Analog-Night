using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAi_AntiCam : MonoBehaviour
{
    public float TimeInt;
    public int AiLevel;
    public float Agression;
    public int ScreenKnocked;

    private float speed;
    private bool isEnd;

    [SerializeField] private Transform PlayerView;
    private Vector3 TargetPoint;

    private VidCntrl vidCntrl;
    private CamCntrl camCntrl;
    private MonsterManager otherEnemy;
    private CanvaManager canvaManager;

    private AudioSource audioSource;
    [SerializeField] private AudioClip Jumpscare;

    private GameObject freeze;
    [SerializeField] private GameObject TvLight;

    // Start is called before the first frame update
    void Start()
    {
        //OrgTime = 30
        TimeInt = 30;

        speed = 60;

        isEnd = false;
        vidCntrl = GameObject.Find("Monitor").GetComponent<VidCntrl>();
        camCntrl = GameObject.Find("Cam").GetComponent <CamCntrl>();
        otherEnemy = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        canvaManager = GameObject.Find("CanvaManager").GetComponent<CanvaManager>();
        audioSource = GetComponent<AudioSource>();
        TvLight.SetActive(false);
        freeze = FreezeScreen.Freeze;

        StartCoroutine(MoveRoll());
    }

    IEnumerator MoveRoll()
    {
        TimeInt = 30;
        while (true)
        {
            yield return new WaitForSeconds(TimeInt);
            if (Random.Range(1, 31) <= AiLevel)
            {
               StartCoroutine (AppearOnCam());
                TimeInt = 30;
                yield break;
            }
            else
            {
                TimeInt /= 3;
                TimeInt = Mathf.Max(4, TimeInt);
            }
        }
    }

    IEnumerator AppearOnCam()
    {
        ScreenKnocked = 0;
        vidCntrl.CamHacking();
        TvLight.SetActive(true);
        yield return new WaitForSeconds(8);
        vidCntrl.isOn=true;
        while (ScreenKnocked < 9)
        {
            ScreenKnocked += 1;
            Debug.Log("knock_Screen" + ScreenKnocked);
            yield return new WaitForSeconds(3);
            if (!vidCntrl.isOn)
            {
                vidCntrl.CamClosing();
                TvLight.SetActive (false);
                StartCoroutine(MoveRoll());
                yield break;
            }
        }
        StartCoroutine(KillAnimation());
        yield break;

    }

    IEnumerator KillAnimation()
    {
        isEnd = true;
        while (Vector3.Distance(transform.position, TargetPoint) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPoint, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(Jumpscare, 1);
        StartCoroutine(canvaManager.DeathScreen());
        SaveMonsterKill();
        yield break;

    }

    void SaveMonsterKill()
    {
        PlayerPrefs.SetString("MonsterKill", "Close your monitor before it is too late...");
        PlayerPrefs.Save();
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
        TargetPoint = PlayerView.position + PlayerView.forward * 1;
        FaceCamera();

        if (isEnd)
        {
            GameFreeze();
        }
    }
}
