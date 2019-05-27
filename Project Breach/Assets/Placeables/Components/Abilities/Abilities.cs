using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Characters;

namespace Breach.Placeable.Component
{
    public class Abilities : MonoBehaviour, ITurn
    {

        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] AnimatorOverrideController animatorOverrideController;

        const string DEFAULT_ABILITY_STATE = " DEFAULT ABILITY";

        Character character;

        public void Init()
        {
            character = GetComponent<Character>();
            AttachInitialAbilities();
        }

        public void SetAbilityAnimation(AnimationClip animation)
        {
            animatorOverrideController[DEFAULT_ABILITY_STATE] = animation;
        }

        public void SetAbilities(AbilityConfig[] abilityConfigs)
        {
            abilities = abilityConfigs;
        }

        void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        public bool ContainsAbility(System.Type type)
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                if (abilities[i].GetType() == type)
                {
                    return true;
                }
            }
            return false;
        }

        #region turn states
        public void PlayerUpkeep()
        {
            foreach(AbilityConfig ability in abilities)
            {
                ability.SetUser(gameObject);
                ability.PlayerUpkeep();
            }
        }

        public void PlayerEndStep()
        {
            foreach (AbilityConfig ability in abilities)
            {
                ability.SetUser(gameObject);
                ability.PlayerEndStep();
            }
        }

        public void EnemyUpkeep()
        {
            foreach (AbilityConfig ability in abilities)
            {
                ability.SetUser(gameObject);
                ability.EnemyUpkeep();
            }
        }

        public void EnemyEndStep()
        {
            foreach (AbilityConfig ability in abilities)
            {
                ability.SetUser(gameObject);
                ability.EnemyEndStep();
            }
        }
        #endregion

        #region Getters
        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        public AbilityConfig GetAbilityAtIndex(int indexPosition)
        {
            return abilities[indexPosition];
        }

        public int GetAbilityIndexByType(System.Type type)
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                if (abilities[i].GetType() == type)
                {
                    return i;
                }
            }
            return -1;
        }

        public AbilityConfig[] GetAllAbilities()
        {
            return abilities;
        }

        public AbilityBehaviour[] GetAllAbilityBehaviours()
        {
            AbilityBehaviour[] abilityBehaviours = new AbilityBehaviour[abilities.Length];
            for(int i = 0; i < abilities.Length; i++)
            {
                abilities[i].SetUser(gameObject);
                abilityBehaviours[i] = abilities[i].GetAbilityBehaviour();
            }
            return abilityBehaviours;
        }
        #endregion

        public void AttempAbility(int abilityIndex, GameObject target = null)
        {
            abilities[abilityIndex].SetUser(gameObject);
            if (abilities[abilityIndex].CanUseAbility())
            {
                StartCoroutine(UseAbility(abilityIndex));
            }
            else
            {
                character.TaskFinished();
            }
        }

        IEnumerator UseAbility(int abilityIndex)
        {
            bool canExit = false;
            //abilities[abilityIndex].SetUser(gameObject);
            while (!canExit)
            {
                yield return new WaitForFixedUpdate();

                abilities[abilityIndex].PlotAbility();
                if (Input.GetMouseButtonDown(0))
                {
                    abilities[abilityIndex].DisablePlot();
                    abilities[abilityIndex].Use();
                    canExit = true;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    abilities[abilityIndex].DisablePlot();
                    character.TaskFinished();
                    canExit = true;
                }
            }
        }

        public void AIAttempAbility(int abilityIndex)
        {
            abilities[abilityIndex].SetUser(gameObject);
            if (abilities[abilityIndex].CanUseAbility())
            {
                AIUseAbility(abilityIndex);
            }
            else
            {
                character.TaskFinished();
            }
        }

        private void AIUseAbility(int abilityIndex)
        {
            abilities[abilityIndex].AIUse();
        }
    }
}