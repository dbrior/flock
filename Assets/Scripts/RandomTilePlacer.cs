using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomTilePlacer : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap
    public TileBase[] grassTiles; // Array of grass tiles

    public int mapWidth;
    public int mapHeight;

    void Start()
    {
        RandomizeTiles();
    }

    void RandomizeTiles()
    {
        for (int x = -mapWidth; x < mapWidth; x++)
        {
            for (int y = -mapHeight; y < mapHeight; y++)
            {
                // Select a random grass tile
                TileBase randomTile = grassTiles[Random.Range(0, grassTiles.Length)];

                if (randomTile == null) continue;

                // Set the tile at the current position
                tilemap.SetTile(new Vector3Int(x, y, 0), randomTile);
            }
        }
    }
}