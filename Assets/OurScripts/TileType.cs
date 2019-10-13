using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType
{
    public string name;
    public GameObject tileVisualPrefab;

    public FieldType CheckIfWalkable()
    {
        if (name == "Mountain")
        {
            return FieldType.Obstacle;
        }

        return FieldType.Empty;
    }
}
