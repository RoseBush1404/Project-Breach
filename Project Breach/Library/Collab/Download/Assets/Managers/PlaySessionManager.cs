using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Breach.Levels;
using Breach.GameModes;
using Breach.SquadInformation;
using Breach.Controler;
using Breach.UI;
using Breach.SavingAndLoading;
using System;

namespace Breach.Manager
{
    public enum GameState 
    {
        StartUp,
        MainMenu,
        SquadSelection,
        MissionSelection,
        GameplayLevel,
        EndScreen
    };

    public enum GameScene
    {
        MainMenu,
        SquadSelection,
        MissionSelection,
        EndScreen,
        Level
    };

    public enum MissionState
    {
        MissionStart,
        MissionCompleted,
        MissionFailed
    };

    public class PlaySessionManager : SingletionBase<PlaySessionManager>
    {
        //TODO look thorugh all the parts of this class, see what needs to be sotred and what dosn't 
        [SerializeField] public SquadConfig mySquad;
        [SerializeField] public LevelCatalog myLevelCatalog;

        [SerializeField] LevelConfig[] blankLevelConfigs;
        [SerializeField] public PlaySessionInformation playSessionInformation = new PlaySessionInformation();

        public SquadConfig GetMySquad()
        {
            return mySquad;
        }

        public void SetMySquad(SquadConfig squadConfig)
        {
            mySquad.UpdateInformation(squadConfig);
        }

        public LevelCatalog GetMyLevelCatalog()
        {
            return myLevelCatalog;
        }

        public void SetMyLevelCatalog(LevelCatalog levelCatalog)
        {
            myLevelCatalog.UpdateInformation(levelCatalog);
        }

        private void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);
            playSessionInformation.gameState = GameState.StartUp;
            SceneManager.sceneLoaded += LevelLoaded;
        }

        #region Saving And Loading
        //TODO consider making these private and the play session manager is the only thing that managers the saving and loading
        public void DelectSavedGameDate()
        {
            SaveLoad.DeleteSavedGame();
        }

        public void SavePlaySession()
        {
            SaveLoad.SavePlaySession();
        }

        public void LoadPlaySession()
        {
            SaveLoad.LoadPlaySession();
        }

        public void SaveSquad()
        {
            SaveLoad.SaveSquad();
        }

        public void LoadSquad()
        {
            SaveLoad.LoadSquad();
        }

        public void SaveLevelCatalog()
        {
            SaveLoad.SaveLevelCatalog();
        }

        public void LoadLevelCatalog()
        {
            SaveLoad.LoadLevelCatalog();
        }
        #endregion

        #region Scene Loading
        public void UpdateGameState(GameState newGameState)
        {
            playSessionInformation.gameState = newGameState;
        }

        public void MoveToMainMenu()
        {
            SceneManager.LoadScene((int)GameScene.MainMenu);
        }

        public void MoveToSquadSelection()
        {
            SceneManager.LoadScene((int)GameScene.SquadSelection);
        }

        public void MoveToMissionSelection()
        {
            mySquad.ResetSquad();
            SaveLoad.SaveSquad();
            SceneManager.LoadScene((int)GameScene.MissionSelection);
        }

        public void MoveToGameplayLevel(LevelConfig levelConfig)
        {
            //copy over level information into a changingable config
            switch (levelConfig.GetGameModeType())
            {
                case GameModeType.Elimination:
                    blankLevelConfigs[0].UpdateInforamtion(levelConfig);
                    playSessionInformation.levelConfig = blankLevelConfigs[0];
                    break;
            }
            SaveLoad.SaveSquad();
            SceneManager.LoadScene((int)blankLevelConfigs[0].GetLevel());
        }

        public void MoveToEndScreen()
        {
            SceneManager.LoadScene((int)GameScene.EndScreen);
        }
        #endregion

        #region On Load
        private void LevelLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (this != Instance) return;

            if (playSessionInformation.levelConfig != null && (int)playSessionInformation.levelConfig.GetLevel() == scene.buildIndex)
            {
                GameObject gameModeGO = Instantiate(playSessionInformation.levelConfig.GetGameMode());
                gameModeGO.GetComponent<IGameLevelGameMode>().Init(playSessionInformation.levelConfig, mySquad);
            }
        }
        #endregion
    }
}