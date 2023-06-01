using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemSpawner : NetworkBehaviour
{
    private int _num_items = 8;
    [SerializeField] private Tilemap _tilesDestructible;
    [SerializeField] private Tilemap _tilesIndestructible;
    [SerializeField] private Item _prefItem;
    [SerializeField] public Item[] items;
    public int typeACounter = 0;
    public int typeBCounter = 0;
    public int typeCCounter = 0;
    public int randNum = 0;
    private NetworkVariable<Vector2> _position1 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position2 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position3 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position4 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position5 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position6 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position7 = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _position8 = new NetworkVariable<Vector2>(new Vector2(0, 0),
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
        items[4] = Instantiate(_prefItem, _position4.Value, Quaternion.identity);
        items[5] = Instantiate(_prefItem, _position4.Value, Quaternion.identity);
        items[6] = Instantiate(_prefItem, _position4.Value, Quaternion.identity);
        items[7] = Instantiate(_prefItem, _position4.Value, Quaternion.identity);
        /*items[0].setId(0);
        items[1].setId(1);
        items[2].setId(2);
        items[3].setId(3);*/
        items[0].initItem(0, getRandomItemType());
        items[1].initItem(1, getRandomItemType());
        items[2].initItem(2, getRandomItemType());
        items[3].initItem(3, getRandomItemType());
        items[4].initItem(4, getRandomItemType());
        items[5].initItem(5, getRandomItemType());
        items[6].initItem(6, getRandomItemType());
        items[7].initItem(7, getRandomItemType());
        //SpawnItemsClientRpc(_position1.Value);
    }

    public Item.ItemType getRandomItemType()
    {
        randNum = Random.Range(1, 3);
        Debug.Log("RandNum: " + randNum);
        Item.ItemType type;
        switch (randNum)
        {
            case 1:
                if (typeACounter >= 2)
                {
                    type = getRandomItemType();
                }
                else
                {
                    type = Item.ItemType.bombLimitUpgrade;
                    typeACounter++;
                }
                break;
            case 2:
                if (typeBCounter >= 2)
                {
                    type = getRandomItemType();
                }
                else
                {
                    type = Item.ItemType.radiusUpgrade;
                    typeBCounter++;
                }
                break;
            case 3:
                if (typeCCounter >= 2)
                {
                    type = getRandomItemType();
                }
                else
                {
                    type = Item.ItemType.speedUpgrade;
                    typeCCounter++;
                }
                break;
            default:
                type = Item.ItemType.unset;
                break;
        }
        return type;
    }

    public void calcPositions()
    {
        BoundsInt bounds = _tilesIndestructible.cellBounds;
        _position1.Value = getRandomPositionForItem(bounds.xMin, 0, 0, bounds.yMax);
        _position2.Value = getRandomPositionForItem(bounds.xMin, 0, 0, bounds.yMax);
        while (_position1.Value != _position2.Value)
        {
            _position2.Value = getRandomPositionForItem(bounds.xMin, 0, 0, bounds.yMax);
        }
        _position3.Value = getRandomPositionForItem(0, bounds.xMax, 0, bounds.yMax);
        _position4.Value = getRandomPositionForItem(0, bounds.xMax, 0, bounds.yMax);
        while (_position3.Value != _position4.Value)
        {
            _position4.Value = getRandomPositionForItem(0, bounds.xMax, 0, bounds.yMax);
        }
        _position5.Value = getRandomPositionForItem(bounds.xMin, 0, bounds.yMin, 0);
        _position6.Value = getRandomPositionForItem(bounds.xMin, 0, bounds.yMin, 0);
        while (_position5.Value != _position6.Value)
        {
            _position6.Value = getRandomPositionForItem(bounds.xMin, 0, bounds.yMin, 0);
        }
        _position7.Value = getRandomPositionForItem(0, bounds.xMax, bounds.yMin, 0);
        _position8.Value = getRandomPositionForItem(0, bounds.xMax, bounds.yMin, 0);
        while (_position7.Value != _position8.Value)
        {
            _position8.Value = getRandomPositionForItem(0, bounds.xMax, bounds.yMin, 0);
        }
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
