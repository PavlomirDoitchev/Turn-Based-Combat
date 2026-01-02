using UnityEngine;
using System.Collections.Generic;
using Assets.Assets.Scripts;
namespace Assets.Assets.Scripts.Grid
{
    public class Testing : MonoBehaviour
    {
        private void Start()
        {
            
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T)) 
            {
                GridPosition mouseGrid = LevelGrid.Instance.GetGridPosition(Mouseworld.GetPosition());
                GridPosition start = new GridPosition(0, 0);
                List<GridPosition> grid = Pathfinding.Instance.FindPath(start, mouseGrid);

                for (int i = 0; i < grid.Count - 1; i++) 
                {
                    Debug.DrawLine(LevelGrid.Instance.GetWorldPosition(grid[i]) + Vector3.up, LevelGrid.Instance.GetWorldPosition(grid[i + 1]) + Vector3.up, Color.red, 10f);
                }
            }
        }
    }
}
