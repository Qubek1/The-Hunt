using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INKWIZYTORState : TurnState
{
    public Unit INKWIZYTOR;
    public TurnManager turnManager;
    public float cooldown = 0.2f;
    public Color AttackingColor;
    float AttackTime;
    List<Tile> actionTiles = new List<Tile>();

    public override void StartState()
    {
        turnManager.currentState = this;
        Move(INKWIZYTOR);
    }

    public override void UpdateState()
    {
        if (Time.time > AttackTime + cooldown)
        {
            EndState();
        }
    }

    public override void EndState()
    {
        ClearActionTiles();
        turnManager.normalState.StartState();
    }

    void Move(Unit unit)
    {
        unit.tile.Highlight(AttackingColor);
        actionTiles.Add(unit.tile);

        AttackTime = Time.time;

        if (CheckForAttacks(unit))
        {
            AttackTime = Time.time;
            return;
        }

        Unit targetUnit = FindClosestEnemy(unit);
        if (targetUnit == null)
        {
            return;
        }
        if (unit.patrolRange - targetUnit.tile.pathFindingRange - 1 <= unit.moveRange)
        {
            turnManager.gridController.MoveUnit(unit, targetUnit.tile.pathTile.gridPos);
            targetUnit.tile.pathTile.Highlight(AttackingColor);
            actionTiles.Add(targetUnit.tile.pathTile);
            CheckForAttacks(unit);
        }
    }

    private bool CheckForAttacks(Unit unit)
    {
        Vector2Int newTilePos = unit.gridPos;
        newTilePos.x++;
        if (CheckForEnemy(newTilePos, unit))
            return true;
        newTilePos.x -= 2;
        if (CheckForEnemy(newTilePos, unit))
            return true;
        newTilePos.x++;
        newTilePos.y++;
        if (CheckForEnemy(newTilePos, unit))
            return true;
        newTilePos.y -= 2;
        if (CheckForEnemy(newTilePos, unit))
            return true;
        return false;
    }

    private bool CheckForEnemy(Vector2Int Pos, Unit unit)
    {
        if (turnManager.gridController.DoesTileExist(Pos) == false)
            return false;

        Unit DemonUnit = turnManager.gridController.TileGrid[Pos.x, Pos.y].unit;
        if (DemonUnit == null || DemonUnit.enemy == true)
        {
            return false;
        }

        //TODO: animacje ataku i otrzymywania obrazen
        unit.Attack(DemonUnit);

        DemonUnit.tile.Highlight(AttackingColor);
        actionTiles.Add(DemonUnit.tile);
        DemonUnit.TakeDamage(unit.damage);

        return true;
    }

    Unit FindClosestEnemy(Unit unit)
    {
        List<Tile> visitedTiles = new List<Tile>();
        Queue<Tile> tilesToCheck = new Queue<Tile>();
        tilesToCheck.Enqueue(unit.tile);
        visitedTiles.Add(unit.tile);
        unit.tile.pathFindingRange = unit.patrolRange;
        unit.tile.visited = true;

        while (tilesToCheck.Count > 0)
        {
            foreach (Tile tile in turnManager.gridController.FindNeighbours(tilesToCheck.Peek().gridPos))
            {
                if (tile.obstacle || tile.visited || (tile.unit != null && tile.unit.enemy))
                    continue;

                tile.pathTile = tilesToCheck.Peek();
                tile.visited = true;
                tile.pathFindingRange = tilesToCheck.Peek().pathFindingRange - 1;
                visitedTiles.Add(tile);

                if (tile.unit != null && tile.unit.enemy == false)
                {
                    ResetTiles(visitedTiles);
                    return tile.unit;
                }

                if (tile.pathFindingRange > 0)
                    tilesToCheck.Enqueue(tile);

            }
            tilesToCheck.Dequeue();
        }
        ResetTiles(visitedTiles);
        return null;
    }

    private void ResetTiles(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            tile.visited = false;
        }
    }

    private void ClearActionTiles()
    {
        foreach (Tile tile in actionTiles)
        {
            tile.Highlight(Color.black);
        }
        actionTiles.Clear();
    }
}
