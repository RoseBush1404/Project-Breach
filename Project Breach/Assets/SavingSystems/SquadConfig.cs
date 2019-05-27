using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Component;
using Breach.Levels;

namespace Breach.SquadInformation
{
    [Serializable]
    [CreateAssetMenu(menuName = ("my Game/SavingSystem/SquadConfig"))]
    public class SquadConfig : ScriptableObject
    {
        public LevelCatalog LevelCatalog;
        public SquadMemeberDate[] squadMemebers;

        public void UpdateInformation(SquadConfig squadConfig)
        {
            this.squadMemebers = squadConfig.squadMemebers;
            this.LevelCatalog = squadConfig.LevelCatalog;
        }

        public void ResetSquad()
        {
            foreach(SquadMemeberDate memeber in squadMemebers)
            {
                memeber.position = new Vector2Int(0, 0);
                memeber.hasMoveAction = true;
                memeber.hasAbilityAction = true;
                memeber.team = TeamType.Player;
                memeber.currentHealth = -1;
                memeber.abilityInformations = null;
            }
        }
    }
}