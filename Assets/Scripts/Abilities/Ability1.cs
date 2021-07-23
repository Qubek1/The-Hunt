using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability1 : TurnState
{
    public TurnManager turnmanager;
    Tile mouseTile;
    public Color color;
    public float cost;
    public int damage;
    public GameObject spikesPrefab;
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
        if(unit.attacked == true)
        {
            if(Time.time > delayTime)
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
        if(Input.GetMouseButtonDown(0))
        {
            if(turnmanager.currentMana > 0)
            {
                UseAbility();
            }
            else
            {
                EndState();
                turnmanager.normalState.StartState();
            }
        }
        if(Input.GetMouseButtonDown(1))
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
        if(newMouseTile != mouseTile)
        {
            if(mouseTile != null)
            {
                abilityTiles = GetAbilityTiles(mouseTile);
                foreach (Tile tile in abilityTiles)
                {
                    tile.Highlight(Color.black);
                }
            }
            if(newMouseTile != null)
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
        foreach(Tile tile in abilityTiles)
        {
            tile.Highlight(Color.black);
            Instantiate(spikesPrefab, tile.transform.position, new Quaternion(0, 0, 0, 0));
        }

        delayTime = Time.time + delay;
    }

    private List<Tile> GetAbilityTiles(Tile tile)
    {
        List<Tile> selectedTiles = new List<Tile>();
        if(!tile.obstacle && !(tile.unit != null && tile.unit.spellImmune))
            selectedTiles.Add(tile);
        Tile[] neighboursTiles = turnmanager.gridController.FindNeighbours(tile.gridPos);
        foreach (Tile nTile in neighboursTiles)
        {
            if(!nTile.obstacle && !(nTile.unit != null && nTile.unit.spellImmune))
                selectedTiles.Add(nTile);
        }
        return selectedTiles;
    }
}
