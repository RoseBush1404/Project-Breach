using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    [CreateAssetMenu(menuName = ("my Game/Abilities/MindControl ability"))]
    public class MindControlConfig : AbilityConfig
    {
        [Header("MindControl Ability Specific")]
        [SerializeField] int maxNumberOfTurnsControlled;
        [SerializeField] float rangeOfInitControl;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<MindControlBehaviour>();
        }

        public override void SetUser(GameObject user)
        {
            behaviour = user.GetComponent<MindControlBehaviour>();
        }

        public int GetMaxNumberOfTurnsControlled()
        {
            return maxNumberOfTurnsControlled;
        }

        public float GetRangeOfInitControl()
        {
            return rangeOfInitControl;
        }
    }
}