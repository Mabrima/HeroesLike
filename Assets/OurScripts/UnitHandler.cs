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
    public bool computerControlled = false;
    public bool canRetaliate = true;
    public bool defended = false;
    public Color[] teamColors = { Color.red, Color.cyan };

    [SerializeField] Text amountText;

    Animator animator;
    float startTime;
    [SerializeField] float unitSpeed = 10;

    /// <summary>
    /// Made by Robin Arkblad
    /// </summary>

    void Start()
    {
        currentUnitHealth = unitBase.baseHealth;
        totalHealth = amountOfUnits * unitBase.baseHealth;
        amountText.text = "" + amountOfUnits;
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

    public void Defend()
    {
        defended = true;
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

        startTime = Time.time;
        StartCoroutine(MoveUnit());
        CombatManager.instance.SetCannotDefend(true);
    }

    public void ResetAbilities()
    {
        foreach(AbilityBase ability in unitBase.abilities)
        {
            ability.used = false;
        }
    }

    public void GetAvailableMovementTiles()
    {
        StartOfTurn();
        if (computerControlled)
        {
            ComputerMove();
            return;
        }
        FieldHandler.instance.ResetSelectableLocations();
        FieldHandler.instance.ClearParents();

        FieldHandler.instance.GetAvailableMovementTiles(currentTile, unitBase.speed);
        FieldHandler.instance.GetAvailableAttackTiles(currentTile, team);
    }

    private void StartOfTurn()
    {
        TriggerAbilitiesAtTiming(AbilityTiming.StartOfTurn, this);
        defended = false;
    }

    private void ComputerMove()
    {
        FieldHandler.instance.ClearParents();

        FieldHandler.instance.GetAvailableMovementTiles(currentTile, 22);
        FieldHandler.instance.ResetSelectableLocations();

        CombatTile closestUnitTile = null;
        int distanceToClosestUnit = 999;
        int distanceToThisUnit;
        foreach (UnitHandler unit in CombatManager.instance.unitsInCombat)
        {
            if (unit.team != team)
            {
                distanceToThisUnit = GetDistanceToTile(unit.currentTile);
                if (distanceToThisUnit < distanceToClosestUnit)
                {
                    closestUnitTile = unit.currentTile;
                    distanceToClosestUnit = distanceToThisUnit;
                }
            }
        }

        if (closestUnitTile == null || closestUnitTile.pfParent == null)
        {
            return;
        }
        CombatTile parent = closestUnitTile.pfParent;
        Stack<CombatTile> stack = new Stack<CombatTile>();

        while (parent != null)
        {
            stack.Push(parent);
            parent = parent.pfParent;
        }

        int i = unitBase.speed;
        CombatTile tileToGoTo = null;
        while (stack.Count > 0 && i => 0)
        {
            tileToGoTo = stack.Pop();
            i--;
        }


        FieldHandler.instance.RemoveUnitFromTile(currentTile);
        currentTile = tileToGoTo;
        FieldHandler.instance.PutUnitOnTile(currentTile, this);

        startTime = Time.time;
        StartCoroutine(MoveUnit());
    }

    private int GetDistanceToTile(CombatTile tile)
    {
        int i = 0;
        CombatTile next = tile.pfParent;
        while (next != null)
        {
            i++;
            next = next.pfParent;
        }
        return i;
    }

    public void GetHit(int otherAttack, int otherMinDamage, int otherMaxDamage, int otherAmountOfUnits)
    {
        TriggerAbilitiesAtTiming(AbilityTiming.DuringDefence, this);
        if (defended)
        {
            unitBase.defence =(int) (unitBase.defence * 1.3f);
        }
        int damageRolled = Random.Range(otherMinDamage * otherAmountOfUnits, (otherMaxDamage * otherAmountOfUnits) + 1);
        if (defended)
        {
            unitBase.defence = (int)(unitBase.defence / 1.3f);
        }
        int damageSustained = unitBase.CalculateDamage(otherAttack,  damageRolled);
        int amountKilled = 0;
        totalHealth -= damageSustained;
        CombatManager.instance.battleText.text += '\n' + unitBase.name + " " + team + " took " + damageSustained + " damage";
        if (totalHealth <= 0)
        {
            SetAnimatorDying(true);
            FieldHandler.instance.RemoveUnitFromTile(currentTile);
            CombatManager.instance.battleText.text += "\n All units of " + unitBase.name + " died";
            canRetaliate = false;
            CombatManager.instance.RemoveDeadUnitFromCombat(this);
            Invoke("KillThis", 1);
            return;
        }

        Invoke("TriggerAnimatorDefending", .2f);
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

    IEnumerator MoveUnit()
    {
        SetAnimatorMoving(true);

        Stack<CombatTile> stack = new Stack<CombatTile>();
        CombatTile parent = currentTile;
        CombatManager.instance.SetCannotDefend(true);
        CombatManager.instance.SetCannotEndTurn(true);


        while (parent != null)
        {
            stack.Push(parent);
            parent = parent.pfParent;
        }

        CombatTile nextMove = stack.Pop();


        while (stack.Count > 0) //Go through stack of the path until its empty.
        {
            nextMove = stack.Pop(); //Get next tile in stack we want to move too.
            startTime = Time.time;
            float journeyLength = Vector3.Distance(transform.position, nextMove.transform.position);
            RotateUnitToTile(nextMove);
            bool lerping = true;

            Vector3 startPos = transform.position;
            while (lerping)
            {
                float distCovered = (Time.time - startTime) * unitSpeed;
                float fractionOfJourney = distCovered / journeyLength;
                
                if (fractionOfJourney > 0.99f)
                {
                    lerping = false;
                }

                transform.position = Vector3.Lerp(startPos, new Vector3(nextMove.transform.position.x, .5f, nextMove.transform.position.z), fractionOfJourney);

                yield return new WaitForFixedUpdate();
            }
        }

        SetAnimatorMoving(false);
        RotateTeamDirection(team);
        FieldHandler.instance.GetAvailableAttackTiles(currentTile, team);
        CombatManager.instance.SetCannotEndTurn(false);
        if (computerControlled)
        {
            ComputerChooseAttack();
        }
    }

    private void ComputerChooseAttack()
    {
        List<UnitHandler> units = FieldHandler.instance.GetEnemyUnitsInRange(currentTile, false, team);
        FieldHandler.instance.ResetSelectableLocations();


        if (units.Count > 0)
        {
            CombatManager.instance.StartCombatAttack(units[Random.Range(0, units.Count)]);
        }
        else
        {
            CombatManager.instance.endTurn = true;
        }

    }

    public void RotateUnitToTile(CombatTile nextMove)
    {
        Quaternion newRotation = Quaternion.LookRotation(transform.GetChild(1).position - nextMove.transform.position, Vector3.up);
        newRotation.x = 0f;
        newRotation.z = 0f;
        transform.GetChild(1).rotation = Quaternion.RotateTowards(transform.rotation, newRotation, 360);
        transform.GetChild(1).rotation = transform.GetChild(1).rotation * Quaternion.Euler(0, 180, 0);
    }

    public void RotateTeamDirection(int team)
    {
        if (team == 1)
        {
            transform.GetChild(1).rotation = Quaternion.RotateTowards(transform.rotation, FieldHandler.ROTATION_TEAM_1, 360);
        }
        else if (team == 2)
        {
            transform.GetChild(1).rotation = Quaternion.RotateTowards(transform.rotation, FieldHandler.ROTATION_TEAM_2, 360);
        }
    }

    public void TriggerAnimatorAttacking()
    {
        animator.SetTrigger("Attack");
    }
    
    public void TriggerAnimatorDefending()
    {
        animator.SetTrigger("Hit");
    }
    
    public void SetAnimatorMoving(bool set)
    {
        animator.SetBool("Moving", set);
    }

    public void SetAnimatorDying(bool set)
    {
        animator.SetBool("Death", set);
    }

}
