using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Tile[,] TileGrid;
    public Vector2Int GridSize;
    public GameObject TilesHolder, UnitsHolder, EnemyUnitsHolder;

    private void Start()
    {
        InitializeGrid();
    }

    private void Update()
    {
        
    }

    private void InitializeGrid()
    {
        TileGrid = new Tile[GridSize.x, GridSize.y];
        FindTiles();
        FindUnits();
    }

    private void FindUnits()
    {
        foreach (Unit unit in UnitsHolder.GetComponentsInChildren<Unit>())
        {
            unit.ResetUnit();
            Vector2Int unitGridPos = FindGridPos(unit.transform);
            unit.gridPos = unitGridPos;
            TileGrid[unitGridPos.x, unitGridPos.y].unit = unit;
            unit.tile = TileGrid[unitGridPos.x, unitGridPos.y];
        }
        foreach (Unit unit in EnemyUnitsHolder.GetComponentsInChildren<Unit>())
        {
            unit.ResetUnit();
            Vector2Int unitGridPos = FindGridPos(unit.transform);
            unit.gridPos = unitGridPos;
            TileGrid[unitGridPos.x, unitGridPos.y].unit = unit;
            unit.tile = TileGrid[unitGridPos.x, unitGridPos.y];
        }
    }

    private void FindTiles()
    {
        foreach (Tile tile in TilesHolder.GetComponentsInChildren<Tile>())
        {
            tile.visited = false;
            Vector2Int tileGridPos = FindGridPos(tile.transform);
            if (DoesTileExist(tileGridPos))
            {
                tile.gridPos = tileGridPos;
                TileGrid[tileGridPos.x, tileGridPos.y] = tile;
            }
        }
        /*for (int i = 0; i < GridSize.x; i++) //sprawdzanie czy wszystkie tile sa zapisane w 'TileGrid'
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                TileGrid[i,j].transform.position += new Vector3(0, 1, 0);
            }
        }*/
    }

    public Tile[] PathFinding(Vector2Int startPos, int range)
    {
        List<Tile> visitedTiles = new List<Tile>();

        Queue<Tile> tilesToVisit = new Queue<Tile>();
        tilesToVisit.Enqueue(TileGrid[startPos.x, startPos.y]);
        tilesToVisit.Peek().pathFindingRange = range;
        tilesToVisit.Peek().visited = true;

        while(tilesToVisit.Count > 0)
        {
            visitedTiles.Add(tilesToVisit.Peek());

            if (tilesToVisit.Peek().pathFindingRange == 0)
            {
                tilesToVisit.Dequeue();
                continue;
            }

            Tile[] newTiles = FindNeighbours(tilesToVisit.Peek().gridPos);
            foreach (Tile newTile in newTiles)
            {
                if (newTile.obstacle || newTile.visited)
                    continue;
                if (newTile.unit != null && newTile.unit.enemy != TileGrid[startPos.x, startPos.y].unit.enemy)
                    continue;

                newTile.visited = true;
                newTile.pathFindingRange = tilesToVisit.Peek().pathFindingRange - 1;
                tilesToVisit.Enqueue(newTile);
            }

            tilesToVisit.Dequeue();
        }

        return visitedTiles.ToArray();
    }

    public Tile[] FindNeighbours (Vector2Int tilePos)
    {
        List<Tile> Tiles = new List<Tile>();

        if (DoesTileExist(new Vector2(tilePos.x - 1, tilePos.y)))
            Tiles.Add(TileGrid[tilePos.x - 1, tilePos.y]);
        if (DoesTileExist(new Vector2(tilePos.x + 1, tilePos.y)))
            Tiles.Add(TileGrid[tilePos.x + 1, tilePos.y]);
        if (DoesTileExist(new Vector2(tilePos.x, tilePos.y - 1)))
            Tiles.Add(TileGrid[tilePos.x, tilePos.y - 1]);
        if (DoesTileExist(new Vector2(tilePos.x, tilePos.y + 1)))
            Tiles.Add(TileGrid[tilePos.x, tilePos.y + 1]);

        return Tiles.ToArray();
    }

    public bool DoesTileExist(Vector2 pos)
    {
        if (pos.x < 0 || pos.x >= GridSize.x)
            return false;
        if (pos.y < 0 || pos.y >= GridSize.y)
            return false;
        return true;
    }
    public Vector2Int FindGridPos(Transform transf)
    {
        Vector2Int gridPos = new Vector2Int();
        gridPos.x = (int)transf.position.x / 2;
        gridPos.y = (int)transf.position.z / 2;
        return gridPos;
    }

    public void MoveUnit(Unit unit, Vector2Int Pos)
    {
        unit.LookAt(unit.transform.position, TileGrid[Pos.x, Pos.y].transform.position);
        unit.tile.unit = null;
        unit.tile = null;
        unit.tile = TileGrid[Pos.x, Pos.y];
        unit.transform.position = unit.tile.transform.position;
        unit.tile.unit = unit;
        unit.gridPos = unit.tile.gridPos;
    }
}
