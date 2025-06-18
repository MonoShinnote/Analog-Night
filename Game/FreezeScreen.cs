using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeScreen : MonoBehaviour
{
    public static GameObject Freeze;
    // Start is called before the first frame update
    void Awake()
    {
        Freeze = gameObject;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
