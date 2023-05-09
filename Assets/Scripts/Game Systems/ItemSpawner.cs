using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemSpawner : NetworkBehaviour
{
    private int _num_items = 4;
    [SerializeField] private Tilemap _tilesDestructible;
    [SerializeField] private Tilemap _tilesIndestructible;
    [SerializeField] private Item _prefItem;
    [SerializeField] public Item[] items;
    private Vector3Int[] positions;

    private void Awake()
    {
        items = new Item[_num_items];
    }

    public void setTileMap(Tilemap destr, Tilemap indestr)
    {
        _tilesDestructible = destr;
        _tilesIndestructible = indestr;
    }
    public void initItems()
    {
        positions = new Vector3Int[_num_items];
        if (IsServer)
        {
            calcPositions();
            items[0] = Instantiate(_prefItem, positions[0], Quaternion.identity);
            items[1] = Instantiate(_prefItem, positions[1], Quaternion.identity);
            items[2] = Instantiate(_prefItem, positions[2], Quaternion.identity);
            items[3] = Instantiate(_prefItem, positions[3], Quaternion.identity);
            items[0].setId(0); // id = index
            items[1].setId(1);
            items[2].setId(2);
            items[3].setId(3);
            SpawnItemsClientRpc(positions);
        }
    }

    public void calcPositions()
    {
        BoundsInt bounds = _tilesIndestructible.cellBounds;

        positions[0] = getRandomPositionForItem(bounds.xMin, 0, 0, bounds.yMax);
        positions[1] = getRandomPositionForItem(0, bounds.xMax, 0, bounds.yMax);
        positions[2] = getRandomPositionForItem(bounds.xMin, 0, bounds.yMin, 0);
        positions[3] = getRandomPositionForItem(0, bounds.xMax, bounds.yMin, 0);
    }
    [ClientRpc]
    private void SpawnItemsClientRpc(Vector3Int[] pos)
    {
        if (IsServer)
        {
            return;
        }
        items[0] = Instantiate(_prefItem, pos[0], Quaternion.identity);
        items[1] = Instantiate(_prefItem, pos[1], Quaternion.identity);
        items[2] = Instantiate(_prefItem, pos[2], Quaternion.identity);
        items[3] = Instantiate(_prefItem, pos[3], Quaternion.identity);
        items[0].setId(0);
        items[1].setId(1);
        items[2].setId(2);
        items[3].setId(3);
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

    public void deleteItem(int index)
    {
        /*if (IsServer)
        {
            Destroy(items[index]);
            removeItemClientRPC(index);
        }*/
        if (items[index] != null)
        {
            Debug.Log("Delete item: " + index);
            Destroy(items[index].gameObject);
            items[index] = null;
        }
        else
        {
            Debug.Log("No item found with index: " + index);
        }
        
    }

    [ClientRpc]
    private void removeItemClientRPC(int index)
    {
        if (items[index] != null)
        {
            Destroy(items[index].gameObject);
            items[index] = null;
        }
        else
        {
            Debug.Log("No item found with index: " + index);
        }
    }
}
