using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    [CreateAssetMenu(menuName = ("my Game/Abilities/Test Ability"))]
    public class TestAbilityConfig : AbilityConfig
    {

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<TestAbilityBehaviour>();
        }

        public override void SetUser(GameObject user)
        {
            behaviour = user.GetComponent<TestAbilityBehaviour>();
        }
    }
}