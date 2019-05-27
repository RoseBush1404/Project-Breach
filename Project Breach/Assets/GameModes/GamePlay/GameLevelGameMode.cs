using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.Placeable;
using Breach.Levels;
using Breach.Placeable.Component;
using Breach.Manager.Pathfinding;
using Breach.Controler;
using Breach.UI;
using Breach.SquadInformation;
using Breach.Placeable.Characters;
using Breach.Animation;
using Breach.Manager;
using Breach.Placeable.Pathfinding;
using System;

namespace Breach.GameModes
{
    public abstract class GameLevelGameMode : GameMode, IGameLevelGameMode, IPauseable
    {
        //TODO fractor all of this!!!!!!
        [SerializeField] GameObject missionStateCanvas;

        #region Pause Menu Info
        [SerializeField] private GameObject pauseMenuUI;
        private GameObject pauseMenuGO;
        #endregion

        protected SquadConfig squadConfig;
        protected List<Character> playerCharacters = new List<Character>();
        protected List<Character> enemyCharacters = new List<Character>();
        protected List<Cover> coverInScene = new List<Cover>();

        private LevelConfig levelConfig;
        private int playerCharactersRemaining;


        public virtual void Init(LevelConfig levelConfig, SquadConfig newSquadConfig)
        {
            this.levelConfig = levelConfig;
            squadConfig = newSquadConfig;
        }

        public virtual void InitChain()
        {
            InitWaypointInitializer(); //spawns and inits all the waypoints

            InitPlayerCharacters();

            InitEnemyCharacters();

            InitAbilityInformation(); //needs both play and enemy charcters

            InitCover(); // needs character movement

            WaypointManager.Instance.Init(); // needs waypoints

            InitUI();

            PlayerController.Instance.Init();

            BindToPlayerEvents();

            EnemyController.Instance.Init();

            SetUpPauseMenu();
        }

        private void InitPlayerCharacters()
        {
            foreach(SquadMemeberDate squadMemeber in squadConfig.squadMemebers)
            {
                int index = 0;
                for(int i = 0; i < squadConfig.squadMemebers.Length; i++)
                {
                    if(squadConfig.squadMemebers[i] == squadMemeber)
                    {
                        index = i;
                    }
                }
                GameObject CharacterGO = Instantiate(squadMemeber.BlankCharacter, new Vector3(0, 0, 0), Quaternion.identity);

                #region Setting location
                if (squadMemeber.position.x == 0 && squadMemeber.position.y == 0)
                {
                    Vector2Int startingLocation = levelConfig.GetStartingLocationAtIndex(index);
                    GameObject waypoint = GameObject.Find(startingLocation.ToString());
                    if(waypoint != null)
                    {
                        CharacterGO.transform.position = waypoint.transform.position;
                    }
                    else
                    {
                        Debug.LogError(startingLocation + " this is an invalid starting location");
                    }
                    
                }
                else
                {
                    GameObject waypoint = GameObject.Find(squadMemeber.position.ToString());
                    if (waypoint != null)
                    {
                        CharacterGO.transform.position = waypoint.transform.position;
                    }
                }
                #endregion

                #region Set Team Type
                CharacterGO.GetComponent<BreachObject>().SetTeamType(squadMemeber.team);
                #endregion

                #region Set Profile
                CharacterGO.GetComponent<BreachObject>().SetProfileImage(squadMemeber.profileImage);
                #endregion

                #region Set Movement Information
                CharacterMovement characterMovement = CharacterGO.GetComponent<CharacterMovement>();
                if(characterMovement != null)
                {
                    characterMovement.SetSpeed(squadMemeber.movementSpeed);
                    characterMovement.SetRange(squadMemeber.movementRange);
                    characterMovement.Init();
                }
                #endregion

                Character character = CharacterGO.GetComponent<Character>();
                playerCharacters.Add(character);
                character.Init();

                MovementAnimationSelecter animationSelecter = CharacterGO.GetComponent<MovementAnimationSelecter>();
                animationSelecter.Init();

                #region Set Action Points
                if (!squadMemeber.hasMoveAction)
                {
                    character.UseMoveAction();
                }

                if (!squadMemeber.hasAbilityAction)
                {
                    character.UseAbilityAction();
                }
                #endregion

                #region Set Health Information
                HealthSystem characterHealth = CharacterGO.GetComponent<HealthSystem>();
                if (characterHealth != null)
                {
                    characterHealth.SetMaxHealth(squadMemeber.maxHealth);
                    characterHealth.SetCurrentHealth(squadMemeber.currentHealth);
                    characterHealth.Init();
                }
                #endregion

                #region Set Abilities
                Abilities abilities = CharacterGO.GetComponent<Abilities>();
                if (abilities != null)
                {
                    abilities.SetAbilities(squadMemeber.abilityConfigs);
                    abilities.Init();

                    AbilityBehaviour[] abilityBehaviours = abilities.GetAllAbilityBehaviours();
                    for (int i = 0; i < squadMemeber.abilityConfigs.Length; i++)
                    {
                        if (squadMemeber.abilityInformations != null && squadMemeber.abilityInformations[i] != null)
                        {
                            abilityBehaviours[i].abilityInformation = squadMemeber.abilityInformations[i];
                        }
                    }
                }
                #endregion
            }
        }

        private void InitEnemyCharacters()
        {
            foreach(SquadMemeberDate squadMemeber in levelConfig.GetEnemies())
            {
                int index = 0;
                for (int i = 0; i < squadConfig.squadMemebers.Length; i++)
                {
                    if (squadConfig.squadMemebers[i] == squadMemeber)
                    {
                        index = i;
                    }
                }
                GameObject CharacterGO;

                if (squadMemeber.PrebuiltCharacter != null)
                {
                    CharacterGO = Instantiate(squadMemeber.PrebuiltCharacter, new Vector3(0, 0, 0), Quaternion.identity);

                    #region Setting location
                    GameObject waypoint = GameObject.Find(squadMemeber.position.ToString());
                    if (waypoint != null)
                    {
                        CharacterGO.transform.position = waypoint.transform.position;
                    }
                    else
                    {
                        Debug.LogError("Invalid waypoint location " + squadMemeber.position.ToString());
                    }
                    #endregion

                    CharacterMovement characterMovement = CharacterGO.GetComponent<CharacterMovement>();
                    if (characterMovement != null) characterMovement.Init();

                    Character character = CharacterGO.GetComponent<Character>();
                    enemyCharacters.Add(character);
                    character.Init();

                    MovementAnimationSelecter animationSelecter = CharacterGO.GetComponent<MovementAnimationSelecter>();
                    animationSelecter.Init();

                    HealthSystem healthSystem = CharacterGO.GetComponent<HealthSystem>();
                    if (healthSystem != null)
                    {
                        healthSystem.SetCurrentHealth(-1);
                        healthSystem.Init();
                    }

                    Abilities abilities = CharacterGO.GetComponent<Abilities>();
                    if (abilities != null) abilities.Init();
                }
                else
                {
                    CharacterGO = Instantiate(squadMemeber.BlankCharacter, new Vector3(0, 0, 0), Quaternion.identity);

                    #region Setting location
                    GameObject waypoint = GameObject.Find(squadMemeber.position.ToString());
                    if (waypoint != null)
                    {
                        CharacterGO.transform.position = waypoint.transform.position;
                    }
                    #endregion

                    #region Set Team Type
                    CharacterGO.GetComponent<BreachObject>().SetTeamType(squadMemeber.team);
                    #endregion

                    #region Set Profile
                    CharacterGO.GetComponent<BreachObject>().SetProfileImage(squadMemeber.profileImage);
                    #endregion

                    #region Set Movement Information
                    CharacterMovement characterMovement = CharacterGO.GetComponent<CharacterMovement>();
                    if (characterMovement != null)
                    {
                        characterMovement.SetSpeed(squadMemeber.movementSpeed);
                        characterMovement.SetRange(squadMemeber.movementRange);
                        characterMovement.Init();
                    }
                    #endregion

                    Character character = CharacterGO.GetComponent<Character>();
                    enemyCharacters.Add(character);
                    character.Init();

                    MovementAnimationSelecter animationSelecter = CharacterGO.GetComponent<MovementAnimationSelecter>();
                    animationSelecter.Init();

                    #region Set Action Points
                    if (!squadMemeber.hasMoveAction)
                    {
                        character.UseMoveAction();
                    }

                    if (!squadMemeber.hasAbilityAction)
                    {
                        character.UseAbilityAction();
                    }
                    #endregion

                    #region Set Health Information
                    HealthSystem characterHealth = CharacterGO.GetComponent<HealthSystem>();
                    if (characterHealth != null)
                    {
                        characterHealth.SetMaxHealth(squadMemeber.maxHealth);
                        characterHealth.SetCurrentHealth(squadMemeber.currentHealth);
                        characterHealth.Init();
                    }
                    #endregion

                    #region Set Abilities
                    Abilities abilities = CharacterGO.GetComponent<Abilities>();
                    if (abilities != null)
                    {
                        abilities.SetAbilities(squadMemeber.abilityConfigs);
                        abilities.Init();

                        AbilityBehaviour[] abilityBehaviours = abilities.GetAllAbilityBehaviours();
                        for(int i = 0; i < squadMemeber.abilityConfigs.Length; i++)
                        {
                            if (squadMemeber.abilityInformations != null && squadMemeber.abilityInformations[i] != null)
                            {
                                abilityBehaviours[i].abilityInformation = squadMemeber.abilityInformations[i];
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        private void InitAbilityInformation()
        {
            int index = 0;
            foreach (SquadMemeberDate squadMemeber in levelConfig.GetEnemies())
            {
                Character enemy = enemyCharacters[index];
                Abilities abilities = enemy.GetComponent<Abilities>();
                if (abilities != null)
                {
                    AbilityBehaviour[] abilityBehaviours = abilities.GetAllAbilityBehaviours();
                    for (int i = 0; i < squadMemeber.abilityConfigs.Length; i++)
                    {
                        if (squadMemeber.abilityInformations != null && squadMemeber.abilityInformations[i] != null)
                        {
                            int characterIndex = squadMemeber.abilityInformations[i].affectedCharacterIndex;
                            switch (abilityBehaviours[i].abilityInformation.affectedTeam)
                            {
                                case TeamType.Player:
                                    abilityBehaviours[i].abilityInformation.affectedCharacter = playerCharacters[characterIndex];
                                    break;
                                case TeamType.Enemy:
                                    abilityBehaviours[i].abilityInformation.affectedCharacter = enemyCharacters[characterIndex];
                                    break;
                            }
                        }
                    }
                }
                index++;
            }

            index = 0;
            foreach (SquadMemeberDate squadMemeber in squadConfig.squadMemebers)
            {
                Character character = playerCharacters[index];
                Abilities abilities = character.GetComponent<Abilities>();
                if (abilities != null)
                {
                    AbilityBehaviour[] abilityBehaviours = abilities.GetAllAbilityBehaviours();
                    for (int i = 0; i < squadMemeber.abilityConfigs.Length; i++)
                    {
                        if (squadMemeber.abilityInformations != null && squadMemeber.abilityInformations[i] != null)
                        {
                            int characterIndex = squadMemeber.abilityInformations[i].affectedCharacterIndex;
                            switch (abilityBehaviours[i].abilityInformation.affectedTeam)
                            {
                                case TeamType.Player:
                                    abilityBehaviours[i].abilityInformation.affectedCharacter = playerCharacters[characterIndex];
                                    break;
                                case TeamType.Enemy:
                                    abilityBehaviours[i].abilityInformation.affectedCharacter = enemyCharacters[characterIndex];
                                    break;
                            }
                        }
                    }
                }
                index++;
            }

        }

        private void InitUI()
        {
            ProfileUI[] profileUIs = FindObjectsOfType<ProfileUI>();
            foreach(ProfileUI profileUI in profileUIs)
            {
                profileUI.Init();
            }
        }

        private void InitCover()
        {
            foreach(CoverConfigDate coverDate in levelConfig.GetCover())
            {
                GameObject waypoint = GameObject.Find(coverDate.location.ToString());
                if (waypoint != null)
                {
                    GameObject coverGO = Instantiate(coverDate.coverPrefab, waypoint.transform.position, Quaternion.identity, GameObject.Find("Cover").transform);
                    Cover cover = coverGO.GetComponent<Cover>();
                    coverInScene.Add(cover);

                    if (coverDate.maxHitPoints > 0)
                    {
                        //put states in acording to the cover date
                        cover.SetMaxHitPoints(coverDate.maxHitPoints);
                        if (coverDate.currentHitPoints > -1)
                        {
                            cover.SetCurrentHitPoints(coverDate.currentHitPoints);
                        }
                        else
                        {
                            cover.SetCurrentHitPoints(coverDate.maxHitPoints);
                        }
                    }
                    else
                    {
                        //put states in acording to the cover prefab
                        if (cover.GetCurrentHitPoints() <= 0)
                        {
                            cover.SetCurrentHitPoints(cover.GetMaxHitPoints());
                        }
                    }
                    cover.Init();
                }
            }
        }

        private static void InitWaypointInitializer()
        {
            WaypointInitializer[] waypointInitializer = FindObjectsOfType<WaypointInitializer>();
            foreach (WaypointInitializer Initializer in waypointInitializer)
            {
                Initializer.Init();
            }
        }

        private void BindToPlayerEvents()
        {
            foreach(Character character in playerCharacters)
            {
                character.OnCharacterDeath += PlayerCharacterDied;
            }
            playerCharactersRemaining = playerCharacters.Count;
        }

        private void PlayerCharacterDied(Character characterWhoDied)
        {
            characterWhoDied.OnCharacterDeath -= PlayerCharacterDied;
            playerCharactersRemaining--;
            if(playerCharactersRemaining <= 0)
            {
                MissionFailed();
            }
        }

        protected void MissionFailed()
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DeselectObject();
                playerController.SetControl(false);
            }

            GameObject missionStateUI = Instantiate(missionStateCanvas, this.gameObject.transform);
            Text text = missionStateUI.GetComponentInChildren<Text>();
            text.text = ("Mission Failed");

            PlaySessionManager.Instance.playSessionInformation.missionState = MissionState.MissionFailed;

            Button button = missionStateUI.GetComponentInChildren<Button>();
            button.onClick.AddListener(PlaySessionManager.Instance.MoveToMissionSelection);
        }

        protected void MissionPassed()
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DeselectObject();
                playerController.SetControl(false);
            }

            GameObject missionStateUI = Instantiate(missionStateCanvas, this.gameObject.transform);
            Text text = missionStateUI.GetComponentInChildren<Text>();
            text.text = ("Mission Passed");

            PlaySessionManager.Instance.playSessionInformation.missionState = MissionState.MissionCompleted;

            Button button = missionStateUI.GetComponentInChildren<Button>();
            button.onClick.AddListener(PlaySessionManager.Instance.MoveToMissionSelection);
        }

        #region Saving to file
        public virtual void SaveLevelInformation()
        {
            SaveEnemyInforamtions();
            SavePlayerInformations();
            SaveCoverInformation();
        }

        private void SavePlayerInformations()
        {
            SquadMemeberDate[] character = new SquadMemeberDate[playerCharacters.Count];
            SquadMemeberDate[] characterFromConfig = squadConfig.squadMemebers;
            for(int i = 0; i < playerCharacters.Count; i++)
            {
                character[i] = new SquadMemeberDate();
                if(characterFromConfig[i].PrebuiltCharacter != null)
                {
                    character[i].BlankCharacter = characterFromConfig[i].PrebuiltCharacter;
                }
                else
                {
                    character[i].BlankCharacter = characterFromConfig[i].BlankCharacter;
                }
                character[i].PrebuiltCharacter = null;

                Waypoint waypoint = playerCharacters[i].GetCurrentWaypoint();
                character[i].position = new Vector2Int(Mathf.FloorToInt(waypoint.transform.position.x), Mathf.FloorToInt(waypoint.transform.position.y));

                character[i].hasMoveAction = playerCharacters[i].GetMoveAction();
                character[i].hasAbilityAction = playerCharacters[i].GetAbilityAction();
                character[i].team = playerCharacters[i].GetTeamType();
                character[i].profileImage = characterFromConfig[i].profileImage;
                if (playerCharacters[i].GetComponent<CharacterMovement>() != null)
                {
                    character[i].movementSpeed = playerCharacters[i].GetComponent<CharacterMovement>().GetSpeed();
                    character[i].movementRange = playerCharacters[i].GetComponent<CharacterMovement>().GetRange();
                }
                if (playerCharacters[i].GetComponent<HealthSystem>() != null)
                {
                    character[i].maxHealth = playerCharacters[i].GetComponent<HealthSystem>().GetMaxHealth();
                    character[i].currentHealth = playerCharacters[i].GetComponent<HealthSystem>().GetCurrentHealth();
                }
                if (playerCharacters[i].GetComponent<Abilities>() != null)
                {
                    character[i].abilityConfigs = playerCharacters[i].GetComponent<Abilities>().GetAllAbilities();
                }
                if (playerCharacters[i].GetComponent<Abilities>() != null)
                {
                    AbilityBehaviour[] abilityBehaviours = playerCharacters[i].GetComponent<Abilities>().GetAllAbilityBehaviours();
                    AbilityInformation[] abilityInformations = new AbilityInformation[abilityBehaviours.Length];
                    for(int a = 0; a < abilityBehaviours.Length; a++)
                    {
                        //copy over all information
                        abilityInformations[a] = abilityBehaviours[a].abilityInformation;
                        if (abilityInformations[a].affectedCharacter != null)
                        {
                            int index = 0;
                            switch (abilityInformations[a].affectedTeam)
                            {
                                case TeamType.Player:
                                    index = playerCharacters.IndexOf(abilityInformations[a].affectedCharacter.GetComponent<Character>());
                                    break;
                                case TeamType.Enemy:
                                    index = enemyCharacters.IndexOf(abilityInformations[a].affectedCharacter.GetComponent<Character>());
                                    break;
                            }
                            //override affected character information
                            abilityInformations[a].affectedCharacterIndex = index;
                            abilityInformations[a].affectedCharacter = null;
                        }
                    }
                    character[i].abilityInformations = abilityInformations;
                }
            }
            squadConfig.squadMemebers = character;
        }

        private void SaveEnemyInforamtions()
        {
            SquadMemeberDate[] enemies = new SquadMemeberDate[enemyCharacters.Count];
            SquadMemeberDate[] enemiesFromConfig = levelConfig.GetEnemies();
            for(int i = 0; i < enemyCharacters.Count; i++)
            {
                enemies[i] = new SquadMemeberDate();
                if (enemiesFromConfig[i].PrebuiltCharacter != null)
                {
                    enemies[i].BlankCharacter = enemiesFromConfig[i].PrebuiltCharacter;
                }
                else
                {
                    enemies[i].BlankCharacter = enemiesFromConfig[i].BlankCharacter;
                }
                enemies[i].PrebuiltCharacter = null;

                Waypoint waypoint = enemyCharacters[i].GetCurrentWaypoint();
                enemies[i].position = new Vector2Int(Mathf.FloorToInt(waypoint.transform.position.x), Mathf.FloorToInt(waypoint.transform.position.y));

                enemies[i].hasMoveAction = enemyCharacters[i].GetMoveAction();
                enemies[i].hasAbilityAction = enemyCharacters[i].GetAbilityAction();
                enemies[i].team = enemyCharacters[i].GetTeamType();
                enemies[i].profileImage = enemyCharacters[i].GetProfileImage();
                if (enemyCharacters[i].GetComponent<CharacterMovement>() != null)
                {
                    enemies[i].movementSpeed = enemyCharacters[i].GetComponent<CharacterMovement>().GetSpeed();
                    enemies[i].movementRange = enemyCharacters[i].GetComponent<CharacterMovement>().GetRange();
                }
                if (enemyCharacters[i].GetComponent<HealthSystem>() != null)
                {
                    enemies[i].maxHealth = enemyCharacters[i].GetComponent<HealthSystem>().GetMaxHealth();
                    enemies[i].currentHealth = enemyCharacters[i].GetComponent<HealthSystem>().GetCurrentHealth();
                }
                if (enemyCharacters[i].GetComponent<Abilities>() != null)
                {
                    enemies[i].abilityConfigs = enemyCharacters[i].GetComponent<Abilities>().GetAllAbilities();
                }
                if (enemyCharacters[i].GetComponent<Abilities>() != null)
                {
                    AbilityBehaviour[] abilityBehaviours = enemyCharacters[i].GetComponent<Abilities>().GetAllAbilityBehaviours();
                    AbilityInformation[] abilityInformations = new AbilityInformation[abilityBehaviours.Length];
                    for (int a = 0; a < abilityBehaviours.Length; a++)
                    {
                        abilityInformations[a] = abilityBehaviours[a].abilityInformation;
                        if (abilityInformations[a].affectedCharacter != null)
                        {
                            int index = 0;
                            switch (abilityInformations[a].affectedTeam)
                            {
                                case TeamType.Player:
                                    index = playerCharacters.IndexOf(abilityInformations[a].affectedCharacter.GetComponent<Character>());
                                    break;
                                case TeamType.Enemy:
                                    index = enemyCharacters.IndexOf(abilityInformations[a].affectedCharacter.GetComponent<Character>());
                                    break;
                            }
                            
                            //override affected character information
                            abilityInformations[a].affectedCharacterIndex = index;
                            abilityInformations[a].affectedCharacter = null;
                        }
                    }
                    enemies[i].abilityInformations = abilityInformations;
                }
            }
            levelConfig.UpdateEnemyInfo(enemies);
        }

        private void SaveCoverInformation()
        {
            CoverConfigDate[] coverDate = new CoverConfigDate[coverInScene.Count];
            CoverConfigDate[] coverFromConfig = levelConfig.GetCover();
            for(int i = 0; i < coverInScene.Count; i++)
            {
                coverDate[i] = new CoverConfigDate();
                coverDate[i].coverPrefab = coverFromConfig[i].coverPrefab;
                coverDate[i].location = coverFromConfig[i].location;
                coverDate[i].maxHitPoints = coverInScene[i].GetMaxHitPoints();
                coverDate[i].currentHitPoints = coverInScene[i].GetCurrentHitPoints();
            }
            levelConfig.UpdateCoverInfo(coverDate);
        }
        #endregion

        #region Pause Menu
        public void SetUpPauseMenu()
        {
            pauseMenuGO = Instantiate(pauseMenuUI, gameObject.transform);
            pauseMenuGO.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
            if (pauseMenuGO != null)
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