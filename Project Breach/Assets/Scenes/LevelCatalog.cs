using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Levels
{
    [Serializable]
    [CreateAssetMenu(menuName = ("my Game/LevelConfig/LevelCatelog"))]
    public class LevelCatalog : ScriptableObject
    {
        [SerializeField]
        private LevelConfig[] Levels;

        [SerializeField]
        private int[] levelsPerLayer;

        [SerializeField]
        private int numberOfLevelsToComplete;
        public int gameProgress;

        public void UpdateInformation(LevelCatalog levelCatalog)
        {
            this.Levels = levelCatalog.Levels;
            this.numberOfLevelsToComplete = levelCatalog.numberOfLevelsToComplete;
            this.levelsPerLayer = levelCatalog.levelsPerLayer;
            this.gameProgress = levelCatalog.gameProgress;
        }

        public LevelConfig GetLevelAtIndex(int index)
        {
            return Levels[index];
        }

        public LevelConfig GetRandomLevel()
        {
            int randomNumber = UnityEngine.Random.Range(0, Levels.Length);
            return Levels[randomNumber];
        }

        public LevelConfig[] GetAllLevels()
        {
            return Levels;
        }

        public int GetNumberOfLevelsToComplete()
        {
            return numberOfLevelsToComplete;
        }

        public int GetLevelsOnLayerLength()
        {
            return levelsPerLayer.Length;
        }

        public int GetLevelsOnLayerByIndex(int index)
        {
            return levelsPerLayer[index];
        }
    }
}