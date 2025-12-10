using UnityEngine;

namespace Assets.Assets.Scripts.Actions
{
    public class SpinAction : BaseAction
    {
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
                Debug.Log("done spinning");
            }
        }
        public void Spin()
        {
            isActive = true;
            totalSpinAmount = 0f;
            Debug.Log("spinning...");
        }
    }
}
