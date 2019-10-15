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
    public CombatTile currentTile;
    public int team = 1;
    public bool canRetaliate = true;
    [SerializeField] Text amountText;
    // Start is called before the first frame update
    void Start()
    {
        currentUnitHealth = unitBase.baseHealth;
        totalHealth = amountOfUnits * unitBase.baseHealth;
        amountText.text = "" + amountOfUnits;
        Invoke("FindFirstTile", .1f);
    }
    
    void FindFirstTile()
    {
        currentTile = (CombatTile) FieldHandler.instance.GetPathfindingTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        currentTile.PutUnitOnTile(this);
    }

    private void OnMouseUp()
    {
        currentTile.OnMouseUp();
    }

    private void OnMouseEnter()
    {
        FieldHandler.instance.GetAvailableMovementTiles(currentTile, unitBase.speed, true);
        currentTile.OnMouseEnter();
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

    public void Move(CombatTile tile)
    {
        FieldHandler.instance.RemoveUnitFromTile(currentTile);
        currentTile = tile;
        transform.position = new Vector3(currentTile.tileX, 1, currentTile.tileY);
        FieldHandler.instance.ResetPossibleLocations();
        FieldHandler.instance.PutUnitOnTile(currentTile, this);
        FieldHandler.instance.GetAvailableAttackTiles(currentTile, team);
        GameManager.instance.SetCannotWait(true);
    }

    public void GetAvailableMovementTiles()
    {
        FieldHandler.instance.ResetPossibleLocations();
        FieldHandler.instance.GetAvailableMovementTiles(currentTile, unitBase.speed);
        FieldHandler.instance.GetAvailableAttackTiles(currentTile, team);
    }

    public void GetHit(int otherAttack, int otherDamage, int otherAmountOfUnits)
    {
        int damageSustained = unitBase.CalculateDamage(otherAttack, otherDamage * otherAmountOfUnits);
        int amountKilled = 0;
        totalHealth -= damageSustained;
        GameManager.instance.battleText.text += '\n' + unitBase.name + " " + team + " took " + damageSustained + " damage";
        if (totalHealth <= 0)
        {
            FieldHandler.instance.RemoveUnitFromTile(currentTile);
            GameManager.instance.battleText.text += "\n All units of " + unitBase.name + " died";
            canRetaliate = false;
            GameManager.instance.RemoveUnit(this);
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

    public void InitiativeWait()
    {
        currentInitiative = 50;
    }

}
