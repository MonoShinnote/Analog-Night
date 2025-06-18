using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    private PlayerCntrl playerCntrl;
    private MonsterManager monsterManager;
    [SerializeField] private GameObject[] GuideList;
    [SerializeField] private GameObject replacer;
    private int currentList;

    private bool isHidden;

    // Start is called before the first frame update
    void Start()
    {
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        monsterManager = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();

        isHidden = PlayerPrefs.GetInt("HidGuide", 0) == 1;
        currentList = 6;

        if (isHidden || monsterManager.isInfi || monsterManager.isNightmare)
        {
            HideGuide();
        }

    }

    public void ShowGuide()
    {
        for (int i = 0; i < GuideList.Length; i++)
        {
            GuideList[i].SetActive(false);
        }
        GuideList[currentList].SetActive(true);
    }
    private void HideGuide()
    {
        for(int i = 0;i < GuideList.Length; i++)
        {
            GuideList[i] = replacer;
        }
    }
    public void CloseGuide()
    {
        GuideList[currentList].SetActive(false);
        GuideList[currentList] = replacer;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCntrl.isFront)
        {
            currentList = 0;
            if (playerCntrl.Hiding)
            {
                currentList = 3;
            }
        }
        else if (playerCntrl.isLeft)
        {
            currentList = 1;
        }
        else if (playerCntrl.isRight)
        {
            currentList = 2;
            if (playerCntrl.atDoor)
            {
                currentList = 4;
                if (playerCntrl.InSub)
                {
                    currentList = 5;
                }
            }
        }
        else
        {
            currentList = 6;
        }
        ShowGuide();
    }
}
