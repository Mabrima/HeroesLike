using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public Button endTurnButton;
    public Button defendButton;
    public Text battleText;
    public static CombatManager instance;
    public GameObject[] spawnGroups;
    public List<UnitHandler> unitsInCombat;
    public List<UnitHandler> initiativeList = new List<UnitHandler>();
    public UnitHandler currentUnit;
    public bool endTurn = false;
    public bool canDefend = true;
    public bool canEndTurn = true;
    int combatCounter = 0;

    [SerializeField] GameObject unitTimeLinePrefab;
    [SerializeField] GameObject timeLinePanel;
    [SerializeField] int timeLineLength = 11;
    [SerializeField] Vector3 imageDisplacement = new Vector3(110, 0);
    List<Image> unitsImageInTimeline = new List<Image>();
    List<Text> unitsAmountInTimeline = new List<Text>();
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

    //Testing of without overworld
    void Start()
    {
        Invoke("InvokeStartCombat", .4f);
    }

    void InvokeStartCombat()
    {
        StartCombat(GameManager.info.player, GameManager.info.opponent);
    }

    public void StartCombat(Player player1, Player player2)
    {
        statViewer.StopShow();
        combatCounter = -1;
        battleText.text = "Combat initiated, press 'D' to begin";

        InitiateUnits(player1, player2);
        InitiateTimeLine();
    }

    private void Update()
    {
        //really doesn't need an update could be made to be called whenever it's actually relevant
        if (endTurn)
        {
            if (!CheckCombatEnd())
                NextUnitTurn();
        }
        
        HandleInputs();
    }

    private bool CheckCombatEnd()
    {
        int winTeam = unitsInCombat[0].team;

        foreach (UnitHandler unit in unitsInCombat)
        {
            if (unit.team != winTeam)
            {
                //no winner yet
                return false;
            }
        }

        battleText.text += '\n' + "<b><color=red>Battle has ended!</color></b> \nWinningTeam is " + winTeam + "\nPress 'D' to continue";
        endTurn = false;
        return true;
    }

    private void InitiateUnits(Player player1, Player player2)
    {
        int maxX = FieldHandler.X_SIZE-1;
        int maxY = FieldHandler.Y_SIZE-1;
        int i = 0;
        foreach (UnitHandler unit in player2.units)
        {
            int posX = 0;
            int posY = (maxY / (player2.units.Count-1)) * i;
            InitiateNewUnit(posX, posY, unit, player2, i);
            i++;
        }
        i = 0;
        foreach (UnitHandler unit in player1.units)
        {
            int posX = maxX;
            int posY = (maxY / (player1.units.Count-1)) * i;
            InitiateNewUnit(posX, posY, unit, player1, i);
            i++;
        }
    }

    void InitiateTimeLine()
    {
        for (int i = 0; i < timeLineLength; i++)
        {
            GameObject temp = Instantiate(unitTimeLinePrefab, timeLinePanel.transform);
            temp.transform.localPosition = unitTimeLinePrefab.transform.position + imageDisplacement * i;
            unitsImageInTimeline.Add(temp.GetComponentInChildren<Image>());
            unitsAmountInTimeline.Add(temp.GetComponentInChildren<Text>());
        }
    }

    void InitiateNewUnit(int posX, int posY, UnitHandler unit, Player player, int i)
    {
        UnitHandler temp = UnitDispenser.instance.SpawnUnit(unit);
        temp.team = player.team;
        temp.amountOfUnits = player.units[i].amountOfUnits;
        temp.computerControlled = player.computerControlled;
        temp.transform.position = new Vector3(posX, 0.5f, posY);
        temp.currentTile = FieldHandler.instance.GetTile(posX, posY);
        temp.currentTile.PutUnitOnTile(temp);
        temp.RotateTeamDirection(temp.team);
        temp.GetComponentInChildren<Image>().color = temp.teamColors[temp.team-1];
        unitsInCombat.Add(temp);
    }

    private void NextUnitTurn()
    {
        while (initiativeList.Count < timeLineLength)
        {
            ForwardInitiatives();
        }

        HandleTimeLine();

        currentUnit = initiativeList[0];
        initiativeList.RemoveAt(0);
        if (unitsInCombat.Contains(currentUnit))
        {
            ReadyUnit();
            endTurn = false;
        }
    }

    private void HandleTimeLine()
    {
        for (int i = 0; i < timeLineLength; i++)
        {
            unitsImageInTimeline[i].sprite = initiativeList[i].unitBase.sprite;
            unitsAmountInTimeline[i].text = initiativeList[i].amountOfUnits.ToString();
            unitsAmountInTimeline[i].color = initiativeList[i].teamColors[initiativeList[i].team-1];
        }
    }

    private void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.D) && canDefend)
        {
            if (CheckCombatEnd())
            {
                Player player = GameManager.info.player;
                player.units.Clear();
                foreach(UnitHandler unit in unitsInCombat)
                {
                    player.units.Add(unit);
                }
                GameManager.instance.SceneSwitchToOverWorld(player);
            }
            Defend();
        }
        else if (Input.GetKeyDown(KeyCode.E) && canEndTurn)
        {
            EndTurn();
        }

        if (Input.GetMouseButtonUp(1))
        {
            statViewer.StopShow();
        }
    }

    private void ReadyUnit()
    {
        currentUnit.ResetAbilities();
        currentUnit.canRetaliate = true;
        currentUnit.TriggerAbilitiesAtTiming(AbilityTiming.StartOfTurn, currentUnit);
        SetCannotEndTurn(false);
        SetCannotDefend(false);
        currentUnit.GetAvailableMovementTiles();
    }

    public void Defend()
    {
        if (currentUnit)
        {
            currentUnit.Defend();
            battleText.text += '\n' + currentUnit.unitBase.name + ' ' + currentUnit.team + " put up their guard";
        }
        endTurn = true;
    }

    public void EndTurn()
    {
        endTurn = true;
    }

    public void StartCombatAttack(UnitHandler unit)
    {
        StartCoroutine(CombatAttack(unit));
    }

    private IEnumerator CombatAttack(UnitHandler unit)
    {
        SetCannotDefend(true);
        SetCannotEndTurn(true);
        currentUnit.TriggerAnimatorAttacking();
        currentUnit.RotateUnitToTile(unit.currentTile);
        unit.RotateUnitToTile(currentUnit.currentTile);

        //currentUnit.TriggerAbilitiesAtTiming(AbilityTiming.DuringAttack, unit); //TODO check if better at tile selection.
        unit.GetHit(currentUnit.unitBase.attack, currentUnit.unitBase.minDamage, currentUnit.unitBase.maxDamage, currentUnit.amountOfUnits);
        if (unit.canRetaliate)
        {
            yield return new WaitForSeconds(1f);

            //currentUnit.TriggerAbilitiesAtTiming(AbilityTiming.DuringAttack, currentUnit); //TODO check if better at tile selection.
            unit.TriggerAnimatorAttacking();
            currentUnit.GetHit(unit.unitBase.attack, unit.unitBase.minDamage, unit.unitBase.maxDamage, unit.amountOfUnits);
            unit.canRetaliate = false;
        }
        if(unit != null)
            unit.TriggerAbilitiesAtTiming(AbilityTiming.AfterDefence, currentUnit);
        yield return new WaitForSeconds(1f);
        if (currentUnit != null)
        {
            currentUnit.TriggerAbilitiesAtTiming(AbilityTiming.AfterAttack, unit);
            currentUnit.RotateTeamDirection(currentUnit.team);
        }
        if (unit != null)
            unit.RotateTeamDirection(unit.team);
        SetCannotDefend(false);
        SetCannotEndTurn(false);
        endTurn = true;
    }

    public void SetCannotEndTurn(bool set)
    {
        endTurnButton.interactable = !set;
        canEndTurn = !set;
    }
    
    public void SetCannotDefend(bool set)
    {
        defendButton.interactable = !set;
        canDefend = !set;
    }

    private void ForwardInitiatives()
    {
        foreach (UnitHandler unit in unitsInCombat)
        {
            unit.ForwardInitiative();
            if (unit.currentInitiative > 100)
            {
                unit.currentInitiative -= 100;
                initiativeList.Add(unit);
            }
        }
    }

    public void RemoveDeadUnitFromCombat(UnitHandler unit)
    {
        unitsInCombat.Remove(unit);
        while (initiativeList.Contains(unit))
            initiativeList.Remove(unit);
    }

}
