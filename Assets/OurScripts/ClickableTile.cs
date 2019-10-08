using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{

    public int tileX;
    public int tileY;

    public TileData tileData;

    public bool isWalkable;
    public Vector3 position;

    public TileMap map;

    private void OnMouseUp()
    {
        Debug.Log("Tile: " + tileX + tileY);
        map.MovePlayerTo(tileX, tileY, this);
    }

    public ClickableTile(bool a_isWalkable, Vector3 a_pos, int a_tileX, int a_tileY)
    {
        isWalkable = a_isWalkable;
        position = a_pos;
        tileX = a_tileX;
        tileY = a_tileY;
    }

    public void ResetPathfindingValues()
    {
        tileData.hCost = 0;
        tileData.gCost = 0;
        tileData.pfParent = null;
    }
}
