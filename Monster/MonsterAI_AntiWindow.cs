using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI_AntiWindow : MonoBehaviour
{
    public float BlockCounter;
    public bool isBlocking;

    private MonsterAi_Spawn spawnManager;
    private LightCntrl lightCntrl;
    private WindowCntrl windowCntrl;

    private Transform PlayerView;
    

    // Start is called before the first frame update
    void Start()
    {
        BlockCounter = 0;

        spawnManager = GameObject.Find("MonsterD").GetComponent<MonsterAi_Spawn>();
        lightCntrl = GameObject.Find("LightPointer").GetComponent<LightCntrl>();
        windowCntrl = GameObject.Find("Window").GetComponent<WindowCntrl>();
        PlayerView = GameObject.Find("Player").transform;


        StartCoroutine(Attack());
    }




    IEnumerator Attack()
    {
        float RandomTime = Random.Range(1, 3);
        while (true)
        {
            if (BlockCounter >= RandomTime)
            {
                EnemyLeave();
                yield return new WaitForSeconds(0.05f);
                lightCntrl.isError = false;
                spawnManager.enemyCount -= 1;
                Destroy (gameObject);
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }
    

    private void OnMouseOver()
    {
        LightOnMonster();
    }

    


    void LightOnMonster()
    {
        if (lightCntrl.isLightOn)
        {
            isBlocking = true;
            BlockCounter += Time.deltaTime;
        }
        else {isBlocking=false; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weaker"))
        {
            if (other.transform.GetInstanceID() < gameObject.transform.GetInstanceID())
            {
                Destroy(other.gameObject);
                spawnManager.enemyCount -= 1;
            }
        }
    }


    public void EnemyLeave()
    {
        if (gameObject.CompareTag("Stronger"))
        {
            spawnManager.AtWindow =false;
        }
        StartCoroutine(lightCntrl.LightError(2));
    }

    void FaceCamera()
    {
        Vector3 lookDirection = PlayerView.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    // Update is called once per frame
    void Update()
    {
        FaceCamera();
    }

}