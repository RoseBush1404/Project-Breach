using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Pathfinding;
using Breach.Placeable.Component;

namespace Breach.Manager.Pathfinding
{

    public class WaypointManager : SingletionBase<WaypointManager>
    { //TODO like class itself, dont like how its called

        [SerializeField] Color inRangeColour = Color.green;
        [SerializeField] Color selectedColour = Color.blue;

        // TODO consider changing to arrays
        private List<Waypoint> changedWaypointsSprites = new List<Waypoint>();
        private List<Waypoint> changedWaypointsValues = new List<Waypoint>();
        private List<Waypoint> pathOfSprites = new List<Waypoint>();

        bool waypointsAreEnabled = true;

        //-----------------------------------------------------------

        public void Init()
        {
            DisableWaypointVisibility();
        }

        //-----------------------------------------------------------

        public void SetTileToInRange(Waypoint waypoint)
        {
            waypoint.SetTileColour(inRangeColour);
            changedWaypointsSprites.Add(waypoint);
            changedWaypointsValues.Add(waypoint);
        }

        public void SetTileToSelectedTile(Waypoint waypoint)
        {
            waypoint.SetTileColour(selectedColour);
        }

        //-----------------------------------------------------------

        public void DisableSingleSpriteNode(Waypoint waypoint)
        {
            waypoint.SetPathSpriteVisability(false);
        }

        public void SetAllChangedTilesToDefaultSprite()
        {
            foreach (Waypoint waypoint in changedWaypointsSprites)
            {
                waypoint.SetTileColour(Color.white);
            }
            changedWaypointsSprites.Clear();
        }

        public void SetAllChangedTilesToDefaultSettings()
        {
            foreach (Waypoint waypoint in changedWaypointsValues)
            {
                waypoint.ResetWaypointValues();
            }
            changedWaypointsValues.Clear();
        }

        //-----------------------------------------------------------

        public void EnableWaypointVisibility()
        {
            if (!waypointsAreEnabled)
            {
                waypointsAreEnabled = true;
                Waypoint[] waypoints = FindObjectsOfType<Waypoint>();
                foreach(Waypoint waypoint in waypoints)
                {
                    waypoint.TurnSpritesOn();
                }
            }
        }

        public void DisableWaypointVisibility()
        {
            if (waypointsAreEnabled)
            {
                waypointsAreEnabled = false;
                Waypoint[] waypoints = FindObjectsOfType<Waypoint>();
                foreach(Waypoint waypoint in waypoints)
                {
                    waypoint.TurnSpritesOff();
                }
            }
        }

        public void SetPathOfSprites(List<Waypoint> path = null)
        {
            //clear path when deselecting
            if (pathOfSprites.Count > 0)
            {
                foreach (Waypoint waypoint in pathOfSprites)
                {
                    waypoint.SetPathSpriteVisability(false);
                }
            }
            pathOfSprites.Clear();

            //if new path is passed through, cope list
            if (path != null)
            {
                pathOfSprites = new List<Waypoint>(path);
            }

            //make new path
            if (pathOfSprites.Count > 0)
            {
                foreach (Waypoint waypoint in pathOfSprites)
                {
                    waypoint.SetPathSpriteVisability(true);
                }
            }
        }
    }
}