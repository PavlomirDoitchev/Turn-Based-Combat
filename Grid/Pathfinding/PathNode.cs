namespace Assets.Assets.Scripts.Grid
{
    public class PathNode
    {
        private GridPosition gridPosition;
        private int gCost, hCost, fCost;
        private PathNode cameFromPathNode;
        private bool isWalkable = true; 
        public PathNode(GridPosition gridPosition)
        {
            this.gridPosition = gridPosition;
        }
        public override string ToString()
        {
            return gridPosition.ToString();
        }
        public int GetGCost()
        {
            return gCost;
        }
        public int GetHCost()
        {
            return hCost;
        }
        public int GetFCost()
        {
            return fCost;
        }
        public void SetGCost(int gCost)
        {
            this.gCost = gCost;
            UpdateFCost();
        }
        public void SetHCost(int hCost)
        {
            this.hCost = hCost;
            UpdateFCost();
        }
        public void UpdateFCost()
        {
            fCost = gCost + hCost;
        }
        public void ResetCameFromPathNode()
        {
            cameFromPathNode = null;
        }
        public void SetCameFromPathNode(PathNode cameFromPathNode)
        {
            this.cameFromPathNode = cameFromPathNode;
        }
        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }
        public PathNode GetCameFromPathNode()
        {
            return cameFromPathNode;
        }
        public bool IsWalkable()
        {
            return isWalkable;
        }
        public void SetIsWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
        }
    }
}
