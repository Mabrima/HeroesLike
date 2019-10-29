using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{
    public int tileX;
    public int tileY;

    public TileData tileData;

    public MeshRenderer sphereRend; //TODO Should be moved down to clickable tile.
    public Material sphereStartMat; //
    public Material sphereEndMat; //
    public TileMap map; //

    protected MeshRenderer rend;
    public Material mouseOverMaterial;
    protected Material defaultMaterial;

    public FieldType fieldType = FieldType.Empty;

    protected Player player;

    public Texture2D blockedPathIcon;
    public Texture2D walkHereIcon;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        defaultMaterial = rend.material;
        player = FindObjectOfType<Player>();
        sphereRend = transform.Find("Sphere").GetComponent<MeshRenderer>();


    }

    public void ResetPathfindingValues()
    {
        tileData.hCost = 0;
        tileData.gCost = 0;
        tileData.pfParent = null;
    }

    public void OnMouseUp()
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

    public void OnMouseEnter()
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

    public void OnMouseExit()
    {
        rend.material = defaultMaterial;
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }


}
