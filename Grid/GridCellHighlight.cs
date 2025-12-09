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

    private void Awake()
    {
        Instance = this;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void ShowCells(List<GridPosition> gridPositions, float cellSize)
    {
        this.cellSize = cellSize;
        BuildMesh(gridPositions);
    }

    public void Hide()
    {
        mesh.Clear();
    }

    private void BuildMesh(List<GridPosition> positions)
    {
        int quadCount = positions.Count;
        vertices = new Vector3[quadCount * 4];
        triangles = new int[quadCount * 6];

        for (int i = 0; i < positions.Count; i++)
        {
            GridPosition pos = positions[i];
            Vector3 cellWorldPos = LevelGrid.Instance.GetWorldPosition(pos);

            int v = i * 4;
            int t = i * 6;

            // quad vertices
            vertices[v + 0] = cellWorldPos + new Vector3(0, 0.05f, 0);
            vertices[v + 1] = cellWorldPos + new Vector3(0, 0.05f, cellSize);
            vertices[v + 2] = cellWorldPos + new Vector3(cellSize, 0.05f, 0);
            vertices[v + 3] = cellWorldPos + new Vector3(cellSize, 0.05f, cellSize);

            // triangle indices
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
        mesh.RecalculateNormals();
    }
}
