using UnityEngine;
using Assets.Assets.Scripts.Grid;
using System.Collections.Generic;
using Assets.Assets.Scripts;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridGroundMesh : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private float yOffset = 0f;
    private List<GridPosition> currentGridPositions;
    private Mesh mesh;
    private MeshCollider meshCollider;
    private Material material;
    public static GridGroundMesh Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        material = GetComponent<MeshRenderer>().material;

        meshCollider = gameObject.AddComponent<MeshCollider>();

        GenerateMesh();
        meshCollider.sharedMesh = mesh;
    }
    private void Update()
    {
        UpdateHover(Mouseworld.GetPosition());
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

    public void ClearHover()
    {
        material.SetInt("_HoveredQuad", -1);
    }
    public void SetVisible(bool visible)
    {
        GetComponent<MeshRenderer>().enabled = visible;
    }
    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        currentGridPositions = new List<GridPosition>(width * height);
        int quadCount = width * height;

        Vector3[] vertices = new Vector3[quadCount * 4];
        int[] triangles = new int[quadCount * 6];
        Vector2[] uvs = new Vector2[quadCount * 4];

        int quadIndex = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                currentGridPositions.Add(new GridPosition(x, z));
                Vector3 origin = new Vector3(x * cellSize, yOffset, z * cellSize);

                int v = quadIndex * 4;
                int t = quadIndex * 6;

                vertices[v + 0] = origin;
                vertices[v + 1] = origin + new Vector3(0, 0, cellSize);
                vertices[v + 2] = origin + new Vector3(cellSize, 0, 0);
                vertices[v + 3] = origin + new Vector3(cellSize, 0, cellSize);

                uvs[v + 0] = new Vector2(0, 0);
                uvs[v + 1] = new Vector2(0, 1);
                uvs[v + 2] = new Vector2(1, 0);
                uvs[v + 3] = new Vector2(1, 1);

                triangles[t + 0] = v + 0;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + 2;

                triangles[t + 3] = v + 2;
                triangles[t + 4] = v + 1;
                triangles[t + 5] = v + 3;

                quadIndex++;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}
