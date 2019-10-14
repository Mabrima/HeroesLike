using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class UnitHandler : MonoBehaviour
{
    public UnitBase unitBase;
    public int amountOfUnits = 1;
    public int currentInitiative;
    public int totalHealth;
    public int currentUnitHealth;
    public int team = 1;
    [SerializeField] Text amountText;
    // Start is called before the first frame update
    void Start()
    {
        currentUnitHealth = unitBase.baseHealth;
        totalHealth = amountOfUnits * unitBase.baseHealth;
        amountText.text = "" + amountOfUnits;
    }

    private void OnMouseEnter()
    {
        FieldHandler.instance.GetAvailableMovementTiles(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), unitBase.speed, true);
    }

    private void OnMouseExit()
    {
        FieldHandler.instance.ResetPossibleLocations(true);
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            GameManager.instance.statViewer.UpdateStats(currentUnitHealth, unitBase.baseHealth, unitBase.attack, unitBase.defence, unitBase.damage, unitBase.damage, unitBase.speed, unitBase.initiative);
            GameManager.instance.statViewer.Show();
        }
    }

    public void Move(int x, int y)
    {
        FieldHandler.instance.RemoveUnitFromTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        transform.position = new Vector3(x, 1, y);
        FieldHandler.instance.ResetPossibleLocations();
        FieldHandler.instance.PutUnitOnTile(x, y, this);
        FieldHandler.instance.GetAvailableAttackTiles(x, y, team);
        GameManager.instance.SetCannotWait(true);
    }

    public void GetAvailableMovementTiles()
    {
        FieldHandler.instance.ResetPossibleLocations();
        FieldHandler.instance.GetAvailableMovementTiles(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), unitBase.speed);
        FieldHandler.instance.GetAvailableAttackTiles(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), team);
    }

    public void GetHit(int otherAttack, int otherDamage, int otherAmountOfUnits)
    {
        int damageSustained = unitBase.CalculateDamage(otherAttack, otherDamage * otherAmountOfUnits);
        int amountKilled = 0;
        totalHealth -= damageSustained;
        GameManager.instance.battleText.text = unitBase.name + " took " + damageSustained + " damage";
        if (totalHealth <= 0)
        {
            FieldHandler.instance.RemoveUnitFromTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            GameManager.instance.battleText.text += "\n All units of " + unitBase.name + " died";
            Destroy(this.gameObject);
            return;
        }

        //calculates how many units die and how much health is left on the top unit.
        amountKilled = Mathf.Max(damageSustained, 0) / unitBase.baseHealth;
        amountOfUnits -= amountKilled;
        currentUnitHealth = currentUnitHealth - damageSustained % unitBase.baseHealth;
        if (currentUnitHealth <= 0)
        {
            currentUnitHealth = unitBase.baseHealth + currentUnitHealth;
            amountKilled++;
            amountOfUnits--;
        }
        amountText.text = "" + amountOfUnits;

        GameManager.instance.battleText.text += "\n" + amountKilled + " units died";

    }

    public void ForwardInitiative()
    {
        currentInitiative += unitBase.initiative;
    }

}
