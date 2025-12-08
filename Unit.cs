using Assets.Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        private GridPosition gridPosition;
        private Vector3 targetPosition;

        [Header("Stats")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 10f;
        float stoppingDistance = 0.1f;


        private bool isMoving = false;
        public bool IsMoving => isMoving;
        private void Awake()
        {
            targetPosition = transform.position;
        }
        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        }

        private void Update()
        {
            float dist = Vector3.Distance(transform.position, targetPosition);

            if (dist > stoppingDistance)
            {
                if (!isMoving)
                {
                    isMoving = true;
                    animator.CrossFadeInFixedTime("Walk_Forward", .1f);
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
                    animator.CrossFadeInFixedTime("idle", .1f);
                }
            }
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if(newGridPosition != gridPosition)
            {
                LevelGrid.Instance.UnitMoveGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }

        }

        public void Move(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }
    }
}
