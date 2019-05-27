using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Breach.Manager;

namespace Breach.UI
{
    public class GamePlayMenuManager : MonoBehaviour
    {
        //TODO think about this class, dont really like it, see if it can be moved to the game play game mode class instead
        //canvases
        [SerializeField] private GameObject GamePlayUI;
        [SerializeField] private GameObject WinUI;
        [SerializeField] private GameObject LoseUI;
        [SerializeField] private GameObject MissionCompleteUI;

        //active canvas
        private GameObject ActiveMenu;

        public enum gameplayUIs
        {
            GamePlayUI,
            WinUI,
            LoseUI,
            MissionCompleteUI
        }

        void Start()
        {
            ActiveMenu = GamePlayUI;

            DisableScreens();
            ActivateScreen(ActiveMenu);
        }

        void DisableScreens()
        {
            GamePlayUI.SetActive(false);
            WinUI.SetActive(false);
            LoseUI.SetActive(false);
            MissionCompleteUI.SetActive(false);
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

        public void OpenMenu(gameplayUIs UI)
        {
            switch (UI)
            {
                case gameplayUIs.GamePlayUI:
                    StartCoroutine(OpenNewMenu(GamePlayUI));
                    break;

                case gameplayUIs.WinUI:
                    StartCoroutine(OpenNewMenu(WinUI));
                    break;

                case gameplayUIs.LoseUI:
                    StartCoroutine(OpenNewMenu(LoseUI));
                    break;

                case gameplayUIs.MissionCompleteUI:
                    StartCoroutine(OpenNewMenu(MissionCompleteUI));
                    break;
            }
        }

        public void WinPressed()
        {
            PlaySessionManager.Instance.MoveToEndScreen();
        }

        public void LosePressed()
        {
            PlaySessionManager.Instance.MoveToEndScreen();
        }

        public void MissionCompletedPressed()
        {
            PlaySessionManager.Instance.MoveToMissionSelection();
        }
    }
}