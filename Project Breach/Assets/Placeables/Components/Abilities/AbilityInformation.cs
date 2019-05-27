using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    [Serializable]
    public class AbilityInformation
    {
        public int currentCoolDown;
        public bool canUseAbility;
        public int numberOfTurnsAffected;
        public BreachObject affectedCharacter;
        public int affectedCharacterIndex;
        public TeamType affectedTeam;

        public void Init()
        {
            currentCoolDown = 0;
            canUseAbility = true;
            numberOfTurnsAffected = 0;
            affectedCharacter = null;
            affectedCharacterIndex = 0;
            affectedTeam = TeamType.Neutral;
        }
    }
}