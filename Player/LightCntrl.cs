using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightCntrl : MonoBehaviour
{
    private float Depth;

    public float Power;
    public float MaxPower;
    public float MaxIntensity;
    private float PowerDrain;
    private float PowerUp;
    private float coolDown;
    private float errorChance;

    public bool isLightOn;
    public bool isError;
    
    private PlayerCntrl PlayerCntrl;
    private MonsterManager monster;
    private AudioSource audioSource;

    [SerializeField] private Camera Player;
    [SerializeField] private GameObject FlashLight;
    [SerializeField] private Light lightSource;
    [SerializeField] private BatteryUI Battery;
    [SerializeField] private AudioClip On;
    [SerializeField] private AudioClip Off;

    // Start is called before the first frame update
    void Awake()
    {
        Power = 1000;
        MaxPower = 1050;
        PowerDrain = 0.1f;
        PowerUp = 3;
        coolDown = 5f;
        errorChance = 0;
        Depth = 5;
        isLightOn = false;
        PlayerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        monster = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        Battery = GameObject.Find("BatteryUI").GetComponent<BatteryUI>();
        audioSource = GetComponent<AudioSource>();

        if (monster.isNightmare)
        {
            MaxPower = 1000;
            PowerDrain = 0.2f;
            PowerUp = 1.5f;
            coolDown = 60;
        }
        StartCoroutine(EnemyDetect());
        StartCoroutine(FalseDetect());
    }


    private void FollowMouse()
    {
       Vector3 MousePosition =Input.mousePosition;

        MousePosition.z = Depth;
        Vector3 worldPosition = Player.ScreenToWorldPoint(MousePosition);

        transform.position = worldPosition;
        
    }
    private void LightOff()
    {
        isLightOn = false;
        FlashLight.SetActive(false);
    }

    private void LightOn()
    {
        if (Power > 0)
        {
            isLightOn = true;
            FlashLight.SetActive(true);
        }
    }

    private IEnumerator FalseDetect()
    {
        while(true)
        {
            if (Random.value < errorChance)
            {
                StartCoroutine(LightError(2));
                yield return new WaitForSeconds(19);
            }
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator EnemyDetect()
    {
        while(true)
        {
            if(monster.HallWayUsed || monster.WindowUsed)
            {
                if (Random.value > 0.4f)
                {
                    StartCoroutine(LightError(2));
                    yield return new WaitForSeconds(coolDown);
                }
            }
            yield return new WaitUntil(()=>monster.HallWayUsed||monster.WindowUsed);
        }
    }
    public IEnumerator LightError(float errorTime)
    {
        float Duration = 0;
        isError = true;
        float OrgIntensity = MaxIntensity;
        while (Duration<errorTime)
        {
            Duration += 1;
            MaxIntensity -= Random.Range(0.1f, 0.4f);
            yield return new WaitForSeconds(0.05f);
            MaxIntensity = OrgIntensity;
            yield return new WaitForSeconds(0.05f);
        }
        if (Power <= 0)
        {
            LightOff();
        }
        isError = false;
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!isLightOn)
            {
                LightOn();
                if (Power > 0)
                {
                    audioSource.PlayOneShot(On, 1);
                }
            }
            else
            {
                LightOff();
                if(Power > 0)
                {
                    audioSource.PlayOneShot(Off, 1);
                }
            }
        }

        if (isLightOn)
        {
            Power -= PowerDrain;
        }
        else if(!isLightOn && Power>0)
        {
            Power += PowerUp * Time.deltaTime;
        }

        if (Power >= MaxPower)
        {
            Power = MaxPower;
        }

        if (Power <= 0)
        {
            Power = 0;
            if (isLightOn && !isError)
            {
                StartCoroutine(LightError(3));
            }
        }
        FollowMouse();

        errorChance = 1/Power;

        if (!isError)
        {
            MaxIntensity = 0.4f + (Power / 1000f) * 0.6f;
        }
        lightSource.intensity = MaxIntensity;
    }
}
