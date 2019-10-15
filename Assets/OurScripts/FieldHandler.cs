using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FieldType
{
    Empty, Occupied, Obstacle
}

public class FieldHandler : MonoBehaviour
{
    public static int X_SIZE = 12;
    public static int Y_SIZE = 10;
    public static FieldHandler instance;
    public CombatTile[,] fieldObjects = new CombatTile[X_SIZE, Y_SIZE];
    [SerializeField] GameObject battleTilePrefab;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        InitiateNewField();
    }

    public PathfindingTile GetPathfindingTile(int x, int y)
    {
        return fieldObjects[x, y];
    }

    public void PutUnitOnTile(PathfindingTile tile, UnitHandler unit)
    {
        fieldObjects[tile.tileX, tile.tileY].PutUnitOnTile(unit);
    }

    public void RemoveUnitFromTile(PathfindingTile tile)
    {
        fieldObjects[tile.tileX, tile.tileY].RemoveUnitFromTile();
    }

    public void GetAvailableMovementTiles(PathfindingTile tile, int range, bool asFake = false)
    {
        int x = tile.tileX;
        int y = tile.tileY;
        for (int i = 0; i < range; i++)
        {
            for (int j = 0; j < range - i; j++)
            {
                try
                {
                    if (fieldObjects[x + i, y + j].fieldType == FieldType.Empty)
                        fieldObjects[x + i, y + j].SetAsSelectable(asFake);
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x + i, y - j].fieldType == FieldType.Empty)
                        fieldObjects[x + i, y - j].SetAsSelectable(asFake);
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x - i, y - j].fieldType == FieldType.Empty)
                        fieldObjects[x - i, y - j].SetAsSelectable(asFake);
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x - i, y + j].fieldType == FieldType.Empty)
                        fieldObjects[x - i, y + j].SetAsSelectable(asFake);
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
            }
        }
    }

    public void GetAvailableAttackTiles(PathfindingTile tile, int team)
    {
        int x = tile.tileX;
        int y = tile.tileY;
        Debug.Log("looking for tiles to attack");
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    j++;
                }
                try
                {
                    if (fieldObjects[x + i, y + j].fieldType == FieldType.Occupied && fieldObjects[x + i, y + j].unitOnTile.team != team)
                    {
                        fieldObjects[x + i, y + j].SetAsSelectable();
                    }
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x + i, y - j].fieldType == FieldType.Occupied && fieldObjects[x + i, y - j].unitOnTile.team != team)
                    {
                        fieldObjects[x + i, y - j].SetAsSelectable();
                    }
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x - i, y - j].fieldType == FieldType.Occupied && fieldObjects[x - i, y - j].unitOnTile.team != team)
                    {
                        fieldObjects[x - i, y - j].SetAsSelectable();
                    }
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x - i, y + j].fieldType == FieldType.Occupied && fieldObjects[x - i, y + j].unitOnTile.team != team)
                    {
                        fieldObjects[x - i, y + j].SetAsSelectable();
                    }
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
            }
        }
    }

    public void InitiateNewField()
    {
        int x = 0;
        int y = 0;

        if (fieldObjects[0, 0] != null)
        {
            foreach (CombatTile tile in fieldObjects)
            {
                Destroy(tile);
            }
        }

        foreach (CombatTile tile in fieldObjects)
        {
            fieldObjects[x, y] = Instantiate(battleTilePrefab, new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<CombatTile>();
            fieldObjects[x, y].SetPositionValues(x, y);

            x = (x + 1) % 12;
            if (x == 0)
                y = (y + 1) % 10;
        }
    }

    public void ResetPossibleLocations(bool fake = false)
    {
        foreach (CombatTile tile in fieldObjects)
        {
            tile.SetAsNotSelectable(fake);
        }
    }
}
