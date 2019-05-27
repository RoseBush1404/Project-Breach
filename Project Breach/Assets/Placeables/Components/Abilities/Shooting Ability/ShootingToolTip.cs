using System.Collections;
using System.Collections.Generic;
using Breach.Placeable.Component;
using UnityEngine.UI;
using UnityEngine;

namespace Breach.UI.ToolTip
{
    public class ShootingToolTip : AbilityToolTip
    {
        [Header("Shooting information")]
        [SerializeField] Text numberOfProjectilesField;
        [SerializeField] Text firingAngleField;

        public override void Init(AbilityConfig abilityConfig)
        {
            base.Init(abilityConfig);

            numberOfProjectilesField.text = (abilityConfig as ShootingConfig).GetNumOfShotsToFire().ToString();
            firingAngleField.text = (abilityConfig as ShootingConfig).GetFiringAngleRange().ToString();

            StartCoroutine(StartMouseTracking());
        }
    }
}