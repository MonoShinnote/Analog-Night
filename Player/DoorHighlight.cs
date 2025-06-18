using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHighlight : MonoBehaviour
{
    [SerializeField] private Material OrgMaterial;
    [SerializeField] private Material Highlight;

    private Renderer MTrenderer;

    private PlayerCntrl playerCntrl;

    // Start is called before the first frame update
    void Start()
    {
        playerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();

        MTrenderer = GetComponent<Renderer>();
        MTrenderer.material = OrgMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCntrl.DoorClose)
        {
            MTrenderer.material = Highlight;
        }
        else
        {
            MTrenderer.material = OrgMaterial;
        }
    }
}
