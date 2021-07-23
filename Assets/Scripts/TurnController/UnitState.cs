using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState : TurnState
{
    public TurnManager turnManager;
    public Color demonMoveColor, demonHLColor, atackColor;
    Unit unit;
    Tile[] moveTiles;
    List<Tile> enemiesTiles = new List<Tile>();
    Tile mouseTile;
    bool attack;
    float attackTime;
    Unit attackedUnit;

    public override void StartState()
    {
        turnManager.currentState = this;

        attack = false;

        mouseTile = null;

        unit = turnManager.GetMouseTile().unit;

        moveTiles = turnManager.gridController.PathFinding(unit.gridPos, unit.currentMoveRange);
        HighlighMove(demonMoveColor);

        unit.tile.Highlight(demonHLColor);
        unit.transform.position += new Vector3(0, 0.2f, 0);
        unit.tile.transform.position += new Vector3(0, 0.2f, 0);

        if (unit.attacked == false)
        {
            DemonUnit dUnit = (DemonUnit)unit;
            dUnit.ShowAbilityButton(true);
            CheckForAttacks();
        }
    }

    public override void UpdateState()
    {
        if(attack)
        {
            if(Time.time > attackTime)
            {
                attackedUnit.TakeDamage(unit.damage);
                if (turnManager.INKWIZYTORturn)
                    turnManager.INKWIZYTORState.StartState();
                else
                    turnManager.normalState.StartState();
            }
            return;
        }
        CheckMouseTile();
        if (Input.GetMouseButtonDown(0) && turnManager.mouseOnUI == false)
        {
            MouseClick();
        }
    }

    public override void EndState()
    {
        DemonUnit dUnit = (DemonUnit)unit;
        dUnit.ShowAbilityButton(false);

        unit.transform.position -= new Vector3(0, 0.2f, 0);
        unit.tile.transform.position -= new Vector3(0, 0.2f, 0);
        if(mouseTile != null)
            UpdateTile(false, mouseTile);
        foreach (Tile tile in moveTiles)
        {
            tile.visited = false;
            tile.Highlight(Color.black);            
        }
        foreach (Tile tile in enemiesTiles)
        {
            tile.visited = false;
            tile.Highlight(Color.black);
        }
    }

    private void CheckForAttacks()
    {
        Vector2Int newTilePos = unit.gridPos;
        newTilePos.x++;
        CheckForEnemy(newTilePos);
        newTilePos.x -= 2;
        CheckForEnemy(newTilePos);
        newTilePos.x++;
        newTilePos.y++;
        CheckForEnemy(newTilePos);
        newTilePos.y -= 2;
        CheckForEnemy(newTilePos);
    }

    private void CheckForEnemy(Vector2Int Pos)
    {
        if (turnManager.gridController.DoesTileExist(Pos) == false)
            return;

        Unit EnemyUnit = turnManager.gridController.TileGrid[Pos.x, Pos.y].unit;
        if (EnemyUnit == null || EnemyUnit.enemy == false)
        {
            return;
        }
        EnemyUnit.tile.Highlight(atackColor);
        enemiesTiles.Add(EnemyUnit.tile);
    }

    private void HighlighMove(Color moveColor)
    {
        foreach (Tile tile in moveTiles)
        {
            tile.Highlight(moveColor);
        }
    }

    private void CheckMouseTile()
    {
        Tile newMouseTile = turnManager.GetMouseTile();
        if(newMouseTile != mouseTile)
        {
            if(mouseTile != null)
                UpdateTile(false, mouseTile);
            if (newMouseTile != null)
                UpdateTile(true, newMouseTile);
        }
        mouseTile = newMouseTile;
    }

    private void MouseClick()
    {
        if(mouseTile == null)
        {
            EndState();
            turnManager.normalState.StartState();
            return;
        }
        if(mouseTile.visited && mouseTile.unit == null)
        {
            unit.currentMoveRange = mouseTile.pathFindingRange;
            EndState();
            turnManager.gridController.MoveUnit(unit, mouseTile.gridPos);
            turnManager.normalState.StartState();
        }
        else if(enemiesTiles.Contains(mouseTile))
        {
            //TODO: animacja ataku i przyjmowania obrazen
            unit.Attack(mouseTile.unit);

            attackedUnit = mouseTile.unit;
            unit.attacked = true;
            attack = true;
            attackTime = Time.time + unit.attackDelay;
            EndState();
        }
        else
        {
            EndState();
            turnManager.normalState.StartState();
        }
    }

    private void UpdateTile(bool t, Tile tile)
    {
        if (tile.visited && tile.unit == null)
        {
            if (t)
                tile.transform.position += new Vector3(0, 0.2f, 0);
            else
                tile.transform.position -= new Vector3(0, 0.2f, 0);
        }
    }
}
