using System.Collections;
using System.Collections.Generic;
using Breach.Controler;
using UnityEngine;
using Breach.Placeable.Pathfinding;
using Breach.Manager.Pathfinding;
using Breach.Placeable.Component;
using Breach.Controler;

namespace Breach.Placeable.Characters.AI
{
    public class AITaskSelecter : MonoBehaviour
    {
        [SerializeField] AITask[] movementTasks;
        [SerializeField] AITask[] abilityTasks;

        [SerializeField] int minRangeFromTarget = 3;
        [SerializeField] int maxRangeFromTarget = 10;

        public int  GetMinRangeFromTarget()
        {
            return minRangeFromTarget;
        }

        public IEnumerator SelectMovementTask()
        {
            if (movementTasks.Length > 0 && IsEnemyInRange())
            {
                EnemyController.Instance.TaskStarted();

                int[] taskScores;
                int topScore = 0;
                int topScoreIndex = 0;

                taskScores = new int[movementTasks.Length];
                for (int i = 0; i < movementTasks.Length; i++)
                {
                    taskScores[i] = movementTasks[i].GetScore(this.gameObject);
                    if (taskScores[i] > topScore)
                    {
                        topScore = taskScores[i];
                        topScoreIndex = i;
                    }
                }

                movementTasks[topScoreIndex].ExecuteTask(this.gameObject);

                while(EnemyController.Instance.GetTaskInAction() == true)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        public IEnumerator SelectAbilityTask()
        {
            if (abilityTasks.Length > 0 && IsEnemyInRange())
            {
                EnemyController.Instance.TaskStarted();

                int[] taskScores;
                int topScore = 0;
                int topScoreIndex = 0;

                taskScores = new int[abilityTasks.Length];
                for (int i = 0; i < abilityTasks.Length; i++)
                {
                    taskScores[i] = abilityTasks[i].GetScore(this.gameObject);
                    if (taskScores[i] > topScore)
                    {
                        topScore = taskScores[i];
                        topScoreIndex = i;
                    }
                }

                abilityTasks[topScoreIndex].ExecuteTask(this.gameObject);

                while (EnemyController.Instance.GetTaskInAction() == true)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        private bool IsEnemyInRange()
        {
            Waypoint myWaypoint = gameObject.GetComponent<Character>().GetCurrentWaypoint();
            Character[] characters = FindObjectsOfType<Character>();

            foreach (Character character in characters)
            {
                if (character.GetTeamType() == TeamType.Player)
                {
                    var path = Pathfinder.Instance.GetPath(myWaypoint, character.GetCurrentWaypoint(), 99);
                    if (path.Count <= maxRangeFromTarget)
                    {
                        //a enemy character is within range
                        foreach (Waypoint waypoint in path)
                        {
                            WaypointManager.Instance.DisableSingleSpriteNode(waypoint);
                        }
                        return true;
                    }
                    //make sure that all waypoints in the path are disabled
                    foreach (Waypoint waypoint in path)
                    {
                        WaypointManager.Instance.DisableSingleSpriteNode(waypoint);
                    }
                }
            }
            //no enemy characters within range
            return false;
        }
    }
}