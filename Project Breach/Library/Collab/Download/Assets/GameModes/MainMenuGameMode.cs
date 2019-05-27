using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Manager;
using Breach.Levels;

namespace Breach.GameModes
{
    public class MainMenuGameMode : GameMode
    {
        //canvases
        [SerializeField] private GameObject mainMenuUI;
        [SerializeField] private GameObject playOptionsUI;
        [SerializeField] private GameObject creditsUI;
        [SerializeField] private GameObject quitUI;

        //active canvas
        private GameObject ActiveMenu;

        #region Menu Changing
        void Start()
        {
            ActiveMenu = mainMenuUI;

            DisableScreens();
            ActivateScreen(ActiveMenu);
        }

        void DisableScreens()
        {
            mainMenuUI.SetActive(false);
            playOptionsUI.SetActive(false);
            creditsUI.SetActive(false);
            quitUI.SetActive(false);
        }

        IEnumerator OpenNewMenu(GameObject NewMenu)
        {
            float f = 0;
            yield return new WaitForSeconds(f);
            DisableScreens();
            ActivateScreen(NewMenu);
        }

        void ActivateScreen(GameObject ScreenToActivate)
        {
            ScreenToActivate.SetActive(true);
            ActiveMenu = ScreenToActivate;
        }
        #endregion

        #region Button Functions
        public void PlayPressed()
        {
            StartCoroutine(OpenNewMenu(playOptionsUI));
        }

        public void CreditsPressed()
        {
            StartCoroutine(OpenNewMenu(creditsUI));
        }

        public void QuitPressed()
        {
            StartCoroutine(OpenNewMenu(quitUI));
        }

        public void QuitConformed()
        {
            Application.Quit();
        }

        public void ReturnPressed()
        {
            StartCoroutine(OpenNewMenu(mainMenuUI));
        }

        public void NewGamePressed()
        {
            PlaySessionManager.Instance.DelectSavedGameDate();
            PlaySessionManager.Instance.MoveToSquadSelection();
        }

        public void ContinueGamePressed()
        {
            PlaySessionManager.Instance.LoadSquad();
            PlaySessionManager.Instance.LoadLevelCatalog();
            PlaySessionManager.Instance.LoadPlaySession();

            switch (PlaySessionManager.Instance.playSessionInformation.gameState)
            {
                case GameState.MissionSelection:
                    //load mission selection
                    PlaySessionManager.Instance.MoveToMissionSelection();
                    break;

                case GameState.GameplayLevel:
                    //load level
                    PlaySessionManager.Instance.MoveToGameplayLevel(PlaySessionManager.Instance.playSessionInformation.levelConfig);
                    break;
            }
        }
        #endregion
    }
}