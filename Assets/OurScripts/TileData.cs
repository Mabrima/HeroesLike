using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TileData
{
    public int gCost;
    public int hCost;
    public int fCost;

    public ClickableTile pfParent;

    public int CalculateAndGetF()
    {
        fCost = gCost + hCost;
        return fCost;
    }
}
