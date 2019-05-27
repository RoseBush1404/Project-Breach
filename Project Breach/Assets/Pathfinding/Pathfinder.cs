using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Pathfinding;

namespace Breach.Manager.Pathfinding
{
    public class Pathfinder : SingletionBase<Pathfinder>
    {

        Dictionary<Vector2, Waypoint> grid = new Dictionary<Vector2, Waypoint>();
        Queue<Waypoint> queue = new Queue<Waypoint>();
        private List<Waypoint> path = new List<Waypoint>();

        Waypoint searchCenter;
        Waypoint startWaypoint;
        Waypoint endWaypoint;
        int movementRange;
        bool pathHasBeenFound = false;

        Vector2Int[] directions = {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,-1),
        new Vector2Int(-1,1)
    };

        //-----------------------------------------------------------

        public List<Waypoint> GetPath(Waypoint startingWaypoint, Waypoint endingWaypoint, int characterRange)
        {
            WaypointManager.Instance.SetTileToInRange(startingWaypoint);
            StoreInformation(startingWaypoint, endingWaypoint, characterRange);
            if (path.Count == 0)
            {
                CalculatePath();
            }
            ResetPathfinder();
            return path;
        }

        //-----------------------------------------------------------

        private void StoreInformation(Waypoint startingWaypoint, Waypoint endingWaypoint, int characterRange)
        {
            startWaypoint = startingWaypoint;
            if (startingWaypoint != endingWaypoint)
            {
                endWaypoint = endingWaypoint;
            }
            movementRange = characterRange;
            path.Clear();
        }

        //-----------------------------------------------------------

        private void CalculatePath()
        {
            LoadBlocks();
            BreadthFirstSearch();
            if (pathHasBeenFound)
            {
                CreatePath();
            }
            WaypointManager.Instance.SetPathOfSprites(path);
        }

        //-----------------------------------------------------------

        private void CreatePath()
        {
            SetAsPath(endWaypoint);

            Waypoint previous = endWaypoint.exploredFrom;
            while (previous != startWaypoint)
            {
                SetAsPath(previous);
                previous = previous.exploredFrom;
            }

            SetAsPath(startWaypoint);
            path.Reverse();
        }

        //-----------------------------------------------------------

        private void SetAsPath(Waypoint waypoint)
        {
            path.Add(waypoint);
        }

        //-----------------------------------------------------------

        private void BreadthFirstSearch()
        {
            queue.Enqueue(startWaypoint);

            while (queue.Count > 0)
            {
                searchCenter = queue.Dequeue();
                HasEndBeenFound();
                ExploreNeighbours();
                searchCenter.isExplored = true;
            }
        }

        //-----------------------------------------------------------

        private void HasEndBeenFound()
        {
            if (startWaypoint.characterOnTile.GetTeamType() == Placeable.Component.TeamType.Enemy)
            {
                if (searchCenter == endWaypoint)
                {
                    pathHasBeenFound = true;
                    WaypointManager.Instance.SetTileToSelectedTile(endWaypoint);
                }
            }
            else
            {
                if (searchCenter == endWaypoint && searchCenter.isSelectable == true)
                {
                    pathHasBeenFound = true;
                    WaypointManager.Instance.SetTileToSelectedTile(endWaypoint);
                }
            }
        }

        //-----------------------------------------------------------

        private void ExploreNeighbours()
        {
            foreach (Vector2 direction in directions)
            {
                Vector2 neighbourCoordinates = searchCenter.GetGridPos() + direction;
                if (grid.ContainsKey(neighbourCoordinates))
                {
                    QueueNewNeighbours(neighbourCoordinates, direction);
                }
            }
        }

        //-----------------------------------------------------------

        private void QueueNewNeighbours(Vector2 neighbourCoordinates, Vector2 direction)
        {
            Waypoint neighbour = grid[neighbourCoordinates];
            if (neighbour.isExplored || queue.Contains(neighbour))
            {
                //do nothing
            }
            else
            {
                // TODO consider performens issues
                List<Vector2> neighbourValidDirections = neighbour.GetValidDirections();
                List<Vector2> searchCenterValidDirection = searchCenter.GetValidDirections();
                if (searchCenter.rangeFromCharacter + 1 <= movementRange && neighbourValidDirections.Contains(direction) && searchCenterValidDirection.Contains(direction))
                {
                    WaypointManager.Instance.SetTileToInRange(neighbour);
                    neighbour.exploredFrom = searchCenter;
                    neighbour.rangeFromCharacter = searchCenter.rangeFromCharacter + 1;
                    neighbour.direction = direction;
                    queue.Enqueue(neighbour);
                }
            }
        }

        //-----------------------------------------------------------

        private void LoadBlocks()
        {
            var waypoints = FindObjectsOfType<Waypoint>();
            foreach (Waypoint waypoint in waypoints)
            {
                var gridPos = waypoint.GetGridPos();
                if (grid.ContainsKey(gridPos))
                {
                    Debug.LogWarning("skipping overlapping block " + waypoint);
                }
                else
                {
                    grid.Add(waypoint.GetGridPos(), waypoint);
                }
            }
        }

        //-----------------------------------------------------------

        private void ResetPathfinder()
        {
            startWaypoint = null;
            endWaypoint = null;
            pathHasBeenFound = false;
            queue.Clear();
            grid.Clear();
            WaypointManager.Instance.SetAllChangedTilesToDefaultSettings();
        }

        //-----------------------------------------------------------

    }
}