using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTile : MonoBehaviour
{

    public int tileX;
    public int tileY;

    public TileData tileData;

    public MeshRenderer sphereRend; //TODO Should be moved down to clickable tile.
    public Material sphereStartMat;
    public Material sphereEndMat;
    public TileMap map;

    protected MeshRenderer rend;
    public Material mouseOverMaterial;
    protected Material defaultMaterial;

    public FieldType fieldType = FieldType.Empty;

    protected Player player;

    public void Initiate()
    {
        rend = GetComponent<MeshRenderer>();
    }

    public void ResetPathfindingValues()
    {
        tileData.hCost = 0;
        tileData.gCost = 0;
        tileData.pfParent = null;
    }
}
