using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticFx : MonoBehaviour
{
    public Color color;
    public RawImage StaticImage;

    public MonsterAI_Phase monsterB;


    // Start is called before the first frame update
    void Start()
    {
        color.a = 0f;
        monsterB = GameObject.Find("test monster").GetComponent<MonsterAI_Phase>();
        StaticImage = GetComponent<RawImage>();
    }

    public IEnumerator AlphaChange()
    {
        while (color.a < 255)
        {
            color.a += 1;
            yield return new WaitForSeconds(1);
        }
        color.a = 200f;
        StartCoroutine(AlphaChange2());
        yield break;
    }

    IEnumerator AlphaChange2()
    {
        while(color.a < 255)
        {
            color.a += 1;
                yield return new WaitForSeconds(1);
        }
        StartCoroutine(AlphaReset());
        yield break;
    }

    IEnumerator AlphaReset()
    {
        while (color.a > 0)
        {
            color.a -= 1;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
        StaticImage.color = color;
        
    }
}
