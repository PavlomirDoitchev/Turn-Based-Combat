using UnityEngine;

namespace Assets.Assets.Scripts
{
    public class Mouseworld : MonoBehaviour
    {
        private static Mouseworld instance;
        [SerializeField] private LayerMask mouseplaneLayerMask;
        private void Awake()
        {
            instance = this;
        }
        
        public static Vector3 GetPosition() 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mouseplaneLayerMask);
            return raycastHit.point;
        }
    }
}
