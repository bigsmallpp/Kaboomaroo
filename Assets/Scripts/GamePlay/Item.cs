using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{
    public int _id = 0;
    private ItemSpawner spawner;
    private SFXPlayer sfx;
    private bool _used = false;
    void Start()
    {
        Debug.Log("Item spawned at: " + gameObject.transform.position);
        spawner = FindObjectOfType<ItemSpawner>();
        sfx = FindObjectOfType<SFXPlayer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_used)
        {
            return;
        }
        //Do item stuff
        if (collision.gameObject.CompareTag("Player"))
        {
            sfx.CollectItem();
            collision.gameObject.GetComponent<PlayerController>().increaseRadiusItem(_used);
            _used = true;
            spawner.deleteItem(_id);
            Destroy(this.gameObject);
        }
        //Destroy(this.gameObject);
    }

    public void setId(int id)
    {
        _id = id;
    }

    public int getId()
    {
        return _id;
    }
}
