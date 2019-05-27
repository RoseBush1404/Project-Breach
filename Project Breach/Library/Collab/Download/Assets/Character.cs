using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour {

    [SerializeField] PlayerController playerController;
    [SerializeField] Color32 characterColor = Color.black; // TODO change to image;

    public enum CharacterState { Idle, Moving, Shooting, dead };
    CharacterState characterState = CharacterState.Idle;

    bool hasMoveAction = true;
    bool hasAbilityAction = true;
    bool canBeSelected = true;

    CharacterMovement characterMovement;
    Abilities abilities;

    //-------------------------------------------------------------

    private void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        abilities = GetComponent<Abilities>();
        TurnSystem turnSystem = FindObjectOfType<TurnSystem>();
        turnSystem.onNewPlayerTurn += NewTurn;
    }

    public CharacterState GetCharacterState()
    {
        return characterState;
    }

    public void TaskFinished()
    {
        characterState = CharacterState.Idle;
        if (playerController != null)
        {
            playerController.TaskFinished();
        }
    }

    void NewTurn(int currentTurn)
    {
        hasMoveAction = true;
        hasAbilityAction = true;
    }

    public bool GetCanBeSelected()
    {
        return canBeSelected;
    }

    public Color32 GetCharacterColor()
    {
        return characterColor;
    }

    //-------------------------------------------------------------

    public void Move(Waypoint waypoint)
    {
        if (hasMoveAction)
        {
            characterState = CharacterState.Moving;
            characterMovement.Move(waypoint);
        }
    }

    public void PlotMovement(Waypoint waypoint = null)
    {
        if (hasMoveAction)
        {
            characterMovement.PlotMovement(waypoint);
        }
    }

    public void UseMoveAction()
    {
        hasMoveAction = false;
    }

    //-------------------------------------------------------------

    public void AbilityPressed(int abilityIndex)
    {
        if (hasAbilityAction)
        {
            characterState = CharacterState.Shooting;
            playerController.TaskStarted();
            FindObjectOfType<WaypointManager>().SetAllChangedTilesToDefaultTile();
            print(gameObject.name);
            abilities.AttempAbility(abilityIndex);
        }
    }

    public void UseAbilityAction()
    {
        hasAbilityAction = false;
    }

    //-------------------------------------------------------------

    public void HasDied()
    {
        characterState = CharacterState.dead;
        canBeSelected = false;
        playerController.DeselectCharacter();
    }
}
