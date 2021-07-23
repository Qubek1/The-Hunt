using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalState : TurnState
{
    public TurnManager turnManager;
    Tile mouseTile = null;
    Tile[] moveTiles;
    public Color demonMoveColor, enemyMoveColor, demonHLColor, enemyHLColor;
    public Button nextTurnButton;

    public override void StartState()
    {
        turnManager.currentState = this;
        mouseTile = null;
        nextTurnButton.interactable = true;
    }
    public override void UpdateState()
    {
        UpdateMouseTile();
        ClickOnTile();
    }

    public override void EndState()
    {
        if (mouseTile != null)
        {
            HighlightTile(false, mouseTile);
        }
        if (moveTiles != null)
        {
            foreach (Tile tile in moveTiles)
            {
                tile.visited = false;
                tile.Highlight(Color.black);
            }
        }
        nextTurnButton.interactable = false;
    }

    private void ClickOnTile()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mouseTile == null)
                return;
            else if (mouseTile.obstacle)
                return;
            else if (mouseTile.unit != null)
            {
                if(mouseTile.unit.enemy == false)
                {
                    EndState();
                    turnManager.unitState.StartState();
                    return;
                }
            }
        }
    }

    private void UpdateMouseTile()
    {
        Tile newMouseTile = turnManager.GetMouseTile();
        if (mouseTile != newMouseTile)
        {
            HighlightTile(true, newMouseTile);
            HighlightTile(false, mouseTile);

            if(mouseTile != null && mouseTile.unit != null)
            {
                HighlighMove(Color.black);
                foreach (Tile tile in moveTiles)
                {
                    tile.visited = false;
                }
            }

            mouseTile = newMouseTile;
            if (mouseTile != null && mouseTile.unit != null)
            {
                moveTiles = turnManager.gridController.PathFinding(mouseTile.unit.gridPos, mouseTile.unit.currentMoveRange);
                if (mouseTile.unit.enemy)
                {
                    HighlighMove(enemyMoveColor);
                    mouseTile.unit.tile.Highlight(enemyHLColor);
                }
                else
                {
                    HighlighMove(demonMoveColor);
                    mouseTile.unit.tile.Highlight(demonHLColor);
                }
            }
        }
    }

    private void HighlighMove(Color moveColor)
    {
        foreach (Tile tile in moveTiles)
        {
            tile.Highlight(moveColor);
        }
    }

    private void HighlightTile(bool t, Tile tile)
    {
        if(t)
        {
            if (tile != null && tile.obstacle == false)
            {
                tile.transform.position += new Vector3(0, 0.2f, 0);
                if(tile.unit != null)
                    tile.unit.transform.position += new Vector3(0, 0.2f, 0);
            }
        }
        else
        {
            if (tile != null && tile.obstacle == false)
            {
                tile.transform.position += new Vector3(0, -0.2f, 0);
                if(tile.unit != null)
                    tile.unit.transform.position += new Vector3(0, -0.2f, 0);
            }
        }
    }

    public void NextTurn()
    {
        EndState();
        turnManager.enemyState.StartState();
    }
}
