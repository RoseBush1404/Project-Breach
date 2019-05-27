using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] Character selectedCharacter;

    CharacterUI characterUI;
    Waypoint currentWaypoint;
    bool taskInAction = false;

    private void Start()
    {
        RegisterForMouseEvent();

        characterUI = FindObjectOfType<CharacterUI>();
    }

    private void RegisterForMouseEvent()
    {
        CameraRaycaster cameraRaycaster = GetComponent<CameraRaycaster>();
        cameraRaycaster.onMouseOverPlayerCharacter += OnMouseOverPlayerCharacter;
        cameraRaycaster.onMouseOverWalkableTiles += OnMouseOverWalkableTile;
        cameraRaycaster.onMouseOverWorld += OnMouseOverWorld;
    }

    public void TaskStarted()
    {
        taskInAction = true;
    }

    public void TaskFinished()
    {
        taskInAction = false;
        selectedCharacter = null;
    }

    public void DeselectCharacter()
    {
        selectedCharacter = null;
        characterUI.ResetUI();
        FindObjectOfType<WaypointManager>().SetAllChangedTilesToDefaultTile();
    }

    private void OnMouseOverPlayerCharacter(Character character)
    {
        if(Input.GetMouseButtonDown(0) && character.GetCanBeSelected())
        {
            selectedCharacter = character;
            characterUI.DisplayCharacterUI(selectedCharacter);
            selectedCharacter.PlotMovement();
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
                TaskStarted();
                selectedCharacter.Move(waypoint);
                DeselectCharacter();
            }
        }
    }

    private void OnMouseOverWorld()
    {
        if(Input.GetMouseButtonDown(0) && selectedCharacter != null && !taskInAction)
        {
            DeselectCharacter();
        }
    }
}
