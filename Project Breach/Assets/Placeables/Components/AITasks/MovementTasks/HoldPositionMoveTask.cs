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
    [CreateAssetMenu(menuName = ("my Game/AI/HoldPositionMoveTask"))]
    public class HoldPositionMoveTask : AITask
    {
        Character nearestCharacter = null;
        Waypoint myWaypoint = null;

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

            int IdleRange = attachedGameObject.GetComponent<AITaskSelecter>().GetMinRangeFromTarget();
            int Score = 0;
            if (nearestCharacterRange <= IdleRange)
            {
                Score = 5;
            }

            if (attachedGameObject.GetComponent<Character>().GetCurrentWaypoint().CoverOnThisTile.Count >= 1)
            {
                Score += 4;
            }
            
            Debug.Log("Hold Position " + (Score));
            return Score;
        }

        public override void ExecuteTask(GameObject attachedGameObject)
        {
            EnemyController.Instance.TaskFinished();
        }
    }
}
