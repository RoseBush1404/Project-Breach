using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Manager;
using Breach.Levels;
using Breach.SquadInformation;

namespace Breach.GameModes
{
    public class SquadSelectionGameMode : GameMode
    {
        public void SquadSelected(SquadConfig squadConfig)
        {
            PlaySessionManager.Instance.SetMySquad(squadConfig);
            PlaySessionManager.Instance.SetMyLevelCatalog(squadConfig.LevelCatalog);
            PlaySessionManager.Instance.SaveSquad();
            PlaySessionManager.Instance.SaveLevelCatalog();
            PlaySessionManager.Instance.MoveToMissionSelection();
        }
    }
}