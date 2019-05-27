using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Levels;
using Breach.SquadInformation;

namespace Breach.GameModes
{
    public interface IGameLevelGameMode : IGameMode
    {
        void Init(LevelConfig levelConfig, SquadConfig newSquadConfig);
        void SaveLevelInformation();
    }
}