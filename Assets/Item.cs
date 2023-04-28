using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Item spawned at: " + gameObject.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Do item stuff
        Destroy(this.gameObject);
    }
}
