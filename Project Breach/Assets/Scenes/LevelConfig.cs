using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.SquadInformation;
using Breach.Placeable.Component;

namespace Breach.Levels
{
    [Serializable]
    public enum Level
    {
        Penthouse = 4,
        RoofTop,
        Apartments,
        HighStreet
    };

    [Serializable]
    public enum GameModeType
    {
        Elimination
    };

    [Serializable]
    public abstract class LevelConfig : ScriptableObject
    {
        [Header("General Level information")]
        public Level level;
        public GameObject gameMode;
        public GameModeType gameModeType;
        public Vector2Int[] playerStartingLocations;
        public SquadMemeberDate[] enemies;
        public CoverConfigDate[] cover;

        public virtual void UpdateInforamtion(LevelConfig levelConfig)
        {
            level = levelConfig.level;
            gameMode = levelConfig.gameMode;
            gameModeType = levelConfig.gameModeType;
            playerStartingLocations = levelConfig.playerStartingLocations;
            enemies = levelConfig.enemies;
            cover = levelConfig.cover;
        }

        public void UpdateEnemyInfo(SquadMemeberDate[] enemies)
        {
            this.enemies = enemies;
        }

        public void UpdateCoverInfo(CoverConfigDate[] cover)
        {
            this.cover = cover;
        }

        public Level GetLevel()
        {
            return level;
        }

        public GameObject GetGameMode()
        {
            return gameMode;
        }

        public Vector2Int[] GetPlayerStartingLocations()
        {
            return playerStartingLocations;
        }

        public Vector2Int GetStartingLocationAtIndex(int index)
        {
            return playerStartingLocations[index];
        }

        public SquadMemeberDate[] GetEnemies()
        {
            return enemies;
        }

        public CoverConfigDate[] GetCover()
        {
            return cover;
        }

        public GameModeType GetGameModeType()
        {
            return gameModeType;
        }
    }
}