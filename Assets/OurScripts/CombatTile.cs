using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatTile : MonoBehaviour
{
    public FieldType fieldType = FieldType.Empty;
    public Material potentialMaterial;
    public Material fakePotentialMaterial;
    public Material overLappingPotentialMaterial;
    public Material selectedMaterial;
    private Material defaultMaterial;
    private MeshRenderer rend;
    private bool selectable = false;
    public int x;
    public int y;
    public UnitHandler unitOnTile;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        defaultMaterial = rend.material;
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
        {
            return;
        }
        rend.material = potentialMaterial;
    }

    private void OnMouseUp()
    {
        if (!selectable)
            return;
        FieldHandler.instance.ResetPossibleLocations();
        if (fieldType == FieldType.Empty)
        {
            GameManager.instance.currentUnit.Move(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        }
        else if (fieldType == FieldType.Occupied)
        {
            GameManager.instance.CombatAttack(unitOnTile);
        }

    }

    //Fake means it can't actually be clicked, just visual.
    public void SetAsSelectable(bool fake = false)
    {
        if (!fake)
        {
            selectable = true;
            rend.material = potentialMaterial;
        }
        else if (fake && selectable == true)
        {
            rend.material = overLappingPotentialMaterial;
        }
        else if (fake)
        {
            rend.material = fakePotentialMaterial;
        }
    }

    //Fake removes only the "fake" ones created.
    public void SetAsNotSelectable(bool fake = false)
    {
        if (fake && selectable)
        {
            rend.material = potentialMaterial;
            return;
        }
        selectable = false;
        rend.material = defaultMaterial;
    }

    public void PutUnitOnTile(UnitHandler unit)
    {
        unitOnTile = unit;
        fieldType = FieldType.Occupied;
    }

    public void RemoveUnitFromTile()
    {
        unitOnTile = null;
        fieldType = FieldType.Empty;
    }


}
