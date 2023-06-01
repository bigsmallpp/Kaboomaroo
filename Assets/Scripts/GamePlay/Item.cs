using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{
    public enum ItemType
    {
        unset,
        radiusUpgrade,
        bombLimitUpgrade,
        speedUpgrade
    }

    public int _id = 0;
    private ItemSpawner spawner;
    private bool _used = false;
    public ItemType itemType;
    private SpriteRenderer spriteRenderer;
    [SerializeField] public List<Sprite> itemIcons;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemType = ItemType.unset;
    }
    void Start()
    {
        Debug.Log("Item spawned at: " + gameObject.transform.position);
        spawner = FindObjectOfType<ItemSpawner>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_used)
        {
            return;
        }
        //Do item stuff
        //Decide each action for itemType
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (itemType)
            {
                case ItemType.unset:
                    Debug.LogError("Itemtype not set!!");
                    return;
                    break;

                case ItemType.radiusUpgrade:
                    collision.gameObject.GetComponent<PlayerController>().increaseRadiusItem(_used);
                    Debug.Log("Radius Upgrade used");
                    break;

                case ItemType.bombLimitUpgrade:
                    Debug.Log("Bomblimit Upgrade used");
                    break;

                case ItemType.speedUpgrade:
                    Debug.Log("Speed Upgrade used");
                    break;

                default:
                    Debug.LogError("No item type given");
                    return;
                    break;
            }
            _used = true;
            deleteItem();
        }
        //Destroy(this.gameObject);
    }

    private void deleteItem()
    {
        spawner.deleteItem(_id);
        Destroy(this.gameObject);
    }

    public void setId(int id)
    {
        _id = id;
    }

    public int getId()
    {
        return _id;
    }

    private void setItemType(ItemType type)
    {
        itemType = type;
    }

    private void setItemIcon()
    {
        switch (itemType)
        {
            case ItemType.unset:
                spriteRenderer.sprite = itemIcons[0];
                break;

            case ItemType.radiusUpgrade:
                spriteRenderer.sprite = itemIcons[1];
                break;

            case ItemType.bombLimitUpgrade:
                spriteRenderer.sprite = itemIcons[2];
                break;

            case ItemType.speedUpgrade:
                spriteRenderer.sprite = itemIcons[3];
                break;

            default:
                break;
        }
    }

    public void initItem(int id, ItemType type)
    {
        setId(id);
        setItemType(type);
        setItemIcon();
    }
}
