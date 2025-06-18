using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmAnimation : MonoBehaviour
{
    private float axisX;
    private float axisY;
    private float axisZ;

    private MonsterAi_Spawn Spawner;

    // Start is called before the first frame update
    void Start()
    {
       // gameObject.SetActive(false);
        Spawner = GameObject.Find("MonsterD").GetComponent<MonsterAi_Spawn>();
        axisY = -0.12f;
        axisZ = 0.45f;
        if (gameObject.CompareTag("Left"))
        {
            axisX = -7.4f;
        }
        else
        {
            axisX = 7.4f;
        }

        transform.localPosition = new Vector3(axisX, axisY, axisZ);
        StartCoroutine(Appearing());

    }

    public IEnumerator Animation()
    {
        float OrgX = transform.localPosition.x;
        float currentTime = 0;
        float Duration = 3;
        float EndX;
        if (gameObject.CompareTag("Left"))
        {
            EndX = axisX + 2f;
        }
        else
        {
            EndX = axisX - 2f;
        }

        while (currentTime < Duration)
        {
            currentTime += Time.deltaTime;
            float newX = Mathf.Lerp(OrgX, EndX, currentTime/Duration);
            transform.localPosition = new Vector3 (newX, axisY, axisZ);
            yield return null;
        }
        transform.localPosition = new Vector3(EndX, axisY, axisZ);
        Spawner.Death();
        yield break;
    }

    public IEnumerator Appearing()
    {
        gameObject.SetActive(true);
        float Duration = 0;
        float OrgX = transform.localPosition.x;
        float EndX;
        if (gameObject.CompareTag("Left"))
        {
            EndX = axisX + 0.6f;
        }
        else
        {
            EndX = axisX - 0.6f;
        }

        while (Duration<60)
        {
            Duration += Time.deltaTime;
            float newX = Mathf.Lerp(OrgX, EndX, Duration/60);
            transform.localPosition = new Vector3(newX, axisY, axisZ);
            if (Spawner.enemyCount <= 2)
            {
                StartCoroutine(Leaving());
                yield break;
            }
            yield return null;
        }
        StartCoroutine(Animation());
        yield break;

    }

    public IEnumerator Leaving()
    {
        gameObject.SetActive(true);
        float Duration = 0;
        float OrgX = transform.localPosition.x;
        float EndX = axisX;

        while (Duration < 2)
        {
            Duration += Time.deltaTime;
            float newX = Mathf.Lerp(OrgX, EndX, Duration / 2);
            transform.localPosition = new Vector3(newX, axisY, axisZ);
            yield return null;
        }
        transform.localPosition = new Vector3(EndX, axisY, axisZ);
        yield break;

    }
}
