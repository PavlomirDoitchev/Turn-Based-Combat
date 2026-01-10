using System;
using UnityEngine;

namespace Assets.Assets.Scripts
{
    public class UnitSelectedVisual : MonoBehaviour
    {
        [SerializeField] Unit unit;
        private MeshRenderer mr;

        private void Awake()
        {
            mr = GetComponent<MeshRenderer>();
        }
        private void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            UpdateVisual();

        }
       
        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
        {
            UpdateVisual();
        }
        private void UpdateVisual()
        {
            if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
            {
                mr.enabled = true;
            }
            else
            {
                mr.enabled = false;
            }
        }
    }
}
