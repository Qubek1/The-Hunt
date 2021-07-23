using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public float maxMana;
    public float currentMana;
    public float manaLoss;
    public Slider manaSlider;
    public GridController gridController;
    public TurnState 
        normalState, 
        unitState,
        enemyState,
        INKWIZYTORState;
    public TurnState currentState;
    public Unit INKWIZYTOR;
    public bool INKWIZYTORturn = false;
    bool Winn = false;
    public bool mouseOnUI = false;

    private void Start()
    {
        currentMana = maxMana;
        currentState = normalState;
    }

    void Update()
    {
        if (Winn)
            return;

        currentState.UpdateState();
        
        currentMana -= Time.deltaTime * manaLoss;
        if(currentMana < 0)
        {
            currentMana = 0;
            manaSlider.value = 0;
            manaSlider.transform.GetChild(2).gameObject.SetActive(false);

            //TODO: INWKIZYTOR
            INKWIZYTORturn = true;
            INKWIZYTOR.gameObject.SetActive(true);
        }
        if(currentMana > 0)
        {
            manaSlider.value = currentMana / maxMana;
        }

        Tile mouseTile = GetMouseTile();
        if(mouseTile != null && mouseTile.unit != null)
            mouseTile.unit.HBC.Show(0.05f);
    }

    public Tile GetMouseTile()
    {
        Vector3 l = Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized;
        Vector3 l0 = -Camera.main.transform.position;
        Vector3 n = new Vector3(0, 1, 0);
        float d = Vector3.Dot(l0, n) / Vector3.Dot(l, n);
        Vector3 TileWorldPos = ((-l0 + l * d) + new Vector3(1, 0, 1))/2f;
        if (TileWorldPos.x < 0 || TileWorldPos.z < 0)
            return null;
        Vector2Int TilePos = new Vector2Int((int)TileWorldPos.x, (int)TileWorldPos.z);
        //Debug.Log(TilePos);
        if(gridController.DoesTileExist(TilePos))
            return gridController.TileGrid[TilePos.x, TilePos.y];
        return null;
    }

    public void ResetTurn()
    {
        foreach (Tile tile in gridController.TileGrid)
        {
            if(tile.unit != null)
            {
                tile.unit.ResetUnit();
            }
        }
    }

    public void WIN()
    {
        Winn = true;
        currentState.EndState();
        currentState = null;
    }

    public void MouseOnUI(bool t)
    {
        mouseOnUI = t;
    }
}
