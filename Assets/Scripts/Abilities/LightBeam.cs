using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : TurnState
{
    public TurnManager turnmanager;
    Tile mouseTile;
    public Color color;
    public Color HLcolor;
    public float cost;
    public int damage;
    public GameObject lightBeamPrefab;
    public Unit unit;
    public float delay;
    float delayTime;
    List<Tile> abilityTiles;
    List<Tile> HighlightedTiles;

    public override void StartState()
    {
        turnmanager.mouseOnUI = false;
        mouseTile = null;
        turnmanager.currentState.EndState();
        turnmanager.currentState = this;
        HighlightedTiles = new List<Tile>();
        HighlightAbilityTiles();
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
            if (turnmanager.currentMana > 0 && HighlightedTiles.Contains(mouseTile))
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
            List<Tile> abilityTiles = GetAbilityTiles(mouseTile);
            if (abilityTiles != null)
            {
                foreach (Tile tile in abilityTiles)
                {
                    tile.Highlight(Color.black);
                }
            }
        }
        foreach(Tile tile in HighlightedTiles)
        {
            tile.Highlight(Color.black);
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
                if (abilityTiles != null)
                {
                    foreach (Tile tile in abilityTiles)
                    {
                        tile.Highlight(HLcolor);
                    }
                }
            }
            if (newMouseTile != null)
            {
                abilityTiles = GetAbilityTiles(newMouseTile);
                if (abilityTiles != null)
                {
                    foreach (Tile tile in abilityTiles)
                    {
                        tile.Highlight(color);
                    }
                }
            }
            mouseTile = newMouseTile;
        }
    }

    private void UseAbility()
    {
        unit.attacked = true;
        unit.animator.SetTrigger("Ability");
        unit.LookAt(unit.transform.position, mouseTile.transform.position);
        turnmanager.currentMana -= cost;

        abilityTiles = GetAbilityTiles(mouseTile);

        foreach (Tile tile in abilityTiles)
        {
            GameObject lbp = Instantiate(lightBeamPrefab, tile.transform.position, new Quaternion(0, 0, 0, 0));
            if (tile.gridPos.x != unit.gridPos.x)
                lbp.transform.Rotate(lbp.transform.up, 90);
            tile.Highlight(Color.black);
        }
        foreach (Tile tile in HighlightedTiles)
        {
            tile.Highlight(Color.black);
        }

        delayTime = Time.time + delay;
    }

    private List<Tile> GetAbilityTiles(Tile tile)
    {
        List<Tile> selectedTiles = new List<Tile>();

        if(!HighlightedTiles.Contains(tile))
        {
            return null;
        }

        /*if (selectedTile != null && !selectedTile.obstacle && !(selectedTile.unit != null && selectedTile.unit.spellImmune))
        {
            selectedTiles.Add(selectedTile);
        }*/

        foreach(Tile aTile in HighlightedTiles)
        {
            if (aTile.unit != null && aTile.unit.spellImmune)
                continue;
            if(aTile.gridPos.x == tile.gridPos.x && (aTile.gridPos.y - unit.gridPos.y) * (tile.gridPos.y - unit.gridPos.y) > 0)
            {
                selectedTiles.Add(aTile);
            }
            if(aTile.gridPos.y == tile.gridPos.y && (aTile.gridPos.x - unit.gridPos.x) * (tile.gridPos.x - unit.gridPos.x) > 0)
            {
                selectedTiles.Add(aTile);
            }
        }
        
        return selectedTiles;
    }

    private void HighlightAbilityTiles()
    {
        Vector2Int moveVector = new Vector2Int(1, 0);
        for(int i=0; i<4; i++)
        {
            Tile tile = unit.tile;
            while(!tile.obstacle)
            {
                Vector2Int newTilePos = new Vector2Int(tile.gridPos.x + moveVector.x, tile.gridPos.y + moveVector.y);
                if (turnmanager.gridController.DoesTileExist(newTilePos))
                    tile = turnmanager.gridController.TileGrid[newTilePos.x, newTilePos.y];
                else
                    break;

                if (tile.obstacle)
                    break;
                HighlightedTiles.Add(tile);
                tile.Highlight(HLcolor);
            }
            int oldX = moveVector.x;
            int oldY = moveVector.y;
            moveVector.x = oldY;
            moveVector.y = -oldX;
        }
    }
}
