using Assets.Assets.Scripts.Actions;
using Assets.Assets.Scripts.GameSystems;
using Assets.Assets.Scripts.Grid;
using UnityEngine;
using System;
namespace Assets.Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        private const int ACTION_POINTS_MAX = 2;
        public static event EventHandler OnAnyActionPointsChanged;
        [SerializeField] private bool isNPC;
        [SerializeField] private bool isEnemy;
        [Header("References")]
        [SerializeField] private GameObject portrait;
        private GridPosition gridPosition;
        public UnitAnimationSet animationSet;
        private UnitAnimationController animationController;

        // Actions
        private MoveAction moveAction;
        private SpinAction spinAction;
        private BaseAction[] baseActionArray;

        [Header("Unit Stats")]
        [SerializeField] private int actionPoints = ACTION_POINTS_MAX;

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

            TurnSystem.Instance.OnTurnChanged += GetTurnSystem_OnTurnChanged;
            UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;

        }


        private void Update()
        {

            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                LevelGrid.Instance.UnitMoveGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }
        }
        public GridPosition GetGridPosition() => gridPosition;
        public UnitAnimationController GetAnimationController() => animationController;
        public bool CanSpendActionPointsToTakeAction(BaseAction baseAction) => actionPoints >= baseAction.GetActionPointsCost();
        private void SpendActionPoints(int amount)
        {
            actionPoints -= amount;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
        private void GetTurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if ((IsNPC() && !TurnSystem.Instance.IsPlayerTurn()) ||
                (!IsNPC() && TurnSystem.Instance.IsPlayerTurn()))
            {
                actionPoints = ACTION_POINTS_MAX;
                OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
        {
            if (CanSpendActionPointsToTakeAction(baseAction))
            {
                SpendActionPoints(baseAction.GetActionPointsCost());
                return true;
            }
            else
            {
                return false;
            }
        }
        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
        {
            if (portrait == null)
            {
                return;
            }
            if (UnitActionSystem.Instance.GetSelectedUnit() == this)
            {
                portrait.gameObject.SetActive(true);
            }
            else
            {
                portrait.gameObject.SetActive(false);
            }
        }
        public void Damage(int damageAmount) 
        {
            Debug.Log(IsNPC() ? "NPC Unit took " + damageAmount + " damage." : "Player Unit took " + damageAmount + " damage.");
        }
        // Expose Actions
        public int GetActionPoints() => actionPoints;
        public MoveAction GetMoveAction() => moveAction;
        public SpinAction GetSpinAction() => spinAction;
        public BaseAction[] GetBaseActionArray() => baseActionArray;
        public bool IsNPC() => isNPC;
        public bool IsEnemy() => isEnemy;
    }
}
