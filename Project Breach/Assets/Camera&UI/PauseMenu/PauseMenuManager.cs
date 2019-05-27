using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.GameModes;
using Breach.Manager;
using Breach.GameModes;

namespace Breach.UI
{
    public class PauseMenuManager : MonoBehaviour
    {
        public void ResumePressed()
        {
            IPauseable pauseable = FindObjectOfType<GameMode>().GetComponent<IPauseable>();
            if(pauseable != null)
            {
                pauseable.ResumeGame();
            }
        }

        public void ConcedePressed()
        {
            PlaySessionManager.Instance.DelectSavedGameDate();
            PlaySessionManager.Instance.MoveToMainMenu();
        }

        public void SaveAndQuit()
        {
            if(PlaySessionManager.Instance.playSessionInformation.gameState == GameState.GameplayLevel)
            {
                IGameLevelGameMode gameMode = FindObjectOfType<GameMode>().GetComponent<IGameLevelGameMode>();
                gameMode.SaveLevelInformation();
            }
            PlaySessionManager.Instance.SaveSquad();
            PlaySessionManager.Instance.SaveLevelCatalog();
            PlaySessionManager.Instance.SavePlaySession();
            PlaySessionManager.Instance.MoveToMainMenu();
        }
    }
}