using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.Placeable.Component;
using Breach.Placeable.Pathfinding;
using Breach.Manager.Pathfinding;
using Breach.Manager.TurnSystem;
using Breach.Controler;

namespace Breach.Placeable.Characters
{
    public class Character : BreachObject, ICharacter, ITurn
    {
        public enum CharacterState { Idle, InCover, Moving, Shooting, dead };
        CharacterState characterState = CharacterState.Idle;

        bool hasMoveAction = true;
        bool hasAbilityAction = true;

        CharacterMovement movementComponent;
        Abilities abilityComponent;
        Waypoint currentWaypoint;

        public delegate void CharacterDelegate(Character thisCharacter);
        public event CharacterDelegate OnCharacterMove;
        public event CharacterDelegate OnCharacterDeath;

        //-------------------------------------------------------------

        public void Init()
        {
            movementComponent = GetComponent<CharacterMovement>();
            abilityComponent = GetComponent<Abilities>();
            TurnSystem.Instance.OnPlayerUpkeep += PlayerUpkeep;
            TurnSystem.Instance.OnPlayerEndStep += PlayerEndStep;
            TurnSystem.Instance.OnEnemyUpkeep += EnemyUpkeep;
            TurnSystem.Instance.OnEnemyEndStep += EnemyEndStep;
            TaskFinished();
        }

        public CharacterState GetCharacterState()
        {
            return characterState;
        }

        private void TaskStarted()
        {
            if (teamType == TeamType.Player)
            {
                PlayerController.Instance.TaskStarted();
            }
            if(teamType == TeamType.Enemy)
            {
                EnemyController.Instance.TaskStarted();
            }
        }

        public void TaskFinished()
        {
            if (currentWaypoint.CoverOnThisTile.Count > 0)
            {
                characterState = CharacterState.InCover;
            }
            else
            {
                characterState = CharacterState.Idle;
            }

            if (teamType == TeamType.Player)
            {
                PlayerController.Instance.TaskFinished();
            }
            if(teamType == TeamType.Enemy)
            {
                EnemyController.Instance.TaskFinished();
            }
        }

        public void PlayerUpkeep()
        {
            if (teamType == TeamType.Player)
            {
                ResetActionPoints();
            }
            ITurn[] turnComponents = GetComponentsInChildren<ITurn>();
            ITurn thisITurn = gameObject.GetComponent<ITurn>();
            foreach (ITurn component in turnComponents)
            {
                if(component != thisITurn)
                {
                    component.PlayerUpkeep();
                }
            }
        }

        public void PlayerEndStep()
        {
            ITurn[] turnComponents = GetComponentsInChildren<ITurn>();
            ITurn thisITurn = gameObject.GetComponent<ITurn>();
            foreach (ITurn component in turnComponents)
            {
                if (component != thisITurn)
                {
                    component.PlayerEndStep();
                }
            }
        }

        public void EnemyUpkeep()
        {
            if (teamType != TeamType.Player)
            {
                ResetActionPoints();
            }
            ITurn[] turnComponents = GetComponentsInChildren<ITurn>();
            ITurn thisITurn = gameObject.GetComponent<ITurn>();
            foreach (ITurn component in turnComponents)
            {
                if (component != thisITurn)
                {
                    component.EnemyUpkeep();
                }
            }
        }

        public void EnemyEndStep()
        {
            ITurn[] turnComponents = GetComponentsInChildren<ITurn>();
            ITurn thisITurn = gameObject.GetComponent<ITurn>();
            foreach (ITurn component in turnComponents)
            {
                if (component != thisITurn)
                {
                    component.EnemyEndStep();
                }
            }
        }

        public void ResetActionPoints()
        {
            hasMoveAction = true;
            hasAbilityAction = true;
        }

        //-------------------------------------------------------------

        public void Move(Waypoint waypoint)
        {
            if (hasMoveAction && movementComponent != null)
            {
                TaskStarted();
                if(OnCharacterMove != null) { OnCharacterMove(this); }
                characterState = CharacterState.Moving;
                movementComponent.Move(waypoint);
            }
        }

        public void PlotMovement(Waypoint waypoint = null)
        {
            if (hasMoveAction && movementComponent != null)
            {
                movementComponent.PlotMovement(waypoint);
            }
        }

        public void UseMoveAction()
        {
            hasMoveAction = false;
        }

        public bool GetMoveAction()
        {
            return hasMoveAction;
        }

        public void SetCurrentWaypoint(Waypoint waypoint)
        {
            currentWaypoint = waypoint;
        }

        public Waypoint GetCurrentWaypoint()
        {
            return currentWaypoint;
        }

        //-------------------------------------------------------------

        public void AbilityPressed(int abilityIndex)
        {
            if (hasAbilityAction && abilityComponent != null)
            {
                TaskStarted();
                characterState = CharacterState.Shooting;
                WaypointManager.Instance.SetAllChangedTilesToDefaultSprite();
                WaypointManager.Instance.SetPathOfSprites();
                WaypointManager.Instance.DisableWaypointVisibility();
                abilityComponent.AttempAbility(abilityIndex);
            }
        }

        public void UseAbilityAction()
        {
            hasAbilityAction = false;
        }

        public bool GetAbilityAction()
        {
            return hasAbilityAction;
        }

        //-------------------------------------------------------------

        public void HasDied()
        {
            if(OnCharacterDeath != null) { OnCharacterDeath(this); }
            characterState = CharacterState.dead;
            canBeSelected = false;
            GetComponent<CapsuleCollider2D>().enabled = false;
            if (teamType == TeamType.Player)
            {
                PlayerController.Instance.DeselectObject();
            }
        }
    }
}