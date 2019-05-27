using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Breach.Manager.Pathfinding;
using Breach.Placeable.Component;
using Breach.Placeable.Characters;

namespace Breach.Placeable.Pathfinding
{
    public class Waypoint : MonoBehaviour
    {

        public bool isExplored = false;
        public Waypoint exploredFrom;
        public int rangeFromCharacter;
        public Vector2 direction;
        public bool isSelectable = true;
        public Character characterOnTile;
        public List<Cover> CoverOnThisTile = new List<Cover>();

        [SerializeField] SpriteRenderer mainSprite;
        [SerializeField] SpriteRenderer shieldSprite;
        [SerializeField] SpriteRenderer pathSprite;

        TileType tileType;
        List<Vector2> validDirections = new List<Vector2> { };

        const int gridSize = 1;

        public void Init()
        {
            LabelTheGameObject();
            tileType = WaypointTypeSelecter.Instance.SetTileType(GetTileName());
            validDirections = WaypointTypeSelecter.Instance.SetDirectionLimitations(this, GetTileName());
            shieldSprite.enabled = false;
        }

        private void LabelTheGameObject()
        {
            Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            string label = "(" + gridPos.x + ", " + gridPos.y + ")";
            gameObject.name = label;
        }

        public string GetTileName()
        {
            Tilemap tilemap = GetComponentInParent<Tilemap>();
            Vector3Int gridPos = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);
            return tilemap.GetTile(gridPos).name;
        }

        public TileType GetTileType()
        {
            return tileType;
        }

        public List<Vector2> GetValidDirections()
        {
            return validDirections;
        }

        public int GetGridSize()
        {
            return gridSize;
        }

        public Vector2 GetGridPos()
        {
            return new Vector2(
                (transform.position.x / gridSize),
                (transform.position.y / gridSize)
            );
        }

        public void SetTileColour(Color color)
        {
            GetComponent<SpriteRenderer>().color = color;
        }

        public void ResetWaypointValues()
        {
            isExplored = false;
            exploredFrom = null;
            rangeFromCharacter = 0;
        }

        public void TurnSpritesOn()
        {
            SetMainSprite(true);
            SetShieldSprite(true);
        }

        public void TurnSpritesOff()
        {
            SetMainSprite(false);
            SetShieldSprite(false);
        }

        private void SetMainSprite(bool isOn)
        {
            
            mainSprite.enabled = isOn;
        }

        private void SetShieldSprite(bool isOn)
        {
            if(CoverOnThisTile.Count > 0)
            {
                shieldSprite.enabled = isOn;
            }
        }

        public void SetPathSpriteVisability(bool isOn)
        {
            pathSprite.enabled = isOn;
        }

        public void LandedOnWaypoint(Character character)
        {
            if(CoverOnThisTile.Count > 0)
            {
                foreach(Cover cover in CoverOnThisTile)
                {
                    cover.EnableCover(character);
                }
            }
        }
    }
}