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

    private NetworkVariable<Item.ItemType> _type1 = new NetworkVariable<Item.ItemType>(Item.ItemType.unset,
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Item.ItemType> _type2 = new NetworkVariable<Item.ItemType>(Item.ItemType.unset,
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Item.ItemType> _type3 = new NetworkVariable<Item.ItemType>(Item.ItemType.unset,
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Item.ItemType> _type4 = new NetworkVariable<Item.ItemType>(Item.ItemType.unset,
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Item.ItemType> _type5 = new NetworkVariable<Item.ItemType>(Item.ItemType.unset,
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Item.ItemType> _type6 = new NetworkVariable<Item.ItemType>(Item.ItemType.unset,
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Item.ItemType> _type7 = new NetworkVariable<Item.ItemType>(Item.ItemType.unset,
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Item.ItemType> _type8 = new NetworkVariable<Item.ItemType>(Item.ItemType.unset,
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
            calcRandomTypes();
        }

        items[0] = Instantiate(_prefItem, _position1.Value, Quaternion.identity);
        items[1] = Instantiate(_prefItem, _position2.Value, Quaternion.identity);
        items[2] = Instantiate(_prefItem, _position3.Value, Quaternion.identity);
        items[3] = Instantiate(_prefItem, _position4.Value, Quaternion.identity);
        items[4] = Instantiate(_prefItem, _position5.Value, Quaternion.identity);
        items[5] = Instantiate(_prefItem, _position6.Value, Quaternion.identity);
        items[6] = Instantiate(_prefItem, _position7.Value, Quaternion.identity);
        items[7] = Instantiate(_prefItem, _position8.Value, Quaternion.identity);

        items[0].initItem(0, _type1.Value);
        items[1].initItem(1, _type2.Value);
        items[2].initItem(2, _type3.Value);
        items[3].initItem(3, _type4.Value);
        items[4].initItem(4, _type5.Value);
        items[5].initItem(5, _type6.Value);
        items[6].initItem(6, _type7.Value);
        items[7].initItem(7, _type8.Value);
    }

    public Item.ItemType getRandomItemType()
    {
        randNum = Random.Range(1, 4);
        Debug.Log("RandNum: " + randNum);
        Item.ItemType type;
        switch (randNum)
        {
            case 1:
                type = Item.ItemType.bombLimitUpgrade;
                break;
            case 2:
                type = Item.ItemType.radiusUpgrade;
                break;
            case 3:
                type = Item.ItemType.speedUpgrade;
                break;
            default:
                type = Item.ItemType.unset;
                break;
        }
        return type;
    }

    public void calcRandomTypes()
    {
        _type1.Value = getRandomItemType();
        _type2.Value = getRandomItemType();
        _type3.Value = getRandomItemType();
        _type4.Value = getRandomItemType();
        _type5.Value = getRandomItemType();
        _type6.Value = getRandomItemType();
        _type7.Value = getRandomItemType();
        _type8.Value = getRandomItemType();
    }

    public void calcPositions()
    {
        BoundsInt bounds = _tilesIndestructible.cellBounds;
        _position1.Value = getRandomPositionForItem(bounds.xMin, 0, 0, bounds.yMax);
        _position2.Value = getRandomPositionForItem(bounds.xMin, 0, 0, bounds.yMax);
        while (_position1.Value == _position2.Value)
        {
            _position2.Value = getRandomPositionForItem(bounds.xMin, 0, 0, bounds.yMax);
        }
        _position3.Value = getRandomPositionForItem(0, bounds.xMax, 0, bounds.yMax);
        _position4.Value = getRandomPositionForItem(0, bounds.xMax, 0, bounds.yMax);
        while (_position3.Value == _position4.Value)
        {
            _position4.Value = getRandomPositionForItem(0, bounds.xMax, 0, bounds.yMax);
        }
        _position5.Value = getRandomPositionForItem(bounds.xMin, 0, bounds.yMin, 0);
        _position6.Value = getRandomPositionForItem(bounds.xMin, 0, bounds.yMin, 0);
        while (_position5.Value == _position6.Value)
        {
            _position6.Value = getRandomPositionForItem(bounds.xMin, 0, bounds.yMin, 0);
        }
        _position7.Value = getRandomPositionForItem(0, bounds.xMax, bounds.yMin, 0);
        _position8.Value = getRandomPositionForItem(0, bounds.xMax, bounds.yMin, 0);
        while (_position7.Value == _position8.Value)
        {
            _position8.Value = getRandomPositionForItem(0, bounds.xMax, bounds.yMin, 0);
        }
    }
    
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
