using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    Animator animator;
    float startTime;
    [SerializeField] float unitSpeed = 10;

    void Start()
    {
        currentUnitHealth = unitBase.baseHealth;
        totalHealth = amountOfUnits * unitBase.baseHealth;
        amountText.text = "" + amountOfUnits;
        //Invoke("FindFirstTile", .5f);
        animator = GetComponentInChildren<Animator>();
    }

    public void TriggerAbilitiesAtTiming(AbilityTiming timing, UnitHandler unitToTriggerOn)
    {
        foreach(AbilityBase ability in unitBase.abilities)
        {
            if (ability.timing == timing)
            {
                ability.TriggerAbility(unitToTriggerOn);
            }
        }
    }

    void FindFirstTile()
    {
        currentTile = (CombatTile)FieldHandler.instance.GetTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        currentTile.PutUnitOnTile(this);
    }

    private void OnMouseUp()
    {
        currentTile.OnMouseUp();
    }

    private void OnMouseEnter()
    {
        if (!unitBase.flyer)
            FieldHandler.instance.GetAvailableMovementTiles(currentTile, unitBase.speed, true);
        else
            FieldHandler.instance.FlyGetAvailableMovementTiles(currentTile, unitBase.speed, true);
        currentTile.OnMouseEnter();
    }

    private void OnMouseExit()
    {
        FieldHandler.instance.ResetSelectableLocations(true);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CombatManager.instance.statViewer.UpdateStats(currentUnitHealth, unitBase.baseHealth, unitBase.attack, unitBase.defence, unitBase.minDamage, unitBase.maxDamage, unitBase.speed, unitBase.initiative);
            CombatManager.instance.statViewer.Show();
        }
    }

    public void Move(CombatTile tile)
    {
        FieldHandler.instance.RemoveUnitFromTile(currentTile);
        currentTile = tile;
        FieldHandler.instance.PutUnitOnTile(currentTile, this);

        animator.SetBool("Moving", true);

        startTime = Time.time;
        StartCoroutine(MoveUnit());
        CombatManager.instance.SetCannotWait(true);
    }

    private void ResetAbilities()
    {
        foreach(AbilityBase ability in unitBase.abilities)
        {
            ability.used = false;
        }
    }

    public void GetAvailableMovementTiles()
    {
        ResetAbilities();
        TriggerAbilitiesAtTiming(AbilityTiming.StartOfTurn, this);

        FieldHandler.instance.ResetSelectableLocations();
        FieldHandler.instance.ClearParents();

        FieldHandler.instance.GetAvailableMovementTiles(currentTile, unitBase.speed);
        FieldHandler.instance.GetAvailableAttackTiles(currentTile, team);
    }

    public void GetHit(int otherAttack, int otherMinDamage, int otherMaxDamage, int otherAmountOfUnits)
    {
        TriggerAbilitiesAtTiming(AbilityTiming.DuringDefence, this);
        int damageRolled = Random.Range(otherMinDamage * otherAmountOfUnits, (otherMaxDamage * otherAmountOfUnits) + 1);
        int damageSustained = unitBase.CalculateDamage(otherAttack,  damageRolled);
        int amountKilled = 0;
        totalHealth -= damageSustained;
        CombatManager.instance.battleText.text += '\n' + unitBase.name + " " + team + " took " + damageSustained + " damage";
        TriggerAbilitiesAtTiming(AbilityTiming.AfterDefence, this);
        if (totalHealth <= 0)
        {
            animator.SetBool("Death", true);
            FieldHandler.instance.RemoveUnitFromTile(currentTile);
            CombatManager.instance.battleText.text += "\n All units of " + unitBase.name + " died";
            canRetaliate = false;
            CombatManager.instance.RemoveUnit(this);
            Invoke("KillThis", 1);
            return;
        }

        animator.SetTrigger("Hit");
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

        CombatManager.instance.battleText.text += "\n" + amountKilled + " units died";
    }

    private void KillThis()
    {
        Destroy(this.gameObject);
    }

    public void ForwardInitiative()
    {
        currentInitiative += unitBase.initiative;
    }

    public void InitiativeWait()
    {
        currentInitiative = 50;
    }

    IEnumerator MoveUnit()
    {
        Stack<CombatTile> stack = new Stack<CombatTile>();
        CombatTile parent = currentTile;
        while (parent != null)
        {
            stack.Push(parent);
            parent = parent.pfParent;
        }

        CombatTile nextMove;

        while (stack.Count > 0) //Go through stack of the path until its empty.
        {
            nextMove = stack.Pop(); //Get next tile in stack we want to move too.
            startTime = Time.time;
            float journeyLength = Vector3.Distance(transform.position, nextMove.transform.position);
            //RotateUnitToTile(nextMove);
            bool lerping = true;

            while (lerping)
            {
                float distCovered = (Time.time - startTime) * unitSpeed;
                float fractionOfJourney = distCovered / journeyLength;
                if (fractionOfJourney > 0.9)
                {
                    lerping = false;
                }

                transform.position = Vector3.Lerp(transform.position, new Vector3(nextMove.transform.position.x, 1, nextMove.transform.position.z), fractionOfJourney);

                yield return new WaitForFixedUpdate();
            }

        }

        animator.SetBool("Moving", false);
        FieldHandler.instance.GetAvailableAttackTiles(currentTile, team);
        Debug.Log("moving ended");
    }

    public void RotateUnitToTile(CombatTile nextMove)
    {
        Quaternion newRotation = Quaternion.LookRotation(transform.position - nextMove.transform.position, Vector3.forward);
        newRotation.x = 0f;
        newRotation.y = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Mathf.Infinity);
    }

    public void SetAnimatorAttacking()
    {
        animator.SetTrigger("Attack");
    }

}
