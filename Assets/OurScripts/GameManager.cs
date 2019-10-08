using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button waitButton;
    public static GameManager instance;
    public UnitHandler[] unitsInCombat;
    public Stack<UnitHandler> waitStack = new Stack<UnitHandler>();
    public UnitHandler currentUnit;
    public bool endTurn;
    int combatCounter = 0;

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
        currentUnit = unitsInCombat[combatCounter % unitsInCombat.Length];
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
    }

    public void GameHandler()
    {
        if (endTurn)
        {
            if (combatCounter % unitsInCombat.Length == 0 && waitStack.Count > 0)
            {
                Debug.Log("popping wait");
                SetCannotWait();
                currentUnit = waitStack.Pop();
                currentUnit.GetAvailableMovementTiles();
                endTurn = false;
                return;
            }
            combatCounter++;
            currentUnit = unitsInCombat[combatCounter % unitsInCombat.Length];
            currentUnit.GetAvailableMovementTiles();
            endTurn = false;
            waitButton.interactable = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Defend();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
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
        unit.unitBase.CalculateDamage(currentUnit.unitBase.attack, currentUnit.unitBase.damage);
    }

    public void SetCannotWait()
    {
        waitButton.interactable = false;
    }

}
