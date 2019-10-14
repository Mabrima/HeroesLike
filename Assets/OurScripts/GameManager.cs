using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button waitButton;
    public static GameManager instance;
    public List<UnitHandler> unitsInCombat;
    public Stack<UnitHandler> waitStack = new Stack<UnitHandler>();
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
        currentUnit = unitsInCombat[combatCounter];
        StartCoroutine(StartUp());
    }

    IEnumerator StartUp()
    {
        yield return new WaitForEndOfFrame();
        currentUnit.GetAvailableMovementTiles();
    }

    private void Update()
    {
        GameHandler();
        if (Input.GetMouseButtonUp(1))
        {
            statViewer.StopShow();
        }
    }

    public void GameHandler()
    {
        if (endTurn)
        {
            if (combatCounter == unitsInCombat.Count-1 && waitStack.Count > 0)
            {
                Debug.Log("popping wait");
                SetCannotWait(true);
                currentUnit = waitStack.Pop();
                currentUnit.GetAvailableMovementTiles();
                endTurn = false;
                return;
            }
            combatCounter = (combatCounter + 1) % unitsInCombat.Count;
            bool found = false;
            while (!found)
            {
                currentUnit = unitsInCombat[combatCounter % unitsInCombat.Count];
                if (currentUnit.totalHealth <= 0)
                {
                    unitsInCombat.Remove(currentUnit);
                }
                else
                {
                    found = true;
                } 
            }
            currentUnit.GetAvailableMovementTiles();
            endTurn = false;
            SetCannotWait(false);
        }
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
    }

    public void Defend()
    {
        endTurn = true;
    }

    public void Wait()
    {
        waitStack.Push(currentUnit);
        endTurn = true;
    }
    public void CombatAttack(UnitHandler unit)
    {
        unit.GetHit(currentUnit.unitBase.attack, currentUnit.unitBase.damage, currentUnit.amountOfUnits);
        endTurn = true;
    }

    public void SetCannotWait(bool set)
    {
        waitButton.interactable = !set;
        canWait = !set;
    }

    private void ForwardInitiative()
    {
        foreach (UnitHandler unit in unitsInCombat)
        {
            unit.ForwardInitiative();
        }
    }

}
