using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Breach.Manager;
using Breach.GameModes;
using Breach.SquadInformation;
using Breach.Levels;

namespace Breach.SavingAndLoading
{
    public static class SaveLoad
    {
        private static string playSessionPath = Path.Combine(Application.persistentDataPath, "savedPlaySession.json");
        private static string squadPath = Path.Combine(Application.persistentDataPath, "savedSquad.json");
        private static string catalogPath = Path.Combine(Application.persistentDataPath, "savedLevelCatalog.json");
        private static string missionLayoutPath = Path.Combine(Application.persistentDataPath, "savedmissionLayout.json");
        private static string json;

        public static void DeleteSavedGame()
        {
            File.Delete(playSessionPath);
            File.Delete(squadPath);
            File.Delete(catalogPath);
            File.Delete(missionLayoutPath);
        }

        #region Play session information saving and loading
        public static void SavePlaySession()
        {
            json = JsonUtility.ToJson(PlaySessionManager.Instance.playSessionInformation);

            BinaryFormatter bf = new BinaryFormatter();

            //Debug.Log("Path: " + squadPath);

            FileStream file = File.Create(playSessionPath);

            bf.Serialize(file, json);
            file.Close();
        }

        public static void LoadPlaySession()
        {
            if (File.Exists(playSessionPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savedPlaySession.json", FileMode.Open);

                json = (string)bf.Deserialize(file);
                file.Close();

                JsonUtility.FromJsonOverwrite(json, PlaySessionManager.Instance.playSessionInformation);
            }
            else
            {
                Debug.Log("didn't load squad");
            }
        }
        #endregion

        #region Squad saving and loading
        public static void SaveSquad()
        {
            json = JsonUtility.ToJson(PlaySessionManager.Instance.mySquad);

            BinaryFormatter bf = new BinaryFormatter();

            //Debug.Log("Path: " + squadPath);

            FileStream file = File.Create(squadPath);

            bf.Serialize(file, json);
            file.Close();
        }

        public static void LoadSquad()
        {
            if (File.Exists(squadPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savedSquad.json", FileMode.Open);

                json = (string)bf.Deserialize(file);
                file.Close();

                JsonUtility.FromJsonOverwrite(json, PlaySessionManager.Instance.mySquad);
            }
            else
            {
                Debug.Log("didn't load squad");
            }
        }
        #endregion

        #region Level Catalog saving and loading
        public static void SaveLevelCatalog()
        {
            json = JsonUtility.ToJson(PlaySessionManager.Instance.myLevelCatalog);

            BinaryFormatter bf = new BinaryFormatter();

            //Debug.Log("Path: " + catalogPath);

            FileStream file = File.Create(catalogPath);

            bf.Serialize(file, json);
            file.Close();
        }
        

        public static void LoadLevelCatalog()
        {
            if (File.Exists(catalogPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savedLevelCatalog.json", FileMode.Open);

                json = (string)bf.Deserialize(file);
                file.Close();

                JsonUtility.FromJsonOverwrite(json, PlaySessionManager.Instance.myLevelCatalog);
            }
            else
            {
                Debug.Log("didn't load catalog");
            }
        }
        #endregion

        #region Mission layout config saving and loading
        public static void SaveMissionLayoutConfig(MissionSelectionGameMode missionSelection)
        {
            json = JsonUtility.ToJson(missionSelection.missionLayoutConfig);

            BinaryFormatter bf = new BinaryFormatter();

            //Debug.Log("Path: " + missionLayoutPath);

            FileStream file = File.Create(missionLayoutPath);

            bf.Serialize(file, json);
            file.Close();
        }

        public static void LoadMissionLayoutConfig(MissionSelectionGameMode missionSelection)
        {
            if (File.Exists(missionLayoutPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savedmissionLayout.json", FileMode.Open);

                json = (string)bf.Deserialize(file);
                file.Close();

                JsonUtility.FromJsonOverwrite(json, missionSelection.missionLayoutConfig);
            }
        }
        #endregion
    }
}