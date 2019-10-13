using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitHandler : MonoBehaviour
{
    public UnitBase unitBase;
    public int amountOfUnits = 1;
    public int currentInitiative;
    public int totalHealth;
    // Start is called before the first frame update
    void Start()
    {
        totalHealth = amountOfUnits * unitBase.baseHealth;
    }

    private void OnMouseEnter()
    {
        FieldHandler.instance.GetAvailableMovementTiles(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), unitBase.speed, true);
    }

    private void OnMouseExit()
    {
        FieldHandler.instance.ResetPossibleLocations(true);
    }

    public void Move(int x, int y)
    {
        FieldHandler.instance.RemoveUnitFromTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        transform.position = new Vector3(x, 1, y);
        FieldHandler.instance.ResetPossibleLocations();
        FieldHandler.instance.PutUnitOnTile(x, y, this);
        FieldHandler.instance.GetAvailableAttackTiles(x, y);
        GameManager.instance.SetCannotWait(true);
    }

    public void GetAvailableMovementTiles()
    {
        FieldHandler.instance.ResetPossibleLocations();
        FieldHandler.instance.GetAvailableMovementTiles(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), unitBase.speed);
        FieldHandler.instance.GetAvailableAttackTiles(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }

    public void GetHit(int otherAttack, int otherDamage)
    {
        GetComponent<MeshRenderer>().material = unitBase.hurtMaterial;
        totalHealth -= unitBase.CalculateDamage(otherAttack, otherDamage);
        if (totalHealth < 0)
        {
            FieldHandler.instance.RemoveUnitFromTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(RevertMaterial());
        }
    }

    private IEnumerator RevertMaterial()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<MeshRenderer>().material = unitBase.normalMaterial;
    }

    public void ForwardInitiative()
    {
        currentInitiative += unitBase.initiative;
    }

}
