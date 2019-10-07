using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{
    public Material potentialMaterial;
    public Material selectedMaterial;
    private Material defaultMaterial;
    private MeshRenderer rend;
    private bool selectable = true;
    public int x;
    public int y;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        defaultMaterial = rend.material;
        rend.material = potentialMaterial;
    }

    public void SetPositionValues(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    private void OnMouseEnter()
    {
        if (!selectable)
            return;
        rend.material = selectedMaterial;
    }

    private void OnMouseExit()
    {
        if (!selectable)
            return;
        rend.material = potentialMaterial;
    }

    private void OnMouseUp()
    {
        if (!selectable)
            return;
        FieldHandler.instance.ResetPossibleLocations();
        GameManager.instance.currentUnit.Move(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }

    public void SetAsSelectable()
    {
        selectable = true;
        rend.material = potentialMaterial;
    }

    public void SetAsNotSelectable()
    {
        selectable = false;
        rend.material = defaultMaterial;
    }


}
