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
        public static event EventHandler OnAnyUnitSpawned;
        public static event EventHandler OnAnyUnitDead;
        [SerializeField] private bool isNPC;
        [SerializeField] private bool isEnemy;
        [Header("References")]
        [SerializeField] private GameObject portrait;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private HealthSystem healthSystem;
        private GridPosition gridPosition;
        public UnitAnimationSet animationSet;
        private UnitAnimationController animationController;

        //// Actions
        //private MoveAction moveAction;
        //private SpinAction spinAction;
        //private AttackAction attackAction;
        //private HealAction healAction;
        private BaseAction[] baseActionArray;
     


        [Header("Unit Stats")]
        [SerializeField] private int actionPoints = ACTION_POINTS_MAX;

        private void Awake()
        {
            healthSystem.Init();
            healthSystem.OnDamaged += HealthSystem_OnDamaged;
            healthSystem.OnDied += HealthSystem_OnDied;
            animationController = GetComponent<UnitAnimationController>();
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
            OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);


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
        private void HealthSystem_OnDamaged()
        {
            if (healthSystem.GetHealth() > 0)
            {
                animationController.Play(AnimationState.Hurt);

            }
        }

        private void HealthSystem_OnDied()
        {
            animationController.Play(AnimationState.Death);
            LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
            this.GetComponent<BoxCollider>().enabled = false;
            UnitActionSystem.Instance.AutoDeSelectUnit();
            OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        }
        private void OnDestroy()
        {
            healthSystem.OnDamaged -= HealthSystem_OnDamaged;
            healthSystem.OnDied -= HealthSystem_OnDied;
        }
        public void Damage(int damageAmount)
        {
            healthSystem.Damage(damageAmount);

            //Debug.Log(IsNPC()
            //    ? $"NPC Unit took {damageAmount} damage."
            //    : $"Player Unit took {damageAmount} damage.");
        }
        public T GetAction<T>() where T : BaseAction
        {
            foreach (BaseAction baseAction in baseActionArray)
            {
                if (baseAction is T)
                {
                    return (T)baseAction;
                }
            }
            return null;
        }
        public int GetActionPoints() => actionPoints;
        public BaseAction[] GetBaseActionArray() => baseActionArray;

        public Vector3 GetWorldPosition()
        {
            return transform.position;
        }


        //#region Expose Actions
        //public MoveAction GetMoveAction() => moveAction;
        //public SpinAction GetSpinAction() => spinAction;
        //public AttackAction GetAttackAction() => attackAction;
        //public HealAction GetHealAction() => healAction;
        //#endregion

        public HealthSystem GetHealthSystem() => healthSystem;
        public bool IsNPC() => isNPC;
        public bool IsEnemy() => isEnemy;

        public Transform GetProjectileSpawnPoint()
        {
            return projectileSpawnPoint != null ? projectileSpawnPoint : transform;
        }
    }
}