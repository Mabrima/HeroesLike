using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public ClickableTile currentTileStandingOn;
    public float playerSpeed;
    private Vector3 tileOffset = new Vector3(0, 0.5f, 0);

    public RectTransform armyWindow;
    private ArmyDisplayManager aw;
    public Texture2D armyIcon;

    public bool isMoving;
    public int team;
    public bool computerControlled;

    public bool lerping;

    private float startTime;
    private float journeyLength;

    public bool allowedToMove = true;


    public List<int> initialUnitsAmount;
    [SerializeField] List<UnitBase> initialUnitBases;
    [HideInInspector] public List<UnitHandler> units = new List<UnitHandler>();

    bool showArmy = false;

    private void Start()
    {
        if (armyWindow)
            aw = armyWindow.GetComponent<ArmyDisplayManager>();
        if (currentTileStandingOn)
            transform.position = currentTileStandingOn.transform.position + tileOffset;

        int i = 0;
        units.Clear();
        foreach (UnitBase unitBase in initialUnitBases)
        {
            UnitHandler unit = new UnitHandler();
            unit.unitBase = unitBase;
            unit.amountOfUnits = initialUnitsAmount[i];
            units.Add(unit);
            i++;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == gameObject && showArmy == false)
                {
                    aw.UpdateArmy();
                    showArmy = true;
                    ShowArmy();
                }
                else if (hit.transform.gameObject == gameObject && showArmy == true)
                {
                    showArmy = false;
                    ShowArmy();
                }
            }
        }
    }

    public void StartMovePlayer(bool isMoving)
    {
        if (isMoving == false)
        {
            StartCoroutine(MovePlayer());
        }
    }

    IEnumerator MovePlayer()
    {
        ClickableTile nextMove;
        isMoving = true;


        while (Pathfinding.INSTANCE.playerPath.Count > 0) //Go through stack of the path until its empty.
        {
            nextMove = Pathfinding.INSTANCE.playerPath.Pop(); //Get next tile in stack we want to move too.
            startTime = Time.time;
            journeyLength = Vector3.Distance(transform.position, nextMove.transform.position + tileOffset);
            RotatePlayerToTile(nextMove);
            lerping = true;

            Vector3 startpos = transform.position;
            while (lerping)
            {
                float distCovered = (Time.time - startTime) * playerSpeed;
                float fractionOfJourney = distCovered / journeyLength;
                if (fractionOfJourney > 0.95)
                {
                    lerping = false;
                }
                    
                transform.position = Vector3.Lerp(startpos, nextMove.transform.position + tileOffset, fractionOfJourney);
                
                yield return new WaitForFixedUpdate();
                nextMove.sphereRend.enabled = false; //Turn sphere off
                nextMove.sphereRend.material = nextMove.sphereStartMat; //change sphere to start material.
            }

        }
        isMoving = false;
    }

    public void RotatePlayerToTile(ClickableTile nextMove)
    {
        Quaternion newRotation = Quaternion.LookRotation(transform.position - nextMove.sphereRend.transform.position, Vector3.up ) * Quaternion.Euler(0, 180, 0);
        newRotation.x = 0f;
        newRotation.z = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Mathf.Infinity);
    }

    public void ShowArmy()
    {
        if (showArmy == true)
        {
            armyWindow.gameObject.SetActive(true);
            aw.ShowUnits();
        }
        else if (showArmy == false)
        {
            armyWindow.gameObject.SetActive(false);
            aw.UpdateArmy();
        }
        
    }
    private void OnMouseOver()
    {
       Cursor.SetCursor(armyIcon, Vector2.zero, CursorMode.Auto);
    }


    private void OnTriggerEnter(Collider other)
    {
        currentTileStandingOn = other.GetComponent<ClickableTile>();

        if (other.GetComponent<ClickableTile>().fieldType == FieldType.Reward)
        {
            Debug.Log("walked on reward tile");
        }
    }

    public void AddToPlayerArmy(UnitBase addUnit, int unitAmount)
    {
        bool found = false;
        foreach (UnitHandler unit in units)
        {
            if (unit.unitBase.name == addUnit.name)
            {
                unit.amountOfUnits += unitAmount;
                found = true;
            }
        }

        if (!found)
        {
            UnitHandler unit = new UnitHandler();
            unit.unitBase = addUnit;
            unit.amountOfUnits = unitAmount;
            units.Add(unit);
        }

        aw.UpdateArmy();
        aw.ShowUnits();
    }

}
