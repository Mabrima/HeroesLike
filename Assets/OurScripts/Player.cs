using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ClickableTile currentTileStandingOn;
    public float playerSpeed;

    public bool isMoving = false;

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
        Debug.Log(isMoving);
        while (Pathfinding.INSTANCE.playerPath.Count > 0)
        {

            nextMove = Pathfinding.INSTANCE.playerPath.Pop();
            transform.position = nextMove.transform.position + new Vector3(0, 0, -0.75f);
            nextMove.sphereRend.enabled = false;
            nextMove.sphereRend.material = nextMove.sphereStartMat;
            yield return new WaitForSeconds(playerSpeed);
        }
        Debug.Log("moving ended");
        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        currentTileStandingOn = other.GetComponent<ClickableTile>();
        
    }

}
