using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickableTile : MonoBehaviour
{
    public int tileX;
    public int tileZ;

    public TileData tileData;

    public MeshRenderer sphereRend; 
    public Material sphereStartMat; 
    public Material sphereEndMat; 
    public TileMap map;

    protected MeshRenderer rend;
    public Material mouseOverMaterial;
    protected Material defaultMaterial;

    public FieldType fieldType = FieldType.Empty;

    protected Player player;

    public Texture2D blockedPathIcon;
    public Texture2D walkHereIcon;
    public Texture2D standingHorseIcon;
    public Texture2D swordIcon;

    public bool showMouseIcons;

    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

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
            map.FindPath(tileX, tileZ, this);
        }
           
    }

    public void OnMouseEnter()
    {
        if (showMouseIcons)
        {
            rend.material = mouseOverMaterial;
            if (fieldType == FieldType.Obstacle)
            {
                Cursor.SetCursor(blockedPathIcon, hotSpot, cursorMode);
            }
            else if (fieldType == FieldType.Empty)
            {
                Cursor.SetCursor(walkHereIcon, hotSpot, cursorMode);
            }
            else if (fieldType == FieldType.Reward)
            {
                Cursor.SetCursor(standingHorseIcon, hotSpot, cursorMode);
            }
            else if (fieldType == FieldType.Combat)
            {
                Cursor.SetCursor(swordIcon, hotSpot, cursorMode);
            }
        }
        
    }

    public void OnMouseExit()
    {
        rend.material = defaultMaterial;
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            fieldType = FieldType.Obstacle;
        }
        
    }



}
