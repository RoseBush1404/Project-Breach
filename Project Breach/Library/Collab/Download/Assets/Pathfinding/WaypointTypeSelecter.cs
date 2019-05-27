using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Pathfinding;

namespace Breach.Manager.Pathfinding
{
    public enum TileType { Walkable, Ladder, Stairs };

    public class WaypointTypeSelecter : SingletionBase<WaypointTypeSelecter>
    {

        public TileType SetTileType(string tileName)
        {
            TileType tileType;

            switch (tileName)
            {
                case "Walkable":
                    tileType = TileType.Walkable;
                    break;

                case "Ladder":
                    tileType = TileType.Ladder;
                    break;

                case "Left Stairs":
                    tileType = TileType.Stairs;
                    break;

                case "Right Stairs":
                    tileType = TileType.Stairs;
                    break;

                default:
                    tileType = TileType.Walkable;
                    break;
            }
            return tileType;
        }

        public List<Vector2> SetDirectionLimitations(Waypoint waypoint, string tileName)
        {
            List<Vector2> validDirections = new List<Vector2> { };
            switch (waypoint.GetTileType())
            {
                case TileType.Walkable:
                    validDirections.Add(Vector2Int.up);
                    validDirections.Add(Vector2Int.down);
                    validDirections.Add(Vector2Int.left);
                    validDirections.Add(Vector2Int.right);
                    validDirections.Add(new Vector2Int(1, 1));
                    validDirections.Add(new Vector2Int(1, -1));
                    validDirections.Add(new Vector2Int(-1, -1));
                    validDirections.Add(new Vector2Int(-1, 1));
                    break;

                case TileType.Ladder:
                    validDirections.Add(Vector2Int.up);
                    validDirections.Add(Vector2Int.down);
                    validDirections.Add(Vector2Int.left);
                    validDirections.Add(Vector2Int.right);
                    break;

                case TileType.Stairs:
                    if (tileName == "Left Stairs")
                    {
                        validDirections.Add(new Vector2Int(1, -1));
                        validDirections.Add(new Vector2Int(-1, 1));
                    }
                    else
                    {
                        validDirections.Add(new Vector2Int(1, 1));
                        validDirections.Add(new Vector2Int(-1, -1));
                    }
                    break;

                default:
                    validDirections.Add(Vector2Int.up);
                    validDirections.Add(Vector2Int.down);
                    validDirections.Add(Vector2Int.left);
                    validDirections.Add(Vector2Int.right);
                    validDirections.Add(new Vector2Int(1, 1));
                    validDirections.Add(new Vector2Int(1, -1));
                    validDirections.Add(new Vector2Int(-1, -1));
                    validDirections.Add(new Vector2Int(-1, 1));
                    break;
            }
            return validDirections;
        }
    }
}
