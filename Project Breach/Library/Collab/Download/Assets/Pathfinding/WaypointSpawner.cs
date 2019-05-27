using UnityEngine;
using UnityEngine.Tilemaps;

public class WaypointSpawner : MonoBehaviour {

    [SerializeField] GameObject waypointToSpawn;

    void Awake ()
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
                Instantiate(waypointToSpawn, tileWorldPos, Quaternion.identity, gameObject.transform);
            }
        }
    }
}
