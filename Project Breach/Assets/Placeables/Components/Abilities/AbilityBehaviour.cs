using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    public abstract class AbilityBehaviour : MonoBehaviour, ITurn
    {
        protected const string USE_ABILITY_TRIGGER = "UseAbility";
        protected const string ABILITY_FINISHED_BOOL = "AbilityFinished";
        protected Animator animator;
        protected AbilityConfig config;
        public AbilityInformation abilityInformation;

        private void Awake()
        {
            animator = gameObject.GetComponent<Animator>();
            abilityInformation = new AbilityInformation();
            abilityInformation.Init();
        }

        public abstract void Use();

        public abstract void AIUse();

        public abstract void PlotAbility();

        public abstract void DisablePlot();

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        public virtual void PlayerUpkeep() { }

        public virtual void PlayerEndStep() { }

        public virtual void EnemyUpkeep() { }

        public virtual void EnemyEndStep() { }

        public void AttempCoolDownTick(TeamType teamUpKeep)
        {
            TeamType teamType = gameObject.GetComponent<ITeam>().GetTeamType();
            if(teamType == teamUpKeep)
            {
                abilityInformation.currentCoolDown = Mathf.Clamp(abilityInformation.currentCoolDown - 1, 0, config.GetMaxCoolDown());
                if(abilityInformation.currentCoolDown == 0)
                {
                    abilityInformation.canUseAbility = true;
                }
            }
        }

        public void SetCurrentCoolDown(int maxCoolDown)
        {
            abilityInformation.currentCoolDown = maxCoolDown;
            abilityInformation.canUseAbility = false;
        }

        public int GetCurrentCoolDown()
        {
            return abilityInformation.currentCoolDown;
        }

        public bool GetCanUseAbility()
        {
            return abilityInformation.canUseAbility;
        }
    }
}