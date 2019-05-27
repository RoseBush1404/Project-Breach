using System;
using UnityEngine;
using Breach.Placeable.Component;

namespace Breach.SquadInformation
{
    [Serializable]
    public class SquadMemeberDate
    {
        public GameObject PrebuiltCharacter;
        public GameObject BlankCharacter;
        public Vector2Int position;
        public bool hasMoveAction = true;
        public bool hasAbilityAction = true;
        public TeamType team;
        public Sprite profileImage;
        public float movementSpeed;
        public int movementRange;
        public float maxHealth;
        public float currentHealth = -1f;
        public AbilityConfig[] abilityConfigs;
        public AbilityInformation[] abilityInformations;
    }
}