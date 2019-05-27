using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.Manager;
using Breach.Levels;
using Breach.SquadInformation;
using Breach.UI;
using Breach.SavingAndLoading;
using System;

namespace Breach.GameModes
{
    //TODO tidy this class up!!!
    public class MissionSelectionGameMode : GameMode, IPauseable
    {
        [SerializeField] private GameObject gameStateUI;

        #region Pause Menu Info
        [SerializeField] private GameObject pauseMenuUI;
        private GameObject pauseMenuGO;
        #endregion

        private LevelCatalog levelCatalog;
        private int currentLevelPorgress;

        public MissionLayoutConfig missionLayoutConfig = new MissionLayoutConfig();

        public void MissionSelected(LevelConfig levelConfig)
        {
            PlaySessionManager.Instance.MoveToGameplayLevel(levelConfig);
        }

        public override void Start()
        {
            base.Start();
            SetUpPauseMenu();
            levelCatalog = PlaySessionManager.Instance.GetMyLevelCatalog();
            WorkoutMissionState();
            currentLevelPorgress = levelCatalog.gameProgress;
            PlaySessionManager.Instance.SaveLevelCatalog();
            try
            {
                SaveLoad.LoadMissionLayoutConfig(this);
                LoadUpButtons();
            }
            catch
            {
                SetUpButtons();
            }
        }

        private void WorkoutMissionState()
        {
            switch (PlaySessionManager.Instance.playSessionInformation.missionState)
            {
                case MissionState.MissionStart:
                    //loading into the level for the first time/back from a saved game
                    break;

                case MissionState.MissionCompleted:
                    levelCatalog.gameProgress++;
                    if (levelCatalog.gameProgress >= levelCatalog.GetNumberOfLevelsToComplete())
                    {
                        SpawnGameStateUI();
                    }
                    else
                    {
                        PlaySessionManager.Instance.playSessionInformation.missionState = MissionState.MissionStart;
                    }
                    break;

                case MissionState.MissionFailed:
                    //failed the level, GAME OVER
                    SpawnGameStateUI();
                    break;
            }
        }

        private void SpawnGameStateUI()
        {
            GameObject UI = Instantiate(gameStateUI, gameObject.transform);
            Text text = UI.GetComponentInChildren<Text>();
            switch (PlaySessionManager.Instance.playSessionInformation.missionState)
            {
                case MissionState.MissionCompleted:
                    text.text = ("Game Won");
                    break;

                case MissionState.MissionFailed:
                    text.text = ("Game Failed");
                    break;
            }

            Button button = UI.GetComponentInChildren<Button>();
            button.onClick.AddListener(PlaySessionManager.Instance.MoveToEndScreen);
        }

        private void SetUpButtons()
        {
            Dictionary<LevelConfig, bool> missions = new Dictionary<LevelConfig, bool>();
            foreach(LevelConfig levelConfig in levelCatalog.GetAllLevels())
            {
                missions.Add(levelConfig, false);
            }

            MissionButton[] missionButtons = FindObjectsOfType<MissionButton>();
            int numberOfActiveButtons = 0;

            foreach (MissionButton missionButton in missionButtons)
            {
                missionButton.SetUpButton(currentLevelPorgress, levelCatalog);
                if (missionButton.isActiveAndEnabled == true)
                {
                    numberOfActiveButtons++;
                    List<LevelConfig> unusedLevels = new List<LevelConfig>();
                    foreach (KeyValuePair<LevelConfig, bool> dictionaryItem in missions)
                    {
                        if (dictionaryItem.Value == false)
                        {
                            unusedLevels.Add(dictionaryItem.Key);
                        }
                    }
                    LevelConfig levelConfig = unusedLevels[UnityEngine.Random.Range(0, unusedLevels.Count)];
                    missionButton.SetButtonsLevel(levelConfig);
                    missions[levelConfig] = true;
                }
            }
            
            missionLayoutConfig.Levels = new LevelConfig[numberOfActiveButtons];
            int i = 0;
            foreach(MissionButton missionButton in missionButtons)
            {
                if(missionButton.isActiveAndEnabled == true)
                {
                    missionLayoutConfig.Levels[i] = missionButton.GetButtonsLevel();
                    i++;
                }
            }
            SaveLoad.SaveMissionLayoutConfig(this);
        }


        private void LoadUpButtons()
        {
            MissionButton[] missionButtons = FindObjectsOfType<MissionButton>();
            int i = 0;
            foreach (MissionButton missionButton in missionButtons)
            {
                missionButton.SetUpButton(currentLevelPorgress, levelCatalog);
                if (missionButton.isActiveAndEnabled == true)
                {
                    missionButton.SetButtonsLevel(missionLayoutConfig.Levels[i]);
                    i++;
                }
            }
        }

        #region Pause Menu
        public void SetUpPauseMenu()
        {
            pauseMenuGO = Instantiate(pauseMenuUI, gameObject.transform);
            pauseMenuGO.SetActive(false);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenuGO.activeInHierarchy == false)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }
        }

        public void PauseGame()
        {
            if(pauseMenuGO != null)
            {
                pauseMenuGO.SetActive(true);
            }
        }

        public void ResumeGame()
        {
            if (pauseMenuGO != null)
            {
                pauseMenuGO.SetActive(false);
            }
        }
        #endregion
    }
}