using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Pathfinding;
using Breach.Manager.Pathfinding;
using Breach.Placeable.Component;
using Breach.Controler;

namespace Breach.Placeable.Characters.AI
{
    //TODO refactor this class
    [CreateAssetMenu(menuName = ("my Game/AI/AdvanceMoveTask"))]
    public class AdvanceMoveTask : AITask
    {
        Character nearestCharacter = null;
        Waypoint myWaypoint = null;

        public override int GetScore(GameObject attachedGameObject)
        {
            myWaypoint = attachedGameObject.GetComponent<Character>().GetCurrentWaypoint();
            int nearestCharacterRange = 99;
            nearestCharacter = null;

            Character[] characters = FindObjectsOfType<Character>();
            foreach(Character character in characters)
            {
                if(character.GetTeamType() == TeamType.Player)
                {
                    var path = Pathfinder.Instance.GetPath(myWaypoint, character.GetCurrentWaypoint(), 99);
                    if(path.Count <= nearestCharacterRange)
                    {
                        nearestCharacterRange = path.Count - 1;
                        nearestCharacter = character;
                    }
                }
            }
            //score distance bewtween you and the nearest character
            int distanceScore = nearestCharacterRange;
            //score your health
            int healthScore = Mathf.RoundToInt(attachedGameObject.GetComponent<HealthSystem>().GetCurrentHealth());

            Debug.Log("Advance " + (distanceScore + healthScore));
            return healthScore + distanceScore;
        }

        public override void ExecuteTask(GameObject attachedGameObject)
        {
            List<Waypoint> path = Pathfinder.Instance.GetPath(myWaypoint, nearestCharacter.GetCurrentWaypoint(), 99);
            List<int> pathScores = new List<int>();

            ScorePath(path, pathScores);
            RemoveScoreTooCloseToTarget(pathScores, attachedGameObject);

            int movementRange = attachedGameObject.GetComponent<CharacterMovement>().GetRange();
            int highestScore = 0;
            Waypoint targetWaypoint = null;

            FindHighestScoreWithinRange(path, pathScores, movementRange, ref highestScore, ref targetWaypoint);
            //targetWaypoint = IsCurrentWaypointBetter(attachedGameObject, highestScore, targetWaypoint);

            if (targetWaypoint != null)
            {
                //move to selected tile
                attachedGameObject.GetComponent<CharacterMovement>().Move(targetWaypoint);
            }
            else
            {
                //else if null then make sure that all the path sprites are disabled
                foreach (Waypoint waypoint in path)
                {
                    WaypointManager.Instance.DisableSingleSpriteNode(waypoint);
                }
                EnemyController.Instance.TaskFinished();
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

        private static void RemoveScoreTooCloseToTarget(List<int> pathScores, GameObject attachedGameObject)
        {
            int minRange = attachedGameObject.GetComponent<AITaskSelecter>().GetMinRangeFromTarget();

            pathScores.Reverse();
            for (int i = 0; i < pathScores.Count; i++)
            {
                if (i < minRange)
                {
                    pathScores[i] = 0;
                }
            }
            pathScores.Reverse();
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
