using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TileMap : MonoBehaviour
{

    public Player player;

    public int[,] tiles;
    public ClickableTile[,] clickableTiles;

    public int mapSizeX;
    public int mapSizeZ;

    public int highestX;
    public int highestZ;

    public static TileMap INSTANCE;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            if (INSTANCE != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void Start()
    {
        foreach (Transform child in transform)
        {
            highestX = Mathf.Max(Mathf.RoundToInt(child.transform.position.x), highestX);
            highestZ = Mathf.Max(Mathf.RoundToInt(child.transform.position.z), highestZ);
        }
        mapSizeX = highestX+1;
        mapSizeZ = highestZ+1;
        clickableTiles = new ClickableTile[mapSizeX, mapSizeZ];

        foreach (Transform child in transform)
        {
            ClickableTile tile = child.GetComponent<ClickableTile>();
            tile.tileX = Mathf.RoundToInt(child.transform.position.x);
            tile.tileZ = Mathf.RoundToInt(child.transform.position.z);
            clickableTiles[tile.tileX, tile.tileZ] = tile;
        }
    }
   
    public List<ClickableTile> GetAllTiles()
    {
        List<ClickableTile> all = new List<ClickableTile>();

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                all.Add(clickableTiles[x, z]);
            }
        }

        return all;
    }

    public List<ClickableTile> GetNeighbouringTiles(ClickableTile a_Tile)
    {
        List<ClickableTile> neighbouringTiles = new List<ClickableTile>();
        int xCheck;
        int zCheck;

        //right side
        xCheck = a_Tile.tileX + 1;
        zCheck = a_Tile.tileZ;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (zCheck >= 0 && zCheck < mapSizeZ)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, zCheck]);
            }
        }

        //left side
        xCheck = a_Tile.tileX - 1;
        zCheck = a_Tile.tileZ;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (zCheck >= 0 && zCheck < mapSizeZ)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, zCheck]);
            }
        }

        //Top side
        xCheck = a_Tile.tileX;
        zCheck = a_Tile.tileZ + 1;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (zCheck >= 0 && zCheck < mapSizeZ)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, zCheck]);
            }
        }

        //Bottom side
        xCheck = a_Tile.tileX;
        zCheck = a_Tile.tileZ - 1;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (zCheck >= 0 && zCheck < mapSizeZ)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, zCheck]);
            }
        }

        return neighbouringTiles;
    }

    public void FindPath(int x, int y, ClickableTile tile)
    {
        Pathfinding.INSTANCE.PathFinding(player.currentTileStandingOn, tile);
    }
}
