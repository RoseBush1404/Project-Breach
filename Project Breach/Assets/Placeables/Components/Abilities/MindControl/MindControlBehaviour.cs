using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Characters;
using Breach.Controler;
using Breach.Placeable.Pathfinding;

namespace Breach.Placeable.Component
{
    public class MindControlBehaviour : AbilityBehaviour
    {
        TeamType thisTeam;
        bool isOverlayActive = false;
        List<GameObject> overlays = new List<GameObject>();
        List<Character> charactersInRange = new List<Character>();

        //int numberOfTurnsControlled = 0;
        //BreachObject affectCharacter;
        //TeamType formerTeam;

        public override void Use()
        {
            CameraRaycaster.Instance.onMouseOverSelectable += TakeControlOfCharacter;
            CameraRaycaster.Instance.onMouseOverWalkableTiles += InvalidTarget;
            CameraRaycaster.Instance.onMouseOverWorld += InvalidTarget;
        }

        public override void AIUse()
        {
            // Not yet implemented
        }

        private void TakeControlOfCharacter(GameObject selectableHit)
        {
            Character selectedCharacter = selectableHit.GetComponent<Character>();
            Character character = GetComponent<Character>();
            if (selectedCharacter != null)
            {
                if (charactersInRange.Contains(selectedCharacter))
                {
                    abilityInformation.affectedCharacter = selectableHit.GetComponent<BreachObject>();
                    abilityInformation.affectedTeam = abilityInformation.affectedCharacter.GetTeamType();
                    abilityInformation.affectedCharacter.SetTeamType(thisTeam);
                    abilityInformation.affectedCharacter.GetComponent<ICharacter>().ResetActionPoints();
                    character.UseAbilityAction();
                    SetCurrentCoolDown(config.GetMaxCoolDown());
                }
            }
            CameraRaycaster.Instance.onMouseOverSelectable -= TakeControlOfCharacter;
            CameraRaycaster.Instance.onMouseOverWalkableTiles -= InvalidTarget;
            CameraRaycaster.Instance.onMouseOverWorld -= InvalidTarget;
            charactersInRange.Clear();
            character.TaskFinished();
        }

        private void InvalidTarget(Waypoint waypoint = null)
        {
            CameraRaycaster.Instance.onMouseOverSelectable -= TakeControlOfCharacter;
            CameraRaycaster.Instance.onMouseOverWalkableTiles -= InvalidTarget;
            CameraRaycaster.Instance.onMouseOverWorld -= InvalidTarget;
            charactersInRange.Clear();
            Character character = GetComponent<Character>();
            character.TaskFinished();
        }

        private void InvalidTarget()
        {
            CameraRaycaster.Instance.onMouseOverSelectable -= TakeControlOfCharacter;
            CameraRaycaster.Instance.onMouseOverWalkableTiles -= InvalidTarget;
            CameraRaycaster.Instance.onMouseOverWorld -= InvalidTarget;
            charactersInRange.Clear();
            Character character = GetComponent<Character>();
            character.TaskFinished();
        }

        public override void PlotAbility()
        {
            if(!isOverlayActive)
            {
                SpawnOverlay();
                CameraRaycaster.Instance.onMouseOverSelectable += HighLightCharacter;
            }
        }

        private void SpawnOverlay()
        {
            thisTeam = gameObject.GetComponent<ITeam>().GetTeamType();
            Character[] characters = FindObjectsOfType<Character>();
            foreach (Character character in characters)
            {
                TeamType charTeam = character.GetTeamType();
                if (charTeam != TeamType.Neutral && charTeam != thisTeam)
                {
                    Vector2 thisPos = gameObject.transform.position;
                    Vector2 characterPos = character.transform.position;
                    if ((config as MindControlConfig).GetRangeOfInitControl() >= Vector2.Distance(thisPos, characterPos))
                    {
                        charactersInRange.Add(character);
                        overlays.Add(Instantiate(config.GetOverlay(), character.transform.position, Quaternion.identity));
                    }
                }
            }
            isOverlayActive = true;
        }

        private void HighLightCharacter(GameObject selectableHit)
        {
            Character selectedCharacter = selectableHit.GetComponent<Character>();
            if(selectedCharacter != null)
            {
                if(charactersInRange.Contains(selectedCharacter))
                {
                    //TODO highlight charcter
                }
            }
        }

        public override void DisablePlot()
        {
            CameraRaycaster.Instance.onMouseOverSelectable -= HighLightCharacter;
            foreach (GameObject overlay in overlays)
            {
                Destroy(overlay);
            }
            isOverlayActive = false;
            overlays.Clear();
        }

        public override void PlayerEndStep()
        {
            TeamType teamEndStep = TeamType.Player;
            EndStep(teamEndStep);
        }

        public override void EnemyEndStep()
        {
            TeamType teamEndStep = TeamType.Enemy;
            EndStep(teamEndStep);
        }

        private void EndStep(TeamType teamEndStep)
        {
            if (gameObject.GetComponent<BreachObject>().GetTeamType() == teamEndStep)
            {
                if (abilityInformation.affectedCharacter != null)
                {
                    abilityInformation.numberOfTurnsAffected++;
                    if (abilityInformation.numberOfTurnsAffected >= (config as MindControlConfig).GetMaxNumberOfTurnsControlled())
                    {
                        abilityInformation.affectedCharacter.SetTeamType(abilityInformation.affectedTeam);
                        abilityInformation.affectedCharacter = null;
                        abilityInformation.affectedTeam = TeamType.Neutral;
                        abilityInformation.numberOfTurnsAffected = 0;
                    }
                }
            }
        }
    }
}
