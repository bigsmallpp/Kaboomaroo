using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private Tilemap _tilesDestructible;
    [SerializeField] private Tilemap _tilesIndestructible;
    [SerializeField] private Item _prefItem;

    public void setTileMap(Tilemap destr, Tilemap indestr)
    {
        _tilesDestructible = destr;
        _tilesIndestructible = indestr;
    }
    public void initItems()
    {
        BoundsInt bounds = _tilesIndestructible.cellBounds;

        Vector3Int pos1 = getRandomPositionForItem(bounds.xMin, 0, 0, bounds.yMax);
        Vector3Int pos2 = getRandomPositionForItem(0, bounds.xMax, 0, bounds.yMax);
        Vector3Int pos3 = getRandomPositionForItem(bounds.xMin, 0, bounds.yMin, 0);
        Vector3Int pos4 = getRandomPositionForItem(0, bounds.xMax, bounds.yMin, 0);

        Instantiate(_prefItem, pos1, Quaternion.identity);
        Instantiate(_prefItem, pos2, Quaternion.identity);
        Instantiate(_prefItem, pos3, Quaternion.identity);
        Instantiate(_prefItem, pos4, Quaternion.identity);
    }
    private Vector3Int getRandomPos(int xMin, int xMax, int yMin, int yMax, int z)
    {
        Vector3Int pos = Vector3Int.zero;

        pos.x = Random.Range(xMin, xMax);
        pos.y = Random.Range(yMin, yMax);
        pos.z = z;

        return pos;
    }

    private Vector3Int getRandomPositionForItem(int xMin, int xMax, int yMin, int yMax)
    {
        TileBase randTile = null;
        Vector3Int randPos = Vector3Int.zero;
        while (randTile == null)
        {
            randPos = getRandomPos(xMin, xMax, yMin, yMax, _tilesDestructible.cellBounds.z);
            randTile = _tilesDestructible.GetTile(randPos);
        }
        return randPos;
    }
}
