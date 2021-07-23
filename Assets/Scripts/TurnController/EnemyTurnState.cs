using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : TurnState
{
    public TurnManager turnManager;
    public int maxEnemiesMoving = 8;
    public int enemiesWithMove;
    public float cooldown = 0.2f;
    public Color AttackingColor;
    float lastAttack = -100;
    List<Unit> Enemies;
    List<Tile> actionTiles = new List<Tile>();
    public override void StartState()
    {
        turnManager.currentState = this;
        Enemies = FindEnemyUnits();
        enemiesWithMove = Mathf.Min(Enemies.Count, maxEnemiesMoving);
    }

    public override void UpdateState()
    {
        if(enemiesWithMove == 0)
        {
            if (Time.time > lastAttack + cooldown)
            {
                EndState();
                return;
            }
        }
        else if(Time.time > lastAttack + cooldown)
        {
            ClearActionTiles();

            int randomIndex = Random.Range(0, Enemies.Count);
            Move(Enemies[randomIndex]);
            Enemies.RemoveAt(randomIndex);
            enemiesWithMove--;
        }
    }

    public override void EndState()
    {
        ClearActionTiles();
        if (!turnManager.INKWIZYTORturn)
            turnManager.normalState.StartState();
        else
            turnManager.INKWIZYTORState.StartState();
        turnManager.ResetTurn();
    }

    void Move(Unit unit)
    {
        if (unit.damage == 0)
            return;

        unit.tile.Highlight(AttackingColor);
        actionTiles.Add(unit.tile);

        if (CheckForAttacks(unit))
        { 
            lastAttack = Time.time;
            return;
        }

        Unit targetUnit = FindClosestEnemy(unit);
        if(targetUnit == null)
        {
            return;
        }
        lastAttack = Time.time;
        if (unit.patrolRange - targetUnit.tile.pathFindingRange - 1 <= unit.moveRange)
        {
            turnManager.gridController.MoveUnit(unit, targetUnit.tile.pathTile.gridPos);
            targetUnit.tile.pathTile.Highlight(AttackingColor);
            actionTiles.Add(targetUnit.tile.pathTile);
            CheckForAttacks(unit);
        }
        else
        {
            Tile moveTile = targetUnit.tile;
            while(unit.patrolRange - moveTile.pathFindingRange > unit.moveRange && moveTile != unit.tile)
            {
                moveTile = moveTile.pathTile;
            }
            if (unit.tile == moveTile)
                return;
            turnManager.gridController.MoveUnit(unit, moveTile.gridPos);
            moveTile.Highlight(AttackingColor);
            actionTiles.Add(moveTile);
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

    List<Unit> FindEnemyUnits()
    {
        List<Unit> list = new List<Unit>();
        foreach(Tile tile in turnManager.gridController.TileGrid)
        {
            if(tile.unit != null && tile.unit.enemy && tile.unit != turnManager.INKWIZYTOR)
            {
                list.Add(tile.unit);
            }
        }
        return list;
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
            foreach( Tile tile in turnManager.gridController.FindNeighbours(tilesToCheck.Peek().gridPos))
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
        foreach(Tile tile in actionTiles)
        {
            tile.Highlight(Color.black);
        }
        actionTiles.Clear();
    }
}
