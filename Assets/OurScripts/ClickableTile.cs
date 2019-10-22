using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : PathfindingTile
{

    public Texture2D blockedPathIcon;
    public Texture2D walkHereIcon;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private void Start()
    {
        base.Initiate();
        defaultMaterial = rend.material;
        player = FindObjectOfType<Player>();
        sphereRend = transform.Find("Sphere").GetComponent<MeshRenderer>();

    }

    public override void OnMouseUp()
    {
        if (player.isMoving == true)
        {
            return;
        }

        if (!Pathfinding.INSTANCE.ChoseNextTile(this) && this != player.currentTileStandingOn)
        {
            map.FindPath(tileX, tileY, this);
        }
           
    }

    public override void OnMouseEnter()
    {
        rend.material = mouseOverMaterial;
        if (fieldType == FieldType.Obstacle)
        {
            Cursor.SetCursor(blockedPathIcon, hotSpot, cursorMode);
        }
        else if (fieldType == FieldType.Empty)
        {
            foreach (ClickableTile neighbour in TileMap.INSTANCE.GetNeighbouringTiles(this)) //Go through an empty tiles neighbours. 
            {
                if (neighbour.fieldType == FieldType.Empty) //If even one of the neighbours are "open" we know there is a path there.
                {
                    Cursor.SetCursor(walkHereIcon, hotSpot, cursorMode);
                    break;
                }
                else //If none of the neighbours are empty == all neighbours are closed, we know the path is blocked.
                {
                    Cursor.SetCursor(blockedPathIcon, hotSpot, cursorMode);
                }
            }
            
        }
    }

    public override void OnMouseExit()
    {
        rend.material = defaultMaterial;
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }


}
