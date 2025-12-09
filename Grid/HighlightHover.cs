using UnityEngine;

public class HighlightHover : MonoBehaviour
{
    MeshRenderer rend;
    Material mat;

    Color normalColor;
    [SerializeField] Color hoverColor = new Color(1f, 0.8f, 0.2f, 1f);

    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        mat = rend.material;
        normalColor = mat.GetColor("_BaseColor");
    }

    void OnMouseEnter()
    {
        mat.SetColor("_BaseColor", hoverColor);
    }

    void OnMouseExit()
    {
        mat.SetColor("_BaseColor", normalColor);
    }
}
