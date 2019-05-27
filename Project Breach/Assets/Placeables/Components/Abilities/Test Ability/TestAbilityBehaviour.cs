using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Characters;

namespace Breach.Placeable.Component
{
    public class TestAbilityBehaviour : AbilityBehaviour
    {
        public override void Use()
        {
            print("used ability");
            Character character = GetComponent<Character>();
            character.UseAbilityAction();
            SetCurrentCoolDown(config.GetMaxCoolDown());
            character.TaskFinished();
        }

        public override void AIUse()
        {
            print("used ability");
            Character character = GetComponent<Character>();
            character.UseAbilityAction();
            SetCurrentCoolDown(config.GetMaxCoolDown());
            character.TaskFinished();
        }

        public override void PlotAbility()
        {
            print("Plotting");
        }

        public override void DisablePlot()
        {
            print("Disabling Plot");
        }
    }
}