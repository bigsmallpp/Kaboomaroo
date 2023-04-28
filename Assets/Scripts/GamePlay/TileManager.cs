using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : NetworkBehaviour
{
    [SerializeField] private Tile _tileIndestructible;
    [SerializeField] private Tile _tileDestructible;

    [SerializeField] private Tilemap _mapIndestructible;
    [SerializeField] private Tilemap _mapDestructible;

    public void PrintMap()
    {
        Debug.Log(_mapDestructible.cellBounds);

        BoundsInt bounds = _mapDestructible.cellBounds;
        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Debug.Log("Tile at " + row + " " + col + "\t" + _mapDestructible.GetTile(new Vector3Int(col, row)));
            }
        }
    }

    public void SetTileMaps(Tilemap destructible, Tilemap indestructible)
    {
        _mapDestructible = destructible;
        _mapIndestructible = indestructible;
    }

    public void RemoveTileAtPosition(Vector3Int pos)
    {
        _mapDestructible.SetTile(pos, null);
        _mapDestructible.RefreshTile(pos);
        
        RemoveTileAtPositionClientRpc(new Vector2Int(pos.x, pos.y));
    }

    public void RemoveFirstDestructibleTiles(int amount)
    {
        BoundsInt bounds = _mapDestructible.cellBounds;
        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                if(amount == 0)
                    return;
                if (_mapDestructible.GetTile(new Vector3Int(col, row)) == _tileDestructible)
                {
                    Debug.Log("Removing Tile at " + col + " " + row + "\t" + _mapDestructible.GetTile(new Vector3Int(col, row)));
                    _mapDestructible.SetTile(new Vector3Int(col, row), null);
                    _mapDestructible.RefreshTile(new Vector3Int(col, row));
                    amount--;
                    RemoveTileAtPositionClientRpc(new Vector2Int(col, row));
                }
            }
        }
    }

    [ClientRpc]
    private void RemoveTileAtPositionClientRpc(Vector2Int pos)
    {
        // Hosting player is Server + Client
        if (IsServer)
        {
            return;
        }
        
        Vector3Int position = new Vector3Int(pos.x, pos.y, 0);
        _mapDestructible.SetTile(position, null);
        _mapDestructible.RefreshTile(position);
    }

    public TileBase GetTileOfDestructibleMap(Vector3Int pos)
    {
        return _mapDestructible.GetTile(pos);
    }

    public bool CheckIndestructibleTile(Vector2 pos)
    {
        Vector3Int cell_pos = _mapIndestructible.WorldToCell(pos);
        return _mapIndestructible.GetTile(cell_pos) == _tileIndestructible;
    }
    
    public bool CheckDestructibleTile(Vector2 pos)
    {
        Vector3Int cell_pos = _mapDestructible.WorldToCell(pos);
        TileBase tile_to_check = _mapDestructible.GetTile(cell_pos);
        bool hit = tile_to_check == _tileDestructible || tile_to_check == null;

        if (hit)
        {
            RemoveTileAtPosition(cell_pos);
        }

        return hit;
    }
}
