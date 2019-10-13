using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PathfindingTile currentTileStandingOn;
    public float playerSpeed;
    public Vector3 tileOffset = new Vector3(0, 0, -0.75f);

    public bool isMoving;

    public bool lerping;

    private float startTime;
    private float journeyLength;

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
         
        Debug.Log("moving ended");
        isMoving = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        currentTileStandingOn = other.GetComponent<PathfindingTile>();
    }

}
