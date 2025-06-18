using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{

    public GameObject Select;

    // Start is called before the first frame update
    void Start()
    {
        Select.SetActive(false);
    }

    private void OnMouseEnter()
    {
            
           Select.SetActive(true);
            
        
    }

    private void OnMouseExit()
    {
            Select.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
