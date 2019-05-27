using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Component;
using Breach.Placeable.Characters;
using Breach.Manager;
using Breach.Manager.TurnSystem;
using Breach.UI;
using Breach.Placeable.Pathfinding;
using Breach.Manager.Pathfinding;

namespace Breach.Controler
{
    public class PlayerController : SingletionBase<PlayerController>
    {// TODO refactor all of this, ect the changing of tiles and deselection of characters and objects

        [SerializeField] GameObject selectedObject;
        [SerializeField] ICharacter selectedCharacter;

        ProfileUI profileUI;
        Waypoint currentWaypoint;
        bool taskInAction = false;
        bool hasControl = true;

        public void Init()
        {
            RegisterForMouseEvent();
            RegisterForTurnSystemEvent();
            profileUI = FindObjectOfType<ProfileUI>();
        }

        private void RegisterForTurnSystemEvent()
        {
            TurnSystem.Instance.OnPlayerUpkeep += NewTurn;
            TurnSystem.Instance.OnEnemyUpkeep += DeselectObject;
        }

        private void RegisterForMouseEvent()
        {
            CameraRaycaster.Instance.onMouseOverSelectable += OnMouseOverSelectable;
            CameraRaycaster.Instance.onMouseOverWalkableTiles += OnMouseOverWalkableTile;
            CameraRaycaster.Instance.onMouseOverWorld += OnMouseOverWorld;
        }

        private void NewTurn()
        {
            DeselectObject();
        }

        public void TaskStarted()
        {
            taskInAction = true;
        }

        public void TaskFinished()
        {
            taskInAction = false;
            if (selectedCharacter != null)
            {
                if (!selectedCharacter.GetMoveAction() && !selectedCharacter.GetAbilityAction())
                {
                    DeselectObject();
                }
                else
                {
                    profileUI.DisplayProfileUI(selectedObject);
                    if (selectedCharacter.GetMoveAction())
                    {
                        WaypointManager.Instance.EnableWaypointVisibility();
                        selectedCharacter.PlotMovement();
                    }
                    if (selectedCharacter.GetAbilityAction())
                    {

                    }
                }
            }
        }

        public bool IsTaskInAction()
        {
            return taskInAction;
        }

        public void SetControl(bool hasControl)
        {
            this.hasControl = hasControl;
        }

        public bool GetControl()
        {
            return hasControl;
        }

        public void DeselectObject()
        {
            selectedObject = null;
            selectedCharacter = null;
            if (profileUI != null)
            {
                profileUI.ResetUI();
            }
            WaypointManager.Instance.SetAllChangedTilesToDefaultSprite();
            WaypointManager.Instance.DisableWaypointVisibility();
        }

        private void OnMouseOverSelectable(GameObject selectable)
        {
            if (Input.GetMouseButtonDown(0) && taskInAction == false && hasControl == true)
            {
                DeselectObject();
                selectedObject = selectable;
                profileUI.DisplayProfileUI(selectedObject);

                //check to see if selected object is one of the player's characters
                ICharacter character = selectable.GetComponent<ICharacter>();
                if (character != null)
                {
                    ITeam team = selectable.GetComponent<ITeam>();
                    if (team.GetTeamType() == TeamType.Player)
                    {
                        selectedCharacter = character;
                        if (selectedCharacter.GetMoveAction())
                        {
                            WaypointManager.Instance.EnableWaypointVisibility();
                            selectedCharacter.PlotMovement();
                        }
                    }
                }
            }
        }

        private void OnMouseOverWalkableTile(Waypoint waypoint)
        {
            if (selectedCharacter != null && !taskInAction)
            {
                if (currentWaypoint != waypoint)
                {
                    currentWaypoint = waypoint;
                    selectedCharacter.PlotMovement(waypoint);
                }

                if (Input.GetMouseButtonDown(0) && selectedCharacter != null)
                {
                    selectedCharacter.Move(waypoint);
                    WaypointManager.Instance.SetAllChangedTilesToDefaultSprite();
                    WaypointManager.Instance.DisableWaypointVisibility();
                }
            }
        }

        private void OnMouseOverWorld()
        {
            if (Input.GetMouseButtonDown(0) && selectedObject != null && !taskInAction)
            {
                DeselectObject();
                WaypointManager.Instance.SetPathOfSprites();
            }
        }
    }
}