using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{

    public int tileX;
    public int tileY;

    public TileData tileData;

    public MeshRenderer sphereRend;

    public Material sphereStartMat;
    public Material sphereEndMat;

    public Material mouseHover;
    public Material startMat;

    public bool isWalkable;

    public TileMap map;

    private Player player;

    private void Start()
    {
        startMat = transform.GetComponent<MeshRenderer>().material;
        player = FindObjectOfType<Player>();
        sphereRend = transform.Find("Sphere").GetComponent<MeshRenderer>();

    }
    private void OnMouseUp()
    {
        Debug.Log("On Click: " + player.isMoving);
        if (player.isMoving == true)
        {
            Debug.Log("hej");
            return;
        }

        if (!Pathfinding.INSTANCE.ChoseNextTile(this))
        {
            map.FindPath(tileX, tileY, this);
        }
        
            
           
    }

    private void OnMouseEnter()
    {
        transform.GetComponent<MeshRenderer>().material = mouseHover;
    }

    private void OnMouseExit()
    {
        transform.GetComponent<MeshRenderer>().material = startMat;
    }

    public void ResetPathfindingValues()
    {
        tileData.hCost = 0;
        tileData.gCost = 0;
        tileData.pfParent = null;
    }
}
