using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance { get; private set; }
    
    [SerializeField] GameObject tileset_parent;
    [SerializeField] GameObject tileset;
    public Player player;

    List<GameObject> tiles = new List<GameObject>();

    private float tile_size = 3.84f;
    private Vector2Int grid_size = new Vector2Int(11, 7);

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void GenerateTiles()
    {
        for (int i = 10; i > -1; i--)
        {
            for (int j = 6; j > -1; j--)
            {
                // this maffs worked, idk how or why but it generated the outcome I was looking for
                tiles.Add(Instantiate(tileset, new Vector3((i * -tile_size) - 5 * -tile_size, (j * -tile_size) - 3 * -tile_size, 0), Quaternion.identity, tileset_parent.transform));
            }
        }
    }

    void Update()
    {
        if (player == null) return;
        
        foreach (var tile in tiles)
        {
            if (Mathf.Abs(player.transform.position.x - tile.transform.position.x) > 22)
            {
                float new_x = player.transform.position.x - tile.transform.position.x > 0 ? tile.transform.position.x + (grid_size.x * tile_size) : (tile.transform.position.x - (grid_size.x * tile_size));
                tile.transform.position = new Vector3(new_x, tile.transform.position.y, tile.transform.position.z);
            }

            if (Mathf.Abs(player.transform.position.y - tile.transform.position.y) > 14)
            {
                float new_y = player.transform.position.y - tile.transform.position.y > 0 ? tile.transform.position.y + (grid_size.y * tile_size) : (tile.transform.position.y - (grid_size.y * tile_size));
                tile.transform.position = new Vector3(tile.transform.position.x, new_y, tile.transform.position.z);
            }
        }
    }
}
