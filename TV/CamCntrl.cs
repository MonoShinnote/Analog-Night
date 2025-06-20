using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCntrl : MonoBehaviour
{
    public bool isOn;
    public bool CamDamaged;
    public bool CamDamagedEx;

    // Start is called before the first frame update
    void Start()
    {
        isOn = false;
        CamDamaged = false;
        CamDamagedEx = false;
    }

}
