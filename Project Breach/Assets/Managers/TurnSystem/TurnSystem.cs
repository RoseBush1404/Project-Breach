using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.Controler;
using Breach.Placeable.Characters;

namespace Breach.Manager.TurnSystem
{
    public class TurnSystem : SingletionBase<TurnSystem>
    {
        enum TurnState { PlayerTurn, EnemyTurn };
        TurnState turnState = TurnState.PlayerTurn;

        protected Button nextTurnButton;

        public delegate void TurnSystemDelegate();
        public event TurnSystemDelegate OnPlayerUpkeep;
        public event TurnSystemDelegate OnPlayerEndStep;
        public event TurnSystemDelegate OnEnemyUpkeep;
        public event TurnSystemDelegate OnEnemyEndStep;

        public void NextTurnPressed()
        {
            if (CheckAllCharacterAreIdle()) { return; }

            if(nextTurnButton == null)
            {
                nextTurnButton = GameObject.Find("NextTurnButton").GetComponent<Button>();
            }

            if (PlayerController.Instance.GetControl())
            {
                print("next turn");
                turnState = TurnState.EnemyTurn;
                if (OnPlayerEndStep != null)
                {
                    OnPlayerEndStep();
                }
                nextTurnButton.interactable = false;
                ProcessTurn();
            }
        }

        private bool CheckAllCharacterAreIdle()
        {
            Character[] characters = FindObjectsOfType<Character>();
            foreach (Character character in characters)
            {
                if (character.GetCharacterState() != Character.CharacterState.dead)
                {
                    if (character.GetCharacterState() != Character.CharacterState.Idle)
                    {
                        if (character.GetCharacterState() != Character.CharacterState.InCover)
                        {
                            print("task still taking place");
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void ProcessTurn()
        {
            switch (turnState)
            {
                case TurnState.PlayerTurn:
                    StartPlayerTurn();
                    break;

                case TurnState.EnemyTurn:
                    StartEnemyTurn();
                    break;
            }
        }

        private void StartPlayerTurn()
        {
            PlayerController.Instance.SetControl(true);
            nextTurnButton.interactable = true;
            if (OnPlayerUpkeep != null)
            {
                OnPlayerUpkeep();
            }
            //add anything else that needs to be added
        }

        private void StartEnemyTurn()
        {
            PlayerController.Instance.SetControl(false);
            if (OnEnemyUpkeep != null)
            {
                OnEnemyUpkeep();
            }
            StartCoroutine(ProcessAI());
            // do enemy turn stuff
        }

        // TODO (TEST) REMOVE OR REWORK BEFORE FINISHING
        IEnumerator ProcessAI()
        {
            yield return StartCoroutine(EnemyController.Instance.AITaskLoop());
            print("passing the turn over to the player now");
            turnState = TurnState.PlayerTurn;
            if (OnEnemyEndStep != null)
            {
                // TODO  Sebastian Lague
                OnEnemyEndStep();
            }
            ProcessTurn();
        }
    }
}