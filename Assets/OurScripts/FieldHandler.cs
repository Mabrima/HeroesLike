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
    List<PathfindingTile> neighbourList1 = new List<PathfindingTile>();
    List<PathfindingTile> neighbourList2 = new List<PathfindingTile>();

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
        neighbourList1.Clear();
        neighbourList2.Clear();
        neighbourList1.Add(tile);

        for (int i = 0; i < range - 1; i++)
        {
            if (i % 2 == 0) 
            {
                GetNeighbours(neighbourList1, neighbourList2, asFake);
            }
            else
            {
                GetNeighbours(neighbourList2, neighbourList1, asFake);
            }
        }
    }

    private void GetNeighbours(List<PathfindingTile> useList, List<PathfindingTile> addList, bool asFake)
    {
        foreach (PathfindingTile tile in useList)
        {
            int x = tile.tileX;
            int y = tile.tileY;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    try
                    {
                        if ((i == 0 && j == 0) || (i != 0 && j != 0))
                            continue;

                        CombatTile neighbourTile = fieldObjects[x + i, y + j];
                        if (!asFake)
                        {
                            if (neighbourTile.fieldType == FieldType.Empty && neighbourTile.tileData.pfParent == null)
                            {
                                neighbourTile.SetAsSelectable(asFake);
                                neighbourTile.tileData.pfParent = tile;
                                addList.Add(neighbourTile);
                            }
                        }
                        else
                        {
                            if (neighbourTile.fieldType == FieldType.Empty && neighbourTile.tileData.perishableParent == null)
                            {
                                neighbourTile.SetAsSelectable(asFake);
                                neighbourTile.tileData.perishableParent = tile;
                                addList.Add(neighbourTile);
                            }
                        }
                    }
                    catch
                    {
                        //Do nothing, just out of bounds
                    }
                }
            }
        }
        useList.Clear();
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
                        fieldObjects[x + i, y + j].SetAsSelectable(false);
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
                        fieldObjects[x + i, y - j].SetAsSelectable(false);
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
                        fieldObjects[x - i, y - j].SetAsSelectable(false);
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
                        fieldObjects[x - i, y + j].SetAsSelectable(false);
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

    public void ResetSelectableLocations(bool fake = false)
    {
        foreach (CombatTile tile in fieldObjects)
        {
            tile.SetAsNotSelectable(fake);
            tile.tileData.perishableParent = null;
        }
    }

    public void ClearParents()
    {
        foreach (CombatTile tile in fieldObjects)
        {
            tile.tileData.pfParent = null;
        }
    }


    public void FlyGetAvailableMovementTiles(PathfindingTile tile, int range, bool asFake = false)
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
}
