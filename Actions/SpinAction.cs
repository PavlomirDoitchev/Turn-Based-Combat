using System;
using UnityEngine;

namespace Assets.Assets.Scripts.Actions
{
    public class SpinAction : BaseAction
    {
        public delegate void SpinCompleteDelegate();
        private float totalSpinAmount;
        private void Update()
        {
            if (!isActive)
            {
                return;
            }
            transform.Rotate(Vector3.up, 360 * Time.deltaTime);
            totalSpinAmount += 360 * Time.deltaTime;
            if (totalSpinAmount >= 360f)
            {
                isActive = false;
                onActionComplete();
            }
        }
        public void Spin(Action onActionComplete)
        {
            this.onActionComplete = onActionComplete;
            isActive = true;
            totalSpinAmount = 0f;
        }
    }
}
