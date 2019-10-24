using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public Button waitButton;
    public Text battleText;
    public static CombatManager instance;
    public List<UnitHandler> unitsInCombat;
    public Stack<UnitHandler> waitStack = new Stack<UnitHandler>();
    public Stack<UnitHandler> actionStack = new Stack<UnitHandler>();
    public UnitHandler currentUnit;
    public bool endTurn = false;
    public bool canWait = false;
    int combatCounter = 0;

    public StatViewer statViewer;

    public enum GameState
    {
        CombatMovement, CombatAttack, 
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void Start()
    {
        statViewer.StopShow();
        combatCounter = -1;
        battleText.text = "Combat initiated, press 'D' to begin";
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


        InitiateUnits(players[0].GetComponent<Player>().units, players[1].GetComponent<Player>().units);

    }

    private void Update()
    {
        //really doesn't need an update could be made to be called whenever it's actually relevant
        if (endTurn)
        {
            NextUnitTurn();
        }
        
        HandleInputs();
    }

    private void InitiateUnits(List<UnitHandler> units1, List<UnitHandler> units2)
    {
        int maxX = FieldHandler.X_SIZE-1;
        int maxY = FieldHandler.Y_SIZE-1;
        int i = 0;
        foreach (UnitHandler unit in units1)
        {
            int posX = 0;
            int posY = (maxY / (units1.Count-1)) * i;
            unit.transform.position = new Vector3(posX, 1, posY);
            FieldHandler.instance.GetTile(posX, posY);
            i++;
        }
        i = 0;
        foreach (UnitHandler unit in units2)
        {
            int posX = maxX;
            int posY = (maxY / (units2.Count-1)) * i;
            unit.transform.position = new Vector3(posX, 1, posY);
            FieldHandler.instance.GetTile(posX, posY);
            i++;
        }
    }

    private void NextUnitTurn()
    {
        while (actionStack.Count < 1)
        {
            ForwardInitiatives();
        }

        currentUnit = actionStack.Pop();
        if (unitsInCombat.Contains(currentUnit))
        {
            ReadyUnit();
            endTurn = false;
        }
    }

    private void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Defend();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (!canWait)
                return;
            Wait();
        }

        if (Input.GetMouseButtonUp(1))
        {
            statViewer.StopShow();
        }
    }

    private void ReadyUnit()
    {
        currentUnit.canRetaliate = true;
        currentUnit.GetAvailableMovementTiles();
        SetCannotWait(false);
    }

    public void Defend()
    {
        endTurn = true;
    }

    public void Wait()
    {
        currentUnit.InitiativeWait();
        endTurn = true;
    }
    public void CombatAttack(UnitHandler unit)
    {
        currentUnit.SetAnimatorAttacking();
        unit.GetHit(currentUnit.unitBase.attack, currentUnit.unitBase.minDamage, currentUnit.unitBase.maxDamage, currentUnit.amountOfUnits);
        if (unit.canRetaliate)
        {
            currentUnit.GetHit(unit.unitBase.attack, unit.unitBase.minDamage, unit.unitBase.maxDamage, unit.amountOfUnits);
            unit.canRetaliate = false;
        }
        endTurn = true;
    }

    public void SetCannotWait(bool set)
    {
        waitButton.interactable = !set;
        canWait = !set;
    }

    private void ForwardInitiatives()
    {
        foreach (UnitHandler unit in unitsInCombat)
        {
            unit.ForwardInitiative();
            if (unit.currentInitiative > 100)
            {
                unit.currentInitiative -= 100;
                actionStack.Push(unit);
            }
        }
    }

    public void RemoveUnit(UnitHandler unit)
    {
        unitsInCombat.Remove(unit);
    }

}
