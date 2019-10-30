using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatTile : MonoBehaviour
{
    [SerializeField] Material potentialMaterial;
    [SerializeField] Material fakePotentialMaterial;
    [SerializeField] Material overLappingPotentialMaterial;
    [SerializeField] Material mouseOverMaterial;
    Material defaultMaterial;

    public CombatTile pfParent;
    public CombatTile perishableParent;

    public Vector2Int position;

    public FieldType fieldType;
    public MeshRenderer rend;

    private bool selectable = false;
    public UnitHandler unitOnTile;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        defaultMaterial = rend.material;
    }

    public void SetPositionValues(int x, int y)
    {
        position.x = x;
        position.y = y;
    }

    public void OnMouseEnter()
    {
        if (!selectable)
            return;
        rend.material = mouseOverMaterial;
    }

    public void OnMouseExit()
    {
        if (!selectable)
        {
            return;
        }
        rend.material = potentialMaterial;
    }

    public void OnMouseUp()
    {
        if (!selectable)
            return;
        FieldHandler.instance.ResetSelectableLocations();
        if (fieldType == FieldType.Empty)
        {
            CombatManager.instance.currentUnit.Move(this);
        }
        else if (fieldType == FieldType.Occupied)
        {
            CombatManager.instance.StartCombatAttack(unitOnTile);
        }

    }

    //"Fake" means it can't actually be clicked, just visual.
    public void SetAsSelectable(bool fake)
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
    public void SetAsNotSelectable(bool fake)
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
