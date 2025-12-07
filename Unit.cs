using UnityEngine;

namespace Assets.Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        private Vector3 targetPosition;
        [SerializeField] private float moveSpeed = 5f;
        float stoppingDistance = 0.1f;
        private void Start()
        {
            targetPosition = transform.position;
        }
        private void Update()
        {
            if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
            {
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                transform.position += moveDirection * Time.deltaTime * moveSpeed;
                transform.forward = moveDirection;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Move(targetPosition: Mouseworld.GetPosition());
            }
        }
        private void Move(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }
    }
}
