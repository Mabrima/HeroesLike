using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FieldType
{
    Empty, Occupied
}

public class FieldHandler : MonoBehaviour
{
    public static int X_SIZE = 12;
    public static int Y_SIZE = 10;
    public static FieldHandler instance;
    public FieldType[,] fieldStates = new FieldType[X_SIZE, Y_SIZE];
    public MouseBehaviour[,] fieldObjects = new MouseBehaviour[X_SIZE, Y_SIZE];
    public GameObject battleTile;

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

    public void PutUnitOnTile(int x, int y)
    {
        fieldStates[x, y] = FieldType.Occupied;
    }

    public void RemoveUnitFromTile(int x, int y)
    {
        fieldStates[x, y] = FieldType.Empty;
    }

    public void GetAvailableTiles(int x, int y, int range)
    {
        for (int i = 0; i < range; i++)
        {
            for (int j = 0; j < range - i; j++)
            {
                try
                {
                    if (fieldStates[x + i, y + j] == FieldType.Empty)
                        fieldObjects[x + i, y + j].SetAsSelectable();
                }
                catch
                {
                    Debug.Log("outofbounds");
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    
                    if (fieldStates[x + i, y - j] == FieldType.Empty)
                        fieldObjects[x + i, y - j].SetAsSelectable();
                }
                catch
                {
                    Debug.Log("outofbounds");
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldStates[x - i, y - j] == FieldType.Empty)
                        fieldObjects[x - i, y - j].SetAsSelectable();
                }
                catch
                {
                    Debug.Log("outofbounds");
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldStates[x - i, y + j] == FieldType.Empty)
                        fieldObjects[x - i, y + j].SetAsSelectable();
                }
                catch
                {
                    Debug.Log("outofbounds");
                    //Do nothing, we just went out of array bounds
                }
            }
        }
    }

    public void InitiateNewField()
    {
        int x = 0;
        int y = 0;

        if (fieldObjects[0,0] != null)
        {
            foreach (FieldType tile in fieldStates)
            {
                Destroy(battleTile);

                x = (x + 1) % 12;
                if (x == 0)
                    y = (y + 1) % 10;
            }
            x = 0;
            y = 0;
        }

        foreach (FieldType tile in fieldStates)
        {
            fieldObjects[x, y] = Instantiate(battleTile, new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<MouseBehaviour>();
            fieldObjects[x, y].SetPositionValues(x, y);

            x = (x + 1) % 12;
            if (x == 0)
                y = (y + 1) % 10;
        }
    }

    public void ResetPossibleLocations()
    {
        foreach (MouseBehaviour tile in fieldObjects)
        {
            tile.SetAsNotSelectable();
        }
    }
}
