using UnityEngine;
using System.Collections.Generic;

namespace Assets.Assets.Scripts.Grid
{
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding Instance { get; private set; }
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private LayerMask obstacleLayerMask;

        private int _width, _height;
        private float _cellSize;

        private GridSystem<PathNode> gridSystem;

        private void Awake()
        {
            if(Instance != null)
            {
                Debug.LogError("There is more than one Pathfinding instance!");
                Destroy(gameObject);
                return;
            }
            Instance = this;

         
        }
        public void Setup(int width, int height, float cellSize) 
        {
            this._width = width;
            this._height = height;
            this._cellSize = cellSize;

            gridSystem = new GridSystem<PathNode>(_width, _height, _cellSize, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

            for(int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    float offSet = 5f;
                    if (Physics.Raycast(
                        worldPosition + Vector3.down * offSet,
                        Vector3.up,
                        offSet * 2f,
                        obstacleLayerMask))
                    {
                        GetNode(x, z).SetIsWalkable(false);
                    }

                }
            }
        }
        public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition)
        {
            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();

            PathNode startNode = gridSystem.GetGridObject(startGridPosition);
            PathNode endNode = gridSystem.GetGridObject(endGridPosition);
            openList.Add(startNode);
            for (int x = 0; x < gridSystem.GetWidth(); x++)
            {
                for (int z = 0; z < gridSystem.GetHeight(); z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                    pathNode.SetGCost(int.MaxValue);
                    pathNode.SetHCost(0);
                    pathNode.UpdateFCost();

                    pathNode.ResetCameFromPathNode();
                }
            }
            startNode.SetGCost(0);
            startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
            startNode.UpdateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostPathNode(openList);
                if (currentNode == endNode)
                {
                    // Reached final node
                    return CalculatePath(endNode);
                }

                // switch currentNode from openList to closedList
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) 
                {
                    if (closedList.Contains(neighbourNode))
                        continue;

                    if (!neighbourNode.IsWalkable()) 
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                    if(tentativeGCost < neighbourNode.GetGCost())
                    {
                        neighbourNode.SetCameFromPathNode(currentNode);
                        neighbourNode.SetGCost(tentativeGCost);
                        neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                        neighbourNode.UpdateFCost();

                        if(!openList.Contains(neighbourNode))
                            openList.Add(neighbourNode);
                    }
                }
            }
            // No path found
            return null;
        }
        public int CalculateDistance(GridPosition a, GridPosition b)
        {
            GridPosition gridDistance = a - b;
            int xDistance = Mathf.Abs(gridDistance.x);
            int zDistance = Mathf.Abs(gridDistance.z);
            int remaining = Mathf.Abs(xDistance - zDistance);   
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
        }
        private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostPathNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
                {
                    lowestFCostPathNode = pathNodeList[i];
                }
            }
            return lowestFCostPathNode;
        }
        private PathNode GetNode(int x, int z)
        {
            return gridSystem.GetGridObject(new GridPosition(x, z));
        }
        private List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();
            GridPosition gridPosition = currentNode.GetGridPosition();

            if (gridPosition.x - 1 >= 0)
            {
                // Left
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z));
                if (gridPosition.z - 1 >= 0)
                {
                    // Diagonal Left Down
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
                }

                if (gridPosition.z + 1 < gridSystem.GetHeight())
                {
                    // Diagonal Left Up
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
                }
            }

            if (gridPosition.x + 1 < gridSystem.GetWidth())
            {
                // Right
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z));
                if (gridPosition.z - 1 >= 0)
                {
                    // Diagonal Right Down
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
                }

                if (gridPosition.z + 1 < gridSystem.GetHeight())
                {
                    // Diagonal Right Up
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
                }
            }

            if (gridPosition.z - 1 >= 0)
            {
                // Down
                neighbourList.Add(GetNode(gridPosition.x, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // Up
                neighbourList.Add(GetNode(gridPosition.x, gridPosition.z + 1));
            }



            return neighbourList;
        }
        private List<GridPosition> CalculatePath(PathNode endNode)
        {
            List<PathNode> pathNodeList = new List<PathNode>();
            pathNodeList.Add(endNode);
            PathNode currentNode = endNode;
            while(currentNode.GetCameFromPathNode() != null)
            {
                pathNodeList.Add(currentNode.GetCameFromPathNode());
                currentNode = currentNode.GetCameFromPathNode();
            }
            pathNodeList.Reverse();
            List<GridPosition> gridPositionList = new List<GridPosition>();
            foreach (PathNode pathNode in pathNodeList)
            {
                gridPositionList.Add(pathNode.GetGridPosition());
            }
            return gridPositionList;
        }
    }

}
