using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Levels;

namespace Breach.Manager
{
    [Serializable]
    public class PlaySessionInformation
    {
        public GameState gameState;
        public LevelConfig levelConfig;
        public MissionState missionState = MissionState.MissionStart;
    }
}