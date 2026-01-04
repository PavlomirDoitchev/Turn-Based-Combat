using UnityEngine;

public class MeshHoverOutline : MonoBehaviour
{
    [SerializeField] private LayerMask characterLayer;

    private Outline lastOutline;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, characterLayer))
        {
            Outline outline = hit.collider.GetComponentInParent<Outline>();

            if (outline == lastOutline)
                return;

            ClearLast();

            if (outline != null)
            {
                outline.Enable();
                lastOutline = outline;
            }
        }
        else
        {
            ClearLast();
        }
    }

    private void ClearLast()
    {
        if (lastOutline != null)
        {
            lastOutline.Disable();
            lastOutline = null;
        }
    }
}
