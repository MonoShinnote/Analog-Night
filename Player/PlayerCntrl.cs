using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCntrl : MonoBehaviour
{ 
    private  bool UiLeft;
    private  bool UiRight;
    
    public bool isRotating;
    public bool isLeft;
    public bool isFront;
    public bool isRight;
    public bool atDoor;
    public float rotateAmount;
    public int CamRotate;
    public bool DoorClose;
    public bool DoorForceOpen;
    public bool CurtainClose;
    public bool InSub;
    public bool Hiding;
    public bool isCasting;
    public bool isCamOn;
    public bool isMoving;
    
    public float rotationSpeed ;
    public float movementSpeed;
    public Vector3 CurrentPosition;
    public Vector3 CurrentRotation;

    public Vector3 mousePos;

    public Vector3 StartView;
    public Vector3 UnderDesk;
    public Vector3 DoorPoint;
    public Vector3 OtherDoorPoint;

    public CamCntrl camCntrl;

    public AudioClip[] WalkSound;
    public AudioClip[] WalkBack;

    private AudioSource PlayerAudio;
    private CanvaManager canvaManager;
    // Start is called before the first frame update
    void Start()
    {
        camCntrl = GameObject.Find("Cam").GetComponent<CamCntrl>();

        rotationSpeed = 60;
        movementSpeed = 60;

        StartView = new Vector3(-4, 8.5f, 1.7f);
        UnderDesk = new Vector3(-4, 2.4f, 8.7f);
        DoorPoint = new Vector3(26.7f, 10, 4.45f);
        OtherDoorPoint = new Vector3(25, 8, -4f);
        transform.position = StartView;

        transform.rotation = Quaternion.identity;
        CamRotate = 0;
        atDoor = false;

        PlayerAudio = GetComponent<AudioSource>();
        canvaManager = GameObject.Find("CanvaManager").GetComponent<CanvaManager>();
    }
    

IEnumerator RotateLeft()
    {
        rotateAmount = 0;
        while (rotateAmount < 18)
        {
            isRotating = true;
            rotateAmount += 1;
            transform.Rotate(0, -5, 0);

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        isRotating = false;
    }

    IEnumerator RotateRight()
    {
        
        rotateAmount = 0;
        while (rotateAmount < 18)
        {
            isRotating = true;
            rotateAmount += 1;
            transform.Rotate(0, 5, 0);

            yield return null;
        }
        
        yield return new WaitForSeconds(0.02f);
        isRotating = false;

    }

    public void LeftView()
    {
        if (!isRotating) 
        {
            StartCoroutine(RotateLeft());
            CamRotate -= 1;
        }
        
    }

    public void RightView()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateRight());
            CamRotate += 1;
        }
    }

    //Move Toward Door
    public void MoveToDoor()
    {
        if (!isMoving && !isRotating)
        {
            StartCoroutine(MoveToDoorAnimation());
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            PlayerAudio.PlayOneShot(WalkSound[Random.Range(0, 2)], 1);
            isMoving = true;
        }
    }
    IEnumerator MoveToDoorAnimation()
    {
        while (Vector3.Distance(transform.position, DoorPoint) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, DoorPoint, movementSpeed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
        atDoor = true;
        InSub = false;
        yield break;
    }

    //Exit Ritual Room
    public void MoveBackToDoor()
    {
        StartCoroutine(MoveBackToDoorAnimation());
        isMoving = true;
    }
    IEnumerator MoveBackToDoorAnimation()
    {
        //StartCoroutine(RotateRight());
        StartCoroutine(canvaManager.FadingBlack());
        yield return new WaitForSeconds(1);

        transform.position = DoorPoint;
        transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        isMoving = false;
        atDoor = true;
        InSub = false;

        StartCoroutine(canvaManager.FadingBack());
        yield break;

    }

    //Go to Ritual Room
    public void MoveToSub()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveToSubAnimation());
            isMoving = true;
        }
    }
    IEnumerator MoveToSubAnimation()
    {
        StartCoroutine(canvaManager.FadingBlack());
        yield return new WaitForSeconds(1);

        transform.position = OtherDoorPoint;
        transform.rotation = Quaternion.Euler(new Vector3(30, 270, 0));

        isMoving = false;
        InSub = true;
        StartCoroutine(canvaManager.FadingBack());
        yield break;
    }

    //Back to starting point
    public void MoveBack()
    {
        PlayerAudio.PlayOneShot(WalkBack[Random.Range(0,2)],1);
        StartCoroutine(MoveBackAnimation());
        isMoving = true;
    }
    IEnumerator MoveBackAnimation()
    {
        StartCoroutine(canvaManager.FadingBlack());
        yield return new WaitForSeconds(1);

        transform.position = StartView;
        transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        isMoving = false;
        atDoor = false;
        Hiding = false;
        StartCoroutine(canvaManager.FadingBack());
        yield break;
    }

    //Out of the desk
    public void HideCancel()
    {
        StartCoroutine(HideCancelAnimation());
        isMoving = true;
    }
    IEnumerator HideCancelAnimation()
    {
        StartCoroutine(canvaManager.FadingBlack());
        yield return new WaitForSeconds(1);

        transform.position = StartView;
        transform.rotation = Quaternion.identity;

        isMoving = false;
        atDoor = false;
        Hiding = false;
        StartCoroutine(canvaManager.FadingBack());
        yield break;
    }

    //Hide Under the desk
    public void Hide()
    {
        StartCoroutine(HideAnimation());
        isMoving = true;
    }
    IEnumerator HideAnimation()
    {
        StartCoroutine(canvaManager.FadingBlack());
        yield return new WaitForSeconds(1);

        transform.position = UnderDesk;
        transform.rotation = Quaternion.Euler(new Vector3(0, 169, 0));

        isMoving = false;
        Hiding = true;
        StartCoroutine(canvaManager.FadingBack());
        yield break;
    }

    void IsHiding()
    {
        
        mousePos = Input.mousePosition;

        float xRatio = Input.mousePosition.x/ Screen.width;

        float RotationY = 0;
        CurrentRotation= transform.eulerAngles;

        if (xRatio <= 0.8f && CurrentRotation.y >= 120)
        {
            RotationY = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, -RotationY, 0);
        }

        if (xRatio >= 0.2f && CurrentRotation.y < 170)
        {
            RotationY = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, RotationY, 0);
        }
      
    }

    // Update is called once per frame
    //Track Player Position
    void Update()
    {
        CurrentPosition = transform.position;

       isCamOn = camCntrl.isOn;

        if (CamRotate == 0)
        {
            isFront = true;
            isLeft = false;
            isRight = false;
        }
        else if (CamRotate < 0)
        {
            isFront = false;
            isLeft = true;
            isRight = false;
        }
        else if (CamRotate > 0)
        {
            isFront = false;
            isLeft = false;
            isRight = true;
        }

        if (Hiding)
        {
            IsHiding();
        }
        
    }
   

}
