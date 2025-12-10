using UnityEngine;

public class GridHighlightHover : MonoBehaviour
{
    public Material highlightMaterial;

    private Mesh mesh;
    private int previousQuad = -1;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        if (mesh == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int tri = hit.triangleIndex;
            int quadIndex = tri / 6;

            if (quadIndex != previousQuad)
            {
                HighlightQuad(quadIndex);
                previousQuad = quadIndex;
            }
        }
        else
        {
            ClearHighlight();
            previousQuad = -1;
        }
    }

    void HighlightQuad(int quadIndex)
    {
        // pass quad index to shader
        highlightMaterial.SetInt("_HoveredQuad", quadIndex);
    }

    void ClearHighlight()
    {
        highlightMaterial.SetInt("_HoveredQuad", -1);
    }
}