using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMap : MonoBehaviour
{

    public Player player;

    public TileType[] tileTypes;

    public int[,] tiles;
    public PathfindingTile[,] clickableTiles;

    public int mapSizeX = 50;
    public int mapSizeY = 50;

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
        clickableTiles = new PathfindingTile[mapSizeX, mapSizeY];

        GenerateMapData();
        GenerateMapVisual();

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

                PathfindingTile go = Instantiate(tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity).GetComponent<PathfindingTile>();
                go.tileX = x;
                go.tileY = y;
                go.map = this;
                go.fieldType = tt.CheckIfWalkable();

                clickableTiles[x, y] = go;
            }
        }
    }
   
    public List<PathfindingTile> GetAllTiles()
    {
        List<PathfindingTile> all = new List<PathfindingTile>();

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeX; y++)
            {
                all.Add(clickableTiles[x, y]);
            }
        }

        return all;
    }

    public List<PathfindingTile> GetNeighbouringTiles(PathfindingTile a_Tile)
    {
        List<PathfindingTile> neighbouringTiles = new List<PathfindingTile>();
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

    public void FindPath(int x, int y, PathfindingTile tile)
    {
        Pathfinding.INSTANCE.PathFinding(player.currentTileStandingOn, tile);
    }
}
