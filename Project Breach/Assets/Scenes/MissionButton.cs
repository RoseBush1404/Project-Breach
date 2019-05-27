using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.GameModes;

namespace Breach.Levels
{
    public class MissionButton : MonoBehaviour
    {
        [SerializeField] private int levelLayer;
        [SerializeField] private int layerPosition;
        [SerializeField] private LevelConfig levelConfig;

        public LevelConfig GetButtonsLevel()
        {
            return levelConfig;
        }

        public void SetUpButton(int currentStageLevel, LevelCatalog levelCatalog)
        {
            
            if (levelCatalog.GetLevelsOnLayerLength() > levelLayer && levelCatalog.GetLevelsOnLayerByIndex(levelLayer) >= layerPosition)
            {
                gameObject.SetActive(true);
                if (currentStageLevel > levelLayer || currentStageLevel < levelLayer)
                {
                    Button button = GetComponent<Button>();
                    button.interactable = false;
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void SetButtonsLevel(LevelConfig newLevelConfig)
        {
            levelConfig = newLevelConfig;
            gameObject.GetComponentInChildren<Text>().text = levelConfig.GetLevel().ToString();
        }

        public void MissionSelected()
        {
            MissionSelectionGameMode missionSelectionGameMode = FindObjectOfType<MissionSelectionGameMode>();
            missionSelectionGameMode.MissionSelected(levelConfig);
        }
    }
}
