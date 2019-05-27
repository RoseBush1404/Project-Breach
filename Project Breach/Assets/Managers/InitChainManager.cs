using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Breach.Manager.Pathfinding;
using Breach.Placeable.Pathfinding;
using Breach.Placeable.Component;

namespace Breach.Manager
{
    public class InitChainManager : SingletionBase<InitChainManager>
    {
        void Start()
        {
            InitWaypointInitializer(); //spawns and inits all the waypoints

            InitCharacterMovment(); //needs waypoints

            InitCover(); // needs character movement

            WaypointManager.Instance.Init(); // needs waypoints
        }

        private static void InitCover()
        {
            Cover[] covers = FindObjectsOfType<Cover>();
            foreach (Cover cover in covers)
            {
                cover.Init();
            }
        }

        private static void InitCharacterMovment()
        {
            CharacterMovement[] movements = FindObjectsOfType<CharacterMovement>();
            foreach (CharacterMovement movement in movements)
            {
                movement.Init();
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
    }
}
