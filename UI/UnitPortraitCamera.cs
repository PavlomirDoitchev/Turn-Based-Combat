using Assets.Assets.Scripts;
using UnityEngine;

public class UnitPortraitCamera : MonoBehaviour
{
    [SerializeField] float upOffset = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 2f);
    private Unit currentUnit;

    void LateUpdate()
    {
        if (currentUnit == null) return;

        Transform target = currentUnit.transform;

        // Point camera at unit from offset position
        transform.position = target.position + target.transform.rotation * offset;
        transform.LookAt(target.position + Vector3.up * upOffset);
    }

    public void SetUnit(Unit unit)
    {
        currentUnit = unit;
    }
}
