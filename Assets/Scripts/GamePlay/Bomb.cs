using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : NetworkBehaviour
{
    [SerializeField] private float _timer = 3.0f;
    [SerializeField] private NetworkVariable<int> _radius = new NetworkVariable<int>(1);

    private List<Vector2> _hitDirections = new List<Vector2>() { Vector2.up, Vector2.right, Vector2.down, Vector2.left }; 
    private TileManager _tileManager = null;
    private GameObject _owner = null;
    private bool _ignoreCollisionWithPlayer = true;
    private int _radiusLocal = 1;

    [Header("Explosion Prefab")]
    [SerializeField] private GameObject _prefExplosion;
    
    public override void OnNetworkSpawn()
    {
        SFXPlayer.Instance.PlaceBomb();
        if (IsServer)
        {
            _tileManager = GameObject.FindWithTag("TileManager").GetComponent<TileManager>();
            _radius.Value = _radiusLocal;
        }
        else
        {
            _owner = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
            CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
            CircleCollider2D owner_col = _owner.gameObject.GetComponent<CircleCollider2D>();
            Physics2D.IgnoreCollision(col, owner_col, true);
        }
    }

    public void SetRadius(int radius)
    {
        _radiusLocal = radius;
    }

    public void SetTimer(float timer)
    {
        _timer = timer;
    }

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
        gameObject.AddComponent<CircleCollider2D>();
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), owner.gameObject.GetComponent<CircleCollider2D>(), true);
    }

    private void Update()
    {
        CheckOwnerLeftCollisionRange();
        ReduceTimer();
    }

    private void Explode()
    {
        if (!IsServer)
        {
            return;
        }

        Vector2 pos = gameObject.transform.position;
        pos.x = Mathf.RoundToInt(pos.x);
        pos.y = Mathf.RoundToInt(pos.y);
        
        Dictionary<Vector2, List<Vector2>> hit_fields = new Dictionary<Vector2, List<Vector2>>()
        {
            { Vector2.up, new List<Vector2>() },
            { Vector2.right, new List<Vector2>() },
            { Vector2.down, new List<Vector2>() },
            { Vector2.left, new List<Vector2>() },
        };

        Debug.LogError(_radius.Value);
        foreach (Vector2 direction in _hitDirections)
        {
            for (int radius = 1; radius <= _radius.Value; radius++)
            {
                Vector2 tile_pos = pos + (direction * radius);
                
                // Check if explosion hit indestructible tile -> stop explosion
                if (_tileManager.CheckIndestructibleTile(tile_pos))
                {
                    break;
                }

                // Check if explosion hit indestructible tile -> remove destructible tile
                if (_tileManager.CheckDestructibleTile(tile_pos))
                {
                    hit_fields[direction].Add(tile_pos);
                }
            }
        }
        
        SpawnExplosions(pos, hit_fields);

        _owner.GetComponent<PlayerController>().DecreaseActiveBombs(_owner);
        GetComponent<NetworkObject>().Despawn();
    }

    private void CheckOwnerLeftCollisionRange()
    {
        if(_owner == null || !_ignoreCollisionWithPlayer)
            return;

        if (Vector3.Magnitude(_owner.transform.position - gameObject.transform.position) >
            GetComponent<CircleCollider2D>().radius + 0.1f)
        {
            _ignoreCollisionWithPlayer = false;
            Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), _owner.gameObject.GetComponent<CircleCollider2D>(), false);
        }
    }

    private void ReduceTimer()
    {
        if (!IsServer)
        {
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer <= 0.0f)
        {
            Explode();
        }
    }

    private void SpawnExplosions(Vector2 Center, Dictionary<Vector2, List<Vector2>> hit_fields)
    {
        // Already in Server Only Section
        GameObject explosion_center = Instantiate(_prefExplosion, Center, Quaternion.identity);
        explosion_center.GetComponent<Explosion>().SetRotationAndType(0.0f, Explosion.ExplosionType.CENTER);
        explosion_center.GetComponent<NetworkObject>().Spawn();

        foreach (KeyValuePair<Vector2, List<Vector2>> hits in hit_fields)
        {
            int field_count = hits.Value.Count;
            int index = 1;
            
            foreach (Vector2 field in hits.Value)
            {
                Explosion.ExplosionType type = field_count == index
                    ? Explosion.ExplosionType.EDGE
                    : Explosion.ExplosionType.MIDDLE;
                
                GameObject explosion = Instantiate(_prefExplosion, field, Quaternion.identity);
                explosion.GetComponent<Explosion>().SetRotationAndType(CalcRotation(hits.Key), type);
                explosion.GetComponent<NetworkObject>().Spawn();
                index++;
                
                Debug.Log("Instantiated explosion at " + field);
            }
        }
    }

    private float CalcRotation(Vector2 dir)
    {
        float rotation = + dir.y * 90.0f;
        if (rotation == 0.0f)
        {
            rotation = (int)dir.x == 1 ? 0.0f : 180.0f;
        }

        return rotation;
    }
}
