using UnityEngine;
using UnityEngine.UI;

public class HardMode : MonoBehaviour
{
    [SerializeField] private GameObject Off;
    [SerializeField] private GameObject On;
    [SerializeField] private GameObject Off2;
    [SerializeField] private GameObject Off3;
    [SerializeField] private GameObject On2;
    [SerializeField] private GameObject On3;
    [SerializeField] private GameObject Warning;
    [SerializeField] private RawImage  Blue;
    [SerializeField] private Image Black;
    [SerializeField] private Light TvLight;

    private Color OrgColorBlue;
    private Color OrgColorBlack;
    private Color OrgColorTv;
    public bool isHard;
    public bool isHidden;
    public bool isInfi;

    // Start is called before the first frame update
    void Start()
    {
        isHard = false;
        isInfi = false;
        PlayerPrefs.SetInt("HardMode", isHard ? 1 : 0);
        PlayerPrefs.SetInt("InfiMode", isInfi ? 1 : 0);
        PlayerPrefs.Save();
        OrgColorBlue = Blue.color;
        OrgColorBlack = Black.color;
        OrgColorTv = TvLight.color;
    }

    public void OnOff()
    {
        if (!isHard)
        {
            isHard = true;
        }
        else if (isHard)
        {
            isHard = false;
        }
        PlayerPrefs.SetInt("HardMode", isHard ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void InfiOnOff()
    {
        if (!isInfi)
        {
            isInfi = true;
        }
        else if (isInfi)
        {
            isInfi = false;
        }
        PlayerPrefs.SetInt("InfiMode", isInfi ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void NoGuide()
    {
        if (!isHidden)
        {
            isHidden = true;
        }
        else if (isHidden)
        {
            isHidden = false;
        }
        PlayerPrefs.SetInt("HidGuide", isHidden ? 1 : 0);
        PlayerPrefs.Save();
    }

    //Reset all Setting except Guide Skip 
    public void LoadSetting()
    {
        isHidden = PlayerPrefs.GetInt("HidGuide", 0) == 1;
        isHard = false;
        isInfi = false;
    }

    // Update is called once per frame
    void Update()
    {
        Off.SetActive(!isHard);
        On.SetActive(isHard);
        Off2.SetActive(!isHidden);
        On2.SetActive(isHidden);
        Off3.SetActive(!isInfi);
        On3.SetActive(isInfi);
        Warning.SetActive(isHard);

        if (isHard)
        {
            Blue.color = new Color(1, 0, 0, 1);
            Black.color = new Color(1, 0, 0, 0.5f);
            TvLight.color = Color.red;
        }
        else
        {
            Blue.color = OrgColorBlue;
            Black.color = OrgColorBlack;
            TvLight.color = OrgColorTv; 
        }
    }
}
