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
    [CreateAssetMenu(menuName = ("my Game/AI/ShootingAbilityTask"))]
    public class ShootingAbilityTask : AITask
    {
        Character nearestCharacter = null;

        public override int GetScore(GameObject attachedGameObject)
        {
            if (!attachedGameObject.GetComponent<Abilities>().ContainsAbility(typeof(ShootingConfig))) { return 0; }
            if (!attachedGameObject.GetComponent<ShootingBehaviour>().GetCanUseAbility()) { return 0; }

            Waypoint myWaypoint = attachedGameObject.GetComponent<Character>().GetCurrentWaypoint();
            int nearestCharacterRange = 99;
            nearestCharacter = null;

            Character[] characters = FindObjectsOfType<Character>();
            foreach (Character character in characters)
            {
                if (character.GetTeamType() == TeamType.Player)
                {
                    if (character.GetCharacterState() != Character.CharacterState.dead)
                    {
                        var path = Pathfinder.Instance.GetPath(myWaypoint, character.GetCurrentWaypoint(), 99);
                        if (path.Count <= nearestCharacterRange)
                        {
                            nearestCharacterRange = path.Count;
                            nearestCharacter = character;
                        }
                        //make sure that all waypoints in the path are disabled
                        foreach (Waypoint waypoint in path)
                        {
                            WaypointManager.Instance.DisableSingleSpriteNode(waypoint);
                        }
                    }
                }
            }
            
            //score distance bewtween you and the nearest character
            int distanceScore = 10 - nearestCharacterRange;
            //score enemy health
            int healthScore = Mathf.RoundToInt(nearestCharacter.GetComponent<HealthSystem>().GetMaxHealth() - nearestCharacter.GetComponent<HealthSystem>().GetCurrentHealth());

            return healthScore + distanceScore;
        }

        public override void ExecuteTask(GameObject attachedGameObject)
        {
            if (!attachedGameObject.GetComponent<Abilities>().ContainsAbility(typeof(ShootingConfig))) { EnemyController.Instance.TaskFinished(); return; }
            if (!attachedGameObject.GetComponent<ShootingBehaviour>().GetCanUseAbility()) { EnemyController.Instance.TaskFinished(); return; }

            attachedGameObject.GetComponent<ShootingBehaviour>().AimTowardsPoint(nearestCharacter.transform.position);
            int index = attachedGameObject.GetComponent<Abilities>().GetAbilityIndexByType(typeof(ShootingConfig));
            if(index != -1)
            {
                attachedGameObject.GetComponent<Abilities>().AIAttempAbility(index);
            }
            else
            {
                EnemyController.Instance.TaskFinished();
            }
        }
    }
}
