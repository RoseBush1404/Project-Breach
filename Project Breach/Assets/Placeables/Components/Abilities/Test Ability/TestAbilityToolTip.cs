using System.Collections;
using System.Collections.Generic;
using Breach.Placeable.Component;
using UnityEngine;

namespace Breach.UI.ToolTip
{
    public class TestAbilityToolTip : AbilityToolTip
    {
        public override void Init(AbilityConfig abilityConfig)
        {
            base.Init(abilityConfig);
            StartCoroutine(StartMouseTracking());
        }
    }
}
