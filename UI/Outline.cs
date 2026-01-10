using UnityEngine;

public class Outline : MonoBehaviour
{
    [SerializeField] private RenderingLayerMask outlineLayer;

    private Renderer[] renderers;
    private uint originalLayer;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalLayer = renderers[0].renderingLayerMask;
    }

    public void Enable()
    {
        foreach (var r in renderers)
            r.renderingLayerMask = originalLayer | outlineLayer;
    }

    public void Disable()
    {
        foreach (var r in renderers)
            r.renderingLayerMask = originalLayer;
    }
}
