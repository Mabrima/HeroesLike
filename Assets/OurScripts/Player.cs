using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public PathfindingTile currentTileStandingOn;
    public float playerSpeed;
    private Vector3 tileOffset = new Vector3(0, 0, -0.5f);

    public RectTransform armyWindow;
    public Texture2D armyIcon;

    public bool isMoving;

    public bool lerping;

    private float startTime;
    private float journeyLength;

    bool showArmy = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Player") && showArmy == false)
                {
                    showArmy = true;
                    ShowArmy();
                }
                else if (hit.transform.CompareTag("Player") && showArmy == true)
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
        PathfindingTile nextMove;
        isMoving = true;


        while (Pathfinding.INSTANCE.playerPath.Count > 0) //Go through stack of the path until its empty.
        {
            nextMove = Pathfinding.INSTANCE.playerPath.Pop(); //Get next tile in stack we want to move too.
            startTime = Time.time;
            journeyLength = Vector3.Distance(transform.position, nextMove.transform.position + tileOffset);
            RotatePlayerToTile(nextMove);
            lerping = true;

            while (lerping)
            {
                float distCovered = (Time.time - startTime) * playerSpeed;
                float fractionOfJourney = distCovered / journeyLength;
                if (fractionOfJourney > 0.95)
                {
                    lerping = false;
                }
                    
                transform.position = Vector3.Lerp(transform.position, nextMove.transform.position + tileOffset, fractionOfJourney);
                
                yield return new WaitForFixedUpdate();
                nextMove.sphereRend.enabled = false; //Turn sphere off
                nextMove.sphereRend.material = nextMove.sphereStartMat; //change sphere to start material.
            }

        }
        isMoving = false;
    }

    public void RotatePlayerToTile(PathfindingTile nextMove)
    {
        Quaternion newRotation = Quaternion.LookRotation(transform.position - nextMove.sphereRend.transform.position, Vector3.forward);
        newRotation.x = 0f;
        newRotation.y = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Mathf.Infinity);
    }

    public void ShowArmy()
    {
        if (showArmy == true)
        {
            armyWindow.gameObject.SetActive(true);
        }
        else if (showArmy == false)
        {
            armyWindow.gameObject.SetActive(false);
        }
        
    }
    private void OnMouseOver()
    {
       Cursor.SetCursor(armyIcon, Vector2.zero, CursorMode.Auto);
    }


    private void OnTriggerEnter(Collider other)
    {
        currentTileStandingOn = other.GetComponent<PathfindingTile>();
    }

}
