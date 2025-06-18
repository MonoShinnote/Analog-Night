using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCntrl : MonoBehaviour
{
    private PlayerCntrl PlayerCntrl;

    public bool isMouseHold;
    public bool doorKill;

    public float TotalTime = 5;

    [SerializeField] private AudioClip[] DoorOpen;
    [SerializeField] private AudioClip DoorClosing;
    [SerializeField] private AudioClip DoorClosed;
    [SerializeField] private AudioClip DoorLock;
    [SerializeField] private AudioClip DoorBreak;
    [SerializeField] private AudioClip DoorKnock;

    private AudioSource DoorAudio;

    // Start is called before the first frame update
    void Start()
    {
        PlayerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();

        PlayerCntrl.DoorForceOpen = false;

        DoorAudio = GetComponent<AudioSource>();

        doorKill = false;
    }

    private void OnMouseDown()
    {
        if (PlayerCntrl.isRight && !PlayerCntrl.atDoor && (gameObject.CompareTag("DoorBox")|| (gameObject.CompareTag("MainDoor"))))
        { PlayerCntrl.MoveToDoor(); }

        else if (PlayerCntrl.atDoor && gameObject.CompareTag("MainDoor") && PlayerCntrl.DoorForceOpen && !PlayerCntrl.isMoving)
        {
                StartCoroutine(CloseDoor());
        }

        else if (PlayerCntrl.atDoor && gameObject.CompareTag("MainDoor") && !PlayerCntrl.DoorForceOpen && !PlayerCntrl.isMoving)
        {
            DoorAudio.PlayOneShot(DoorLock, 1);
            isMouseHold = true;
            PlayerCntrl.DoorClose = true;
        }
    }


    private void OnMouseUp()
    {
        isMouseHold = false;
        PlayerCntrl.DoorClose = false;
    }

    public void DoorKnocking()
    {
        DoorAudio.PlayOneShot(DoorKnock, 1);
    }

    public IEnumerator DoorAnimation()
    {
        if (gameObject.CompareTag("MainDoor"))
        {
            float rotateAmount = 0;
            PlayerCntrl.DoorForceOpen = true;
            DoorAudio.PlayOneShot(DoorOpen[Random.Range(0, 2)], 1);
            while (rotateAmount < 85)
            {
                rotateAmount += 1;
                transform.Rotate(0, 1, 0);
                yield return new WaitForSeconds(0.05f);
            }
        }
        yield break;
    }
    public IEnumerator DoorDestroy()
    {
        if (gameObject.CompareTag("MainDoor"))
        {

            float rotateAmount = 0;
            while (rotateAmount < 85)
            {
                rotateAmount += 1;
                transform.Rotate(0, 1, 0);
                yield return new WaitForSeconds(0.01f);
            }
            DoorAudio.PlayOneShot(DoorBreak, 1);
            PlayerCntrl.DoorForceOpen = true;
        }
        yield break;

    }

    public IEnumerator CloseDoor()
    {
        PlayerCntrl.DoorForceOpen = false;

        if (doorKill)
        {
            yield break;
        }
        float rotateAmount = 0;
        DoorAudio.PlayOneShot(DoorClosing,1);
        while (rotateAmount < 85)
        {
            rotateAmount += 1;
            transform.Rotate(0, -1, 0);
            yield return null;
        }
        DoorAudio.PlayOneShot(DoorClosed, 1);
        yield break;

    }

}
