using UnityEngine;
using UnityEngine.Tilemaps;
using Breach.Placeable.Pathfinding;

namespace Breach.Manager.Pathfinding
{
    public class WaypointInitializer : MonoBehaviour
    {

        [SerializeField] GameObject waypointToSpawn;

        public void Init()
        {
            Tilemap tilemap = GetComponent<Tilemap>();

            foreach (Vector3Int tile in tilemap.cellBounds.allPositionsWithin)
            {
                Vector3Int tileLocalPos = new Vector3Int(tile.x, tile.y, tile.z);
                Vector3 tileWorldPos = tilemap.CellToWorld(tileLocalPos);
                Vector3 gridOffSet = new Vector3(0.5f, 0.5f, 0);
                tileWorldPos = tileWorldPos + gridOffSet;

                if (tilemap.HasTile(tileLocalPos))
                {
                    GameObject newWaypointGO = Instantiate(waypointToSpawn, tileWorldPos, Quaternion.identity, gameObject.transform);
                    newWaypointGO.GetComponent<Waypoint>().Init();
                    newWaypointGO.GetComponent<Waypoint>().SetPathSpriteVisability(false);
                }
            }
        }
    }
}