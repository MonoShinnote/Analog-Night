using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCntrl : MonoBehaviour
{
    private PlayerCntrl PlayerCntrl;

    public Material OrgMat;
    public Material GlowMat;
    private Renderer ObjRenderer;


    // Start is called before the first frame update
    void Start()
    {
        PlayerCntrl = GameObject.Find("Player").GetComponent<PlayerCntrl>();
        ObjRenderer = GetComponent<Renderer>();
        ObjRenderer.material = OrgMat;
    }

    private void OnMouseDown()
    {
        PlayerCntrl.isCasting = true;
        ObjRenderer.material = GlowMat;
    }

    private void OnMouseUp()
    {
        PlayerCntrl.isCasting = false;
        ObjRenderer.material = OrgMat;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
