using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TileData
{
    public int gCost;
    public int hCost;
    public int fCost;

    public PathfindingTile pfParent;

    public int CalculateAndGetF()
    {
        fCost = gCost + hCost;
        return fCost;
    }
}
