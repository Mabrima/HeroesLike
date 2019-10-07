using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{
    public Material selectedMaterial;
    private Material defaultMaterial;
    private MeshRenderer rend;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        defaultMaterial = rend.material;
    }

    private void OnMouseEnter()
    {
        rend.material = selectedMaterial;
    }

    private void OnMouseExit()
    {
        rend.material = defaultMaterial;
    }


}
