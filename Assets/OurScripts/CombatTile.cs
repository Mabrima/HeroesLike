using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatTile : PathfindingTile
{
    public Material potentialMaterial;
    public Material fakePotentialMaterial;
    public Material overLappingPotentialMaterial;

    private bool selectable = false;
    public UnitHandler unitOnTile;

    private void Start()
    {
        base.Initiate();
        defaultMaterial = rend.material;
    }

    public void SetPositionValues(int x, int y)
    {
        tileX = x;
        tileY = y;
    }

    public override void OnMouseEnter()
    {
        if (!selectable)
            return;
        rend.material = mouseOverMaterial;
    }

    public override void OnMouseExit()
    {
        if (!selectable)
        {
            return;
        }
        rend.material = potentialMaterial;
    }

    public override void OnMouseUp()
    {
        if (!selectable)
            return;
        FieldHandler.instance.ResetPossibleLocations();
        if (fieldType == FieldType.Empty)
        {
            GameManager.instance.currentUnit.Move(this);
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
