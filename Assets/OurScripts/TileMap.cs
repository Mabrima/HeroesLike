using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TileMap : MonoBehaviour
{

    public Player player;

    public TileType[] tileTypes;

    public int[,] tiles;
    public ClickableTile[,] clickableTiles;

    public int mapSizeX;
    public int mapSizeY;

    public int highestX;
    public int highestY;

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
            highestX = Mathf.Max((int)child.transform.position.x, highestX);
            highestY = Mathf.Max((int)child.transform.position.y, highestY);
        }
        mapSizeX = highestX+1;
        mapSizeY = highestY+1;
        clickableTiles = new ClickableTile[mapSizeX, mapSizeY];

        foreach (Transform child in transform)
        {
            ClickableTile tile = child.GetComponent<ClickableTile>();
            tile.tileX = (int)child.transform.position.x;
            tile.tileY = (int)child.transform.position.y;
            clickableTiles[tile.tileX, tile.tileY] = tile;
        }

        //GenerateMapData();
        //GenerateMapVisual();
    }

    void GenerateMapData()
    {
        tiles = new int[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeX; y++)
            {
                tiles[x, y] = 0;
            }
        }

        tiles[1, 1] = 2;
        tiles[1, 0] = 2;
        tiles[1, 2] = 2;
        tiles[2, 2] = 2;
        tiles[3, 1] = 2;
        tiles[3, 0] = 2;
    }
    
    void GenerateMapVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeX; y++)
            {
                TileType tt = tileTypes[tiles[x, y]];

                ClickableTile go = Instantiate(tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity).GetComponent<ClickableTile>();
                go.tileX = x;
                go.tileY = y;
                go.map = this;
                go.fieldType = tt.CheckIfWalkable();

                clickableTiles[x, y] = go;
            }
        }
    }
   
    public List<ClickableTile> GetAllTiles()
    {
        List<ClickableTile> all = new List<ClickableTile>();

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                all.Add(clickableTiles[x, y]);
            }
        }

        return all;
    }

    public List<ClickableTile> GetNeighbouringTiles(ClickableTile a_Tile)
    {
        List<ClickableTile> neighbouringTiles = new List<ClickableTile>();
        int xCheck;
        int yCheck;

        //right side
        xCheck = a_Tile.tileX + 1;
        yCheck = a_Tile.tileY;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (yCheck >= 0 && yCheck < mapSizeY)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, yCheck]);
            }
        }

        //left side
        xCheck = a_Tile.tileX - 1;
        yCheck = a_Tile.tileY;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (yCheck >= 0 && yCheck < mapSizeY)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, yCheck]);
            }
        }

        //Top side
        xCheck = a_Tile.tileX;
        yCheck = a_Tile.tileY + 1;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (yCheck >= 0 && yCheck < mapSizeY)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, yCheck]);
            }
        }

        //Bottom side
        xCheck = a_Tile.tileX;
        yCheck = a_Tile.tileY - 1;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (yCheck >= 0 && yCheck < mapSizeY)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, yCheck]);
            }
        }

        return neighbouringTiles;
    }

    public void FindPath(int x, int y, ClickableTile tile)
    {
        Pathfinding.INSTANCE.PathFinding(player.currentTileStandingOn, tile);
    }
}
