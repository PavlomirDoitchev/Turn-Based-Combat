using UnityEngine;

namespace Assets.Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private Vector3 targetPosition;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 10f;
        float stoppingDistance = 0.1f;

        private bool isMoving = false;
        public bool IsMoving => isMoving;
        private void Awake()
        {
            targetPosition = transform.position;
        }

        private void Update()
        {
            float dist = Vector3.Distance(transform.position, targetPosition);

            if (dist > stoppingDistance)
            {
                if (!isMoving)
                {
                    isMoving = true;
                    animator.CrossFadeInFixedTime("Run_1H_Forward", .1f);
                }

                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                transform.position += moveDirection * Time.deltaTime * moveSpeed;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            }
            else
            {
                if (isMoving)
                {
                    isMoving = false;
                    animator.CrossFadeInFixedTime("Combat_1H_Ready", .1f);
                }
            }

         
        }

        public void Move(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }
    }
}
