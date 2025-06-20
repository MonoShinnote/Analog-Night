using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    private PlayerCntrl PlayerCntrl;

    // Start is called before the first frame update
    void Start()
    {
        PlayerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
    }

    private void OnMouseEnter()
    {
        if (!PlayerCntrl.isMoving)
        {
            if (gameObject.CompareTag("Left"))
            {
                PlayerCntrl.LeftView();


            }
            else if (gameObject.CompareTag("Right") && !PlayerCntrl.atDoor)
            {
                PlayerCntrl.RightView();


            }
        }
    }

    private void OnMouseDown()
    {
            if (gameObject.CompareTag("Bottom") && PlayerCntrl.atDoor)
            {
                PlayerCntrl.MoveBack();
            }
            else if(gameObject.CompareTag("Right") && PlayerCntrl.atDoor &&!PlayerCntrl.isMoving)
            {
                if (PlayerCntrl.InSub)
                {
                PlayerCntrl.MoveBackToDoor();
                }
                else if (!PlayerCntrl.InSub)
            
                { PlayerCntrl.MoveToSub(); }
            }
            else if (gameObject.CompareTag("Bottom") && PlayerCntrl.isFront)
            {
                PlayerCntrl.Hide();
            }
            else if (PlayerCntrl.Hiding)
            {
                PlayerCntrl.HideCancel();
            }
    }


    // Update is called once per frame
    void Update()
    {

        


    }
}
