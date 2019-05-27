using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Breach.Placeable.Pathfinding;
using Breach.Manager.Pathfinding;
using Breach.Placeable.Component;
using Breach.Controler;

namespace Breach.Placeable.Characters.AI
{
    //TODO refactor this class
    [CreateAssetMenu(menuName = ("my Game/AI/RetreatMoveTask"))]
    public class RetreatMoveTask : AITask
    {
        Character nearestCharacter = null;
        Waypoint myWaypoint = null;

        protected enum DirectionToMove { Left, Right, Up, Down };
        DirectionToMove directionToMove = DirectionToMove.Up;

        public override int GetScore(GameObject attachedGameObject)
        {
            myWaypoint = attachedGameObject.GetComponent<Character>().GetCurrentWaypoint();
            int nearestCharacterRange = 99;

            Character[] characters = FindObjectsOfType<Character>();
            foreach (Character character in characters)
            {
                if (character.GetTeamType() == TeamType.Player)
                {
                    var path = Pathfinder.Instance.GetPath(myWaypoint, character.GetCurrentWaypoint(), 99);
                    if (path.Count <= nearestCharacterRange)
                    {
                        nearestCharacterRange = path.Count - 1;
                        nearestCharacter = character;
                    }
                }
            }
            //score distance bewtween you and the nearest character
            int minRange = attachedGameObject.GetComponent<AITaskSelecter>().GetMinRangeFromTarget();
            int distanceScore = minRange - nearestCharacterRange;
            if(distanceScore > 0)
            {
                distanceScore = distanceScore * 6;
            }
            //score your health
            int healthScore = Mathf.RoundToInt(nearestCharacter.GetComponent<HealthSystem>().GetMaxHealth() - nearestCharacter.GetComponent<HealthSystem>().GetCurrentHealth());

            Debug.Log("Retreat " + (distanceScore + healthScore));
            return healthScore + distanceScore;
        }

        public override void ExecuteTask(GameObject attachedGameObject)
        {
            List<Waypoint> waypoints = new List<Waypoint>();

            //select direction to retreat from enemy
            SetDirectionToMove(attachedGameObject);
            //find all valid waypoints
            FindWaypoints(attachedGameObject, waypoints);

            //order list from farthest to nearest
            waypoints = waypoints.OrderBy(x => Vector2.Distance(attachedGameObject.transform.position, x.transform.position)).ToList();
            waypoints.Reverse();

            //select tile and move to it
            SelectTileToPathToo(attachedGameObject, waypoints);
        }

        private void SelectTileToPathToo(GameObject attachedGameObject, List<Waypoint> waypoints)
        {
            foreach (Waypoint waypoint in waypoints)
            {
                List<Waypoint> path = Pathfinder.Instance.GetPath(myWaypoint, waypoint, 99);
                List<int> pathScores = new List<int>();

                if (path.Count != 0 || path.Count <= attachedGameObject.GetComponent<CharacterMovement>().GetRange())
                {
                    ScorePath(path, pathScores);

                    int movementRange = attachedGameObject.GetComponent<CharacterMovement>().GetRange();
                    int highestScore = 0;
                    Waypoint targetWaypoint = null;

                    FindHighestScoreWithinRange(path, pathScores, movementRange, ref highestScore, ref targetWaypoint);
                    //targetWaypoint = IsCurrentWaypointBetter(attachedGameObject, highestScore, targetWaypoint);

                    if (targetWaypoint != null)
                    {
                        //move to selected tile
                        attachedGameObject.GetComponent<CharacterMovement>().Move(targetWaypoint);
                        return;
                    }
                    else
                    {
                        //else if null then make sure that all the path sprites are disabled
                        foreach (Waypoint waypointInPath in path)
                        {
                            WaypointManager.Instance.DisableSingleSpriteNode(waypointInPath);
                        }
                        EnemyController.Instance.TaskFinished();
                        return;
                    }
                }
            }
        }

        private void FindWaypoints(GameObject attachedGameObject, List<Waypoint> waypoints)
        {
            int characterPosX = Mathf.FloorToInt(attachedGameObject.GetComponent<Character>().GetCurrentWaypoint().transform.position.x);
            int characterPosY = Mathf.FloorToInt(attachedGameObject.GetComponent<Character>().GetCurrentWaypoint().transform.position.y);
            int range = attachedGameObject.GetComponent<CharacterMovement>().GetRange();

            if (directionToMove == DirectionToMove.Right || directionToMove == DirectionToMove.Left)
            {
                for (int i = -range; i <= range; i++)
                {
                    int tilePosY = characterPosY + i;
                    int tilePosX = 0;
                    for (int j = 0; j <= range; j++)
                    {
                        if (directionToMove == DirectionToMove.Right)
                        {
                            tilePosX = characterPosX + j;
                        }
                        else
                        {
                            tilePosX = characterPosX - j;
                        }

                        GameObject waypointGO = GameObject.Find("(" + tilePosX + ", " + tilePosY + ")");
                        if (waypointGO != null)
                        {
                            waypoints.Add(waypointGO.GetComponent<Waypoint>());
                        }
                        if ((Mathf.Abs(i) + j) >= range)
                        {
                            break;
                        }
                    }
                }
                waypoints.Remove(attachedGameObject.GetComponent<Character>().GetCurrentWaypoint());
            }
            else if (directionToMove == DirectionToMove.Down || directionToMove == DirectionToMove.Up)
            {
                for (int i = -range; i <= range; i++)
                {
                    int tilePosX = characterPosX + i;
                    int tilePosY = 0;
                    for (int j = 0; j <= range; j++)
                    {
                        if (directionToMove == DirectionToMove.Up)
                        {
                            tilePosY = characterPosY + j;
                        }
                        else
                        {
                            tilePosY = characterPosY - j;
                        }

                        GameObject waypointGO = GameObject.Find("(" + tilePosX + ", " + tilePosY + ")");
                        if (waypointGO != null)
                        {
                            waypoints.Add(waypointGO.GetComponent<Waypoint>());
                        }
                        if ((Mathf.Abs(i) + j) >= range)
                        {
                            break;
                        }
                    }
                }
                waypoints.Remove(attachedGameObject.GetComponent<Character>().GetCurrentWaypoint());
            }
        }

        private void SetDirectionToMove(GameObject attachedGameObject)
        {
            int TargetPosX = Mathf.FloorToInt(nearestCharacter.GetCurrentWaypoint().transform.position.x);
            int TargetPosY = Mathf.FloorToInt(nearestCharacter.GetCurrentWaypoint().transform.position.y);

            int characterPosX = Mathf.FloorToInt(attachedGameObject.GetComponent<Character>().GetCurrentWaypoint().transform.position.x);
            int characterPosY = Mathf.FloorToInt(attachedGameObject.GetComponent<Character>().GetCurrentWaypoint().transform.position.y);

            //if the target isn't on the same x axis
            if (TargetPosX != characterPosX)
            {
                if (TargetPosX > characterPosX)
                {
                    directionToMove = DirectionToMove.Left;
                }
                else
                {
                    directionToMove = DirectionToMove.Right;
                }
            }
            else
            {
                if (TargetPosY > characterPosY)
                {
                    directionToMove = DirectionToMove.Down;
                }
                else
                {
                    directionToMove = DirectionToMove.Up;
                }
            }
        }

        private static Waypoint IsCurrentWaypointBetter(GameObject attachedGameObject, int highestScore, Waypoint targetWaypoint)
        {
            int currentWaypointScore = 0;
            Waypoint currentWaypoint = attachedGameObject.GetComponent<Character>().GetCurrentWaypoint();
            if (currentWaypoint.CoverOnThisTile.Count > 0)
            {
                currentWaypointScore += 2;
            }
            if (currentWaypointScore > highestScore)
            {
                targetWaypoint = null;
            }

            return targetWaypoint;
        }

        private static void FindHighestScoreWithinRange(List<Waypoint> path, List<int> pathScores, int movementRange, ref int highestScore, ref Waypoint targetWaypoint)
        {
            for (int i = 0; i < movementRange; i++)
            {
                if (i < path.Count)
                {
                    if (pathScores[i] > highestScore)
                    {
                        highestScore = pathScores[i];
                        targetWaypoint = path[i];
                    }
                }
            }
        }

        private static void ScorePath(List<Waypoint> path, List<int> pathScores)
        {
            for (int i = 0; i < path.Count; i++)
            {
                pathScores.Add(i);
                if (path[i].CoverOnThisTile.Count > 0)
                {
                    pathScores[i] += 2;
                }
                if (path[i].characterOnTile != null)
                {
                    pathScores[i] = 0;
                }
            }
        }
    }
}