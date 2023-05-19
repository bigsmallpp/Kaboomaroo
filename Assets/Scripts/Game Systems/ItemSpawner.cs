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
    private NetworkVariable<Vector2> _position1 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position2 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position3 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position4 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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
        if (IsServer)
        {
            calcPositions();
        }

        items[0] = Instantiate(_prefItem, _position1.Value, Quaternion.identity);
        items[1] = Instantiate(_prefItem, _position2.Value, Quaternion.identity);
        items[2] = Instantiate(_prefItem, _position3.Value, Quaternion.identity);
        items[3] = Instantiate(_prefItem, _position4.Value, Quaternion.identity);
        items[0].setId(0);
        items[1].setId(1);
        items[2].setId(2);
        items[3].setId(3);
        //SpawnItemsClientRpc(_position1.Value);
    }

    public void calcPositions()
    {
        BoundsInt bounds = _tilesIndestructible.cellBounds;
        _position1.Value = getRandomPositionForItem(bounds.xMin, 0, 0, bounds.yMax);
        _position2.Value = getRandomPositionForItem(0, bounds.xMax, 0, bounds.yMax);
        _position3.Value = getRandomPositionForItem(bounds.xMin, 0, bounds.yMin, 0);
        _position4.Value = getRandomPositionForItem(0, bounds.xMax, bounds.yMin, 0);
    }
    /*[ClientRpc]
    private void SpawnItemsClientRpc(Vector2 pos)
    {
        if (IsServer)
        {
            return;
        }
        Instantiate(_prefItem, pos, Quaternion.identity);
        Debug.Log("Init item at pos: " + pos);
    }*/

    /*[ClientRpc]
    private void SpawnItemAtPositionClientRpc(Vector3Int pos)
    {
        if (IsServer)
        {
            return;
        }

        Vector3Int position = new Vector3Int(pos.x, pos.y, 0);
        Instantiate(_prefItem, position, Quaternion.identity);
        Debug.Log("Spawned item at position: " + position.x + ", " + position.y);
    }*/
    private Vector2Int getRandomPos(int xMin, int xMax, int yMin, int yMax)
    {
        Vector2Int pos = Vector2Int.zero;

        pos.x = Random.Range(xMin, xMax);
        pos.y = Random.Range(yMin, yMax);

        return pos;
    }

    private Vector2Int getRandomPositionForItem(int xMin, int xMax, int yMin, int yMax)
    {
        TileBase randTile = null;
        Vector2Int randPos = Vector2Int.zero;
        while (randTile == null)
        {
            randPos = getRandomPos(xMin, xMax, yMin, yMax);
            Vector3Int tilePos = new Vector3Int(randPos.x, randPos.y, _tilesDestructible.cellBounds.z);
            randTile = _tilesDestructible.GetTile(tilePos);
        }
        return randPos;
    }

    public void deleteItem(int index)
    {
        if (items[index] == null)
        {
            Debug.Log("No item found with index: " + index);  
        }

        Destroy(items[index].gameObject);
        items[index] = null;

        if (IsServer)
        {
            removeItemClientRPC(index);
        }
        else
        {
            removeItemServerRPC(index);
        }
        
        
    }

    [ClientRpc]
    private void removeItemClientRPC(int index)
    {
        //Debug.Log("Enter item Client RPC: " + index);
        if (IsServer)
        {
            return;
        }

        if (items[index] == null)
        {
            Debug.Log("No item found with index: " + index);
            return;
        }

        Destroy(items[index].gameObject);
        items[index] = null;
        //Debug.Log("Destroy item Client RPC: " + index);
    }

    [ServerRpc(RequireOwnership = false)]
    private void removeItemServerRPC(int index)
    {
        //Debug.Log("Enter item Server RPC: " + index);
        if (items[index] == null)
        {
            Debug.Log("No item found with index: " + index);
            return;
        }

        Destroy(items[index].gameObject);
        items[index] = null;
        //Debug.Log("Destroy item Server RPC: " + index);
    }
}
