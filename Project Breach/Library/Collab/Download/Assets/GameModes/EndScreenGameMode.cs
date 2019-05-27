using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.Manager;
using Breach.Levels;

namespace Breach.GameModes
{
    public class EndScreenGameMode : GameMode
    {
        [SerializeField] Text text;
        [SerializeField] string WinText = "You Won!";
        [SerializeField] string LoseText = "Mission Failed";

        private void Start()
        {
            base.Start();

            switch (PlaySessionManager.Instance.playSessionInformation.missionState)
            {
                case MissionState.MissionCompleted:
                    text.text = WinText;
                    break;

                case MissionState.MissionFailed:
                    text.text = LoseText;
                    break;
            }
            PlaySessionManager.Instance.playSessionInformation.missionState = MissionState.MissionStart;
            PlaySessionManager.Instance.DelectSavedGameDate();
        }

        public void ButtonPressed()
        {
            PlaySessionManager.Instance.MoveToMainMenu();
        }
    }
}