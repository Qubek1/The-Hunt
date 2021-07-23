using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallAbility : TurnState
{
    public TurnManager turnmanager;
    Tile mouseTile;
    public Color color;
    public float cost;
    public int damage;
    public GameObject fireBallPrefab;
    public Unit unit;
    public float delay;
    float delayTime;
    List<Tile> abilityTiles;

    public override void StartState()
    {
        turnmanager.mouseOnUI = false;
        mouseTile = null;
        turnmanager.currentState.EndState();
        turnmanager.currentState = this;
    }

    public override void UpdateState()
    {
        if (unit.attacked == true)
        {
            if (Time.time > delayTime)
            {
                foreach (Tile tile in abilityTiles)
                {
                    if (tile.unit != null)
                    {
                        tile.unit.TakeDamage(damage);
                    }
                }
                EndState();
                turnmanager.normalState.StartState();
            }
            return;
        }

        CheckMouse();
        if (Input.GetMouseButtonDown(0))
        {
            if (turnmanager.currentMana > 0)
            {
                UseAbility();
            }
            else
            {
                EndState();
                turnmanager.normalState.StartState();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            EndState();
            turnmanager.normalState.StartState();
        }
    }

    public override void EndState()
    {
        if (mouseTile != null)
        {
            foreach (Tile tile in GetAbilityTiles(mouseTile))
            {
                tile.Highlight(Color.black);
            }
        }
    }

    private void CheckMouse()
    {
        Tile newMouseTile = turnmanager.GetMouseTile();
        if (newMouseTile != mouseTile)
        {
            if (mouseTile != null)
            {
                abilityTiles = GetAbilityTiles(mouseTile);
                foreach (Tile tile in abilityTiles)
                {
                    tile.Highlight(Color.black);
                }
            }
            if (newMouseTile != null)
            {
                abilityTiles = GetAbilityTiles(newMouseTile);
                foreach (Tile tile in abilityTiles)
                {
                    tile.Highlight(color);
                }
            }
            mouseTile = newMouseTile;
        }
    }

    private void UseAbility()
    {
        unit.attacked = true;
        unit.animator.SetTrigger("Ability");
        turnmanager.currentMana -= cost;

        abilityTiles = GetAbilityTiles(mouseTile);

        Instantiate(fireBallPrefab, mouseTile.transform.position, new Quaternion(0, 0, 0, 0));

        foreach (Tile tile in abilityTiles)
        {
            tile.Highlight(Color.black);
        }

        delayTime = Time.time + delay;
    }

    private List<Tile> GetAbilityTiles(Tile tile)
    {
        List<Tile> selectedTiles = new List<Tile>();
        for(int x = tile.gridPos.x -1; x <= tile.gridPos.x + 2; x++)
        {
            for(int y = tile.gridPos.y - 1; y <= tile.gridPos.y + 2; y++)
            {
                if (!turnmanager.gridController.DoesTileExist(new Vector2(x, y)))
                {
                    continue;
                }
                Tile selectedTile = turnmanager.gridController.TileGrid[x, y];
                if(selectedTile != null && !selectedTile.obstacle && !(selectedTile.unit != null && selectedTile.unit.spellImmune))
                {
                    selectedTiles.Add(selectedTile);
                }
            }
        }
        return selectedTiles;
    }
}
