using System.Collections;
using System.Collections.Generic;
using Breach.Levels;
using UnityEngine;
using Breach.SquadInformation;
using Breach.Placeable.Characters;
using Breach.Manager;
using Breach.UI;
using System;

namespace Breach.GameModes
{
    public class EliminationGameMode : GameLevelGameMode
    {
        EliminationLevelConfig eliminationLevelConfig;

        private int enemiesRemaining;

        #region Init
        public override void Init(LevelConfig levelConfig, SquadConfig newSquadConfig)
        {
            base.Init(levelConfig, newSquadConfig);
            eliminationLevelConfig = levelConfig as EliminationLevelConfig;
            squadConfig = newSquadConfig;
            InitChain();
        }

        public override void InitChain()
        {
            base.InitChain();

            BindToEnemyEvents();

            SetValues();
        }

        private void BindToEnemyEvents()
        {
            foreach(Character enemy in enemyCharacters)
            {
                enemy.OnCharacterDeath += EnemyDied;
            }
        }

        private void SetValues()
        {
            foreach (Character enemy in enemyCharacters)
            {
                if(enemy.GetCharacterState() != Character.CharacterState.dead)
                {
                    enemiesRemaining++;
                }
            }
        }
        #endregion

        private void EnemyDied(Character characterWhoDied)
        {
            characterWhoDied.OnCharacterDeath -= EnemyDied;
            enemiesRemaining--;
            CheckScore();
        }

        private void CheckScore()
        {
            if(enemiesRemaining <= 0)
            {
                MissionPassed();
            }
        }
    }
}