using Assets.Assets.Scripts.Actions;
using Assets.Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        [Header("References")]
        private GridPosition gridPosition;
        public UnitAnimationSet animationSet;
        private UnitAnimationController animationController;
        // Actions
        private MoveAction moveAction;
        private SpinAction spinAction;
        private BaseAction[] baseActionArray;

        private void Awake()
        {
            animationController = GetComponent<UnitAnimationController>();
            moveAction = GetComponent<MoveAction>();
            spinAction = GetComponent<SpinAction>();
            baseActionArray = GetComponents<BaseAction>();
        }
        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

            animationController.Init(animationSet);
            animationController.Play(AnimationState.Idle);
        }

        private void Update()
        {
            
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if(newGridPosition != gridPosition)
            {
                LevelGrid.Instance.UnitMoveGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }
        }
        public GridPosition GetGridPosition() => gridPosition;
        public UnitAnimationController GetAnimationController() => animationController;
        // Expose Actions
        public MoveAction GetMoveAction() => moveAction;
        public SpinAction GetSpinAction() => spinAction;
        public BaseAction[] GetBaseActionArray() => baseActionArray;
    }
}
