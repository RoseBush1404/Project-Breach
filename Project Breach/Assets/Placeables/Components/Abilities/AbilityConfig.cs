using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    [Serializable]
    public abstract class AbilityConfig : ScriptableObject, ITurn
    {

        [Header("Ability General")]
        [SerializeField] string abilityName;
        [SerializeField] AnimationClip abilityAnimation;
        [SerializeField] GameObject overlay;
        [SerializeField] GameObject abilityToolTip;
        [SerializeField] int maxCoolDown = 1;
        [SerializeField] Sprite abilityImage; // TODO change to image

        protected AbilityBehaviour behaviour;

        public abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);

        public abstract void SetUser(GameObject user);

        public void AttachAbilityTo(GameObject objectToAttachTo)
        {
            AbilityBehaviour behaviourComponent = GetBehaviourComponent(objectToAttachTo);
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public void Use()
        {
            behaviour.Use();
        }

        public void AIUse()
        {
            behaviour.AIUse();
        }

        public void PlotAbility()
        {
            behaviour.PlotAbility();
        }

        public void DisablePlot()
        {
            behaviour.DisablePlot();
        }

        public bool CanUseAbility()
        {
            return behaviour.GetCanUseAbility();
        }

        public int GetCurrentCoolDown()
        {
            return behaviour.GetCurrentCoolDown();
        }

        public void PlayerUpkeep()
        {
            behaviour.AttempCoolDownTick(TeamType.Player);
        }

        public void PlayerEndStep()
        {
            
        }

        public void EnemyUpkeep()
        {
            behaviour.AttempCoolDownTick(TeamType.Enemy);
        }

        public void EnemyEndStep()
        {
            
        }

        public string GetAbilityName()
        {
            return abilityName;
        }

        public AnimationClip GetAbilityAnimation()
        {
            return abilityAnimation;
        }

        public GameObject GetOverlay()
        {
            return overlay;
        }

        public GameObject GetAbilityToolTip()
        {
            return abilityToolTip;
        }

        public int GetMaxCoolDown()
        {
            return maxCoolDown;
        }

        public Sprite GetAbilityColor()
        {
            return abilityImage;
        }

        public AbilityBehaviour GetAbilityBehaviour()
        {
            return behaviour;
        }
    }
}