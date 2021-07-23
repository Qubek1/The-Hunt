using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool obstacle;
    public Unit unit;
    public bool visited;
    public Vector2Int gridPos;
    public int pathFindingRange;
    public Material material;
    MeshRenderer meshRenderer;
    public Tile pathTile;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Highlight(Color color)
    {
        if (obstacle)
        {
            return;
        }
        foreach (Material material in meshRenderer.materials)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor" ,color);
        }
    }
}
