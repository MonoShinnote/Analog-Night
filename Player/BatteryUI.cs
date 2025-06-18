using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] private GameObject[] Battery;

    private LightCntrl lightCntrl;

    public int BatteryBar;
    public int BatteryFilled;

    public float powerPerBar;

    // Start is called before the first frame update
    void Start()
    {
        lightCntrl = GameObject.Find("LightPointer").GetComponent<LightCntrl>();

        BatteryBar = Battery.Length;
        powerPerBar = lightCntrl.MaxPower / BatteryBar;


    }


    void UpdateBattery()
    {
        BatteryFilled = Mathf.CeilToInt(lightCntrl.Power / powerPerBar);

        for(int i = 0; i < BatteryBar; i++)
        {
            if (i < BatteryFilled)
            {
                Battery[i].SetActive(true);
            }
            else
            {
                Battery[i].SetActive(false);
            }
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        UpdateBattery();
    }
}
