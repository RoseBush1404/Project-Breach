using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Manager;
using Breach.Placeable;
using Breach.Placeable.Characters;
using Breach.Placeable.Characters.AI;

namespace Breach.Controler
{
    public class EnemyController : SingletionBase<EnemyController>
    {
        private AITaskSelecter[] aiTaskSelecters;
        private bool TaskInAction = false;

        public void Init()
        {
            Character[] characters = FindObjectsOfType<Character>();
            aiTaskSelecters = new AITaskSelecter[characters.Length];
            for(int i = 0; i < characters.Length; i++)
            {
                aiTaskSelecters[i] = characters[i].GetComponent<AITaskSelecter>();
            }
        }

        public void TaskStarted()
        {
            TaskInAction = true;
        }

        public void TaskFinished()
        {
            TaskInAction = false;
        }

        public bool GetTaskInAction()
        {
            return TaskInAction;
        }

        public IEnumerator AITaskLoop()
        {
            for (int i = 0; i < aiTaskSelecters.Length; i++)
            {
                BreachObject breachObject = aiTaskSelecters[i].GetComponent<BreachObject>();
                Character character = aiTaskSelecters[i].GetComponent<Character>();
                if(breachObject.GetTeamType() == Placeable.Component.TeamType.Enemy && character.GetCharacterState() != Character.CharacterState.dead)
                {
                    //go through all tasks on the AI
                    yield return StartCoroutine(aiTaskSelecters[i].SelectMovementTask());
                    yield return StartCoroutine(aiTaskSelecters[i].SelectAbilityTask());
                }
            }
            print("enemy controller finished looping through all enemies");
        }
    }
}