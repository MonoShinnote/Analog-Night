using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainAnimation : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Close()
    {
        transform.localScale = new Vector3(1, 1, 0.55f);
        
    }
    public void Open()
    {
        transform.localScale = new Vector3(1, 1, 0.25f);
    }

}
