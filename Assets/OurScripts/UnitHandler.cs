using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHandler : MonoBehaviour
{
    public UnitBase unitBase;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Move(int x, int y)
    {
        FieldHandler.instance.RemoveUnitFromTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        transform.position = new Vector3(x, 1, y);
        FieldHandler.instance.GetAvailableAttackTiles(x, y, 5);
        FieldHandler.instance.PutUnitOnTile(x, y, this);
        GameManager.instance.SetCannotWait();
    }

    public void GetAvailableMovementTiles()
    {
        FieldHandler.instance.GetAvailableMovementTiles(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), unitBase.speed);
    }

    public void GetHit(int otherAttack, int otherDamage)
    {
        unitBase.CalculateDamage(otherAttack, otherDamage);
        if (unitBase.health < 0)
        {
            Destroy(this.gameObject);
        }
    }


}
