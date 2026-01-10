using System.Collections.Generic;
using UnityEngine;
using Assets.Assets.Scripts.Grid;

public class GridCellHighlight : MonoBehaviour
{
    public static GridCellHighlight Instance { get; private set; }

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private float cellSize;
    private MeshCollider meshCollider;
    private Material material;
    private List<GridPosition> currentGridPositions;
    private int clickedQuadIndex = -1;
    private void Awake()
    {
        Instance = this;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = gameObject.AddComponent<MeshCollider>();
        material = GetComponent<MeshRenderer>().material;
    }

    public void ShowCells(List<GridPosition> gridPositions, float cellSize)
    {
        this.cellSize = cellSize;
        currentGridPositions = gridPositions;
        BuildMesh(gridPositions);

        meshCollider.sharedMesh = null;
        if (vertices.Length > 0)
        {
            meshCollider.sharedMesh = mesh;
        }
    }

    public void Hide()
    {
        mesh.Clear();
        meshCollider.sharedMesh = null;
    }
    public void UpdateHover(Vector3 mouseWorldPosition)
    {
        if (currentGridPositions == null)
        {
            material.SetInt("_HoveredQuad", -1);
            return;
        }

        GridPosition mouseGridPosition =
            LevelGrid.Instance.GetGridPosition(mouseWorldPosition);

        int index = currentGridPositions.IndexOf(mouseGridPosition);
        material.SetInt("_HoveredQuad", index);
    }

    public void SetActionColor(Color color)
    {
        material.SetColor("_ActionColor", color);
    }
    public void SetActionTexture(Texture texture)
    {
        material.SetTexture("_ActionTex", texture);
    }

    public void SetDefaultTexture(Texture texture) 
    {
        material.SetTexture("_MainTex", texture);
    }
    public void ConfirmActionAt(GridPosition gridPosition)
    {
        if (currentGridPositions == null)
            return;

        clickedQuadIndex = currentGridPositions.IndexOf(gridPosition);
        material.SetInt("_ClickedQuad", clickedQuadIndex);

        CancelInvoke(nameof(ClearClick));
        Invoke(nameof(ClearClick), 0.2f);
    }

    private void ClearClick()
    {
        clickedQuadIndex = -1;
        material.SetInt("_ClickedQuad", -1);
    }
    private void BuildMesh(List<GridPosition> positions)
    {
        int quadCount = positions.Count;
        vertices = new Vector3[quadCount * 4];
        triangles = new int[quadCount * 6];
        Vector2[] uvs = new Vector2[quadCount * 4];

        for (int i = 0; i < positions.Count; i++)
        {
            GridPosition pos = positions[i];
            Vector3 cellWorldPos = LevelGrid.Instance.GetWorldPosition(pos);

            int v = i * 4;
            int t = i * 6;

            // quad vertices (XZ plane)
            vertices[v + 0] = cellWorldPos + new Vector3(0, 0.05f, 0);
            vertices[v + 1] = cellWorldPos + new Vector3(0, 0.05f, cellSize);
            vertices[v + 2] = cellWorldPos + new Vector3(cellSize, 0.05f, 0);
            vertices[v + 3] = cellWorldPos + new Vector3(cellSize, 0.05f, cellSize);

            // UVs for the quad (standard 0-1 square)
            uvs[v + 0] = new Vector2(0, 0);
            uvs[v + 1] = new Vector2(0, 1);
            uvs[v + 2] = new Vector2(1, 0);
            uvs[v + 3] = new Vector2(1, 1);

            // triangles
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + 1;
            triangles[t + 2] = v + 2;

            triangles[t + 3] = v + 2;
            triangles[t + 4] = v + 1;
            triangles[t + 5] = v + 3;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }


}