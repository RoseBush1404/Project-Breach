using System.Collections;
using System.Collections.Generic;
using Breach.Placeable.Component;
using UnityEngine.UI;
using UnityEngine;

namespace Breach.UI.ToolTip
{
    public class MindControlToolTip : AbilityToolTip
    {
        [Header("Mind Control information")]
        [SerializeField] Text numberOfTurnsControlledField;
        [SerializeField] Text RangeField;

        public override void Init(AbilityConfig abilityConfig)
        {
            base.Init(abilityConfig);

            numberOfTurnsControlledField.text = (abilityConfig as MindControlConfig).GetMaxNumberOfTurnsControlled().ToString();
            RangeField.text = (abilityConfig as MindControlConfig).GetRangeOfInitControl().ToString();

            StartCoroutine(StartMouseTracking());
        }
    }
}
