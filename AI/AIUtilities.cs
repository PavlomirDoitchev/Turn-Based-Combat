using Assets.Assets.Scripts.Grid;
using System.Linq;

namespace Assets.Assets.Scripts.AI
{
    public static class AIUtilities
    {
        public static Unit GetClosestPlayer(Unit enemy, GridPosition from)
        {
            return UnitManager.Instance.GetPlayerUnits()
                .OrderBy(p =>
                    Pathfinding.Instance.GetPathLength(from, p.GetGridPosition()))
                .FirstOrDefault();
        }

        public static int GetDistance(GridPosition a, GridPosition b)
        {
            return Pathfinding.Instance.GetPathLength(a, b);
        }
    }
}
