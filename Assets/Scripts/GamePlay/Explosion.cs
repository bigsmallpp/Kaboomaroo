using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Explosion : NetworkBehaviour
{
    public enum ExplosionType
    {
        CENTER,
        MIDDLE,
        EDGE
    }
    
    [Header("The Sprites")]
    [SerializeField] private List<Sprite> _explosionCenter;
    [SerializeField] private List<Sprite> _explosionMiddle;
    [SerializeField] private List<Sprite> _explosionEdge;
    [SerializeField] private SpriteRenderer _sprite;

    [Header("Duration of Explosion")]
    [SerializeField] private float _duration;
    
    private ExplosionType _type;
    private NetworkVariable<int> _typeNetworked = new NetworkVariable<int>();
    private NetworkVariable<float> _rotationNetworked = new NetworkVariable<float>();
    private float _durationSegment;
    
    private float _remainingTimeDuration;
    private float _remainingTimeSegment;
    private int _spriteIndex = 0;

    public override void OnNetworkSpawn()
    {
        _durationSegment = _duration / _explosionCenter.Count;
        _remainingTimeSegment = _durationSegment;
        _remainingTimeDuration = _duration;

        StartCameraShake();

        if (!IsServer)
        {
            _type = (ExplosionType)_typeNetworked.Value;
            Vector3 rotation = transform.rotation.eulerAngles; 
            rotation.z = _rotationNetworked.Value;
            transform.rotation = Quaternion.Euler(rotation);
            SwitchToNextSprite(false);
        }
        else
        {
            _typeNetworked.Value = (int)_type;
            _rotationNetworked.Value = gameObject.transform.rotation.eulerAngles.z;
        }
    }

    private void Update()
    {
        _remainingTimeSegment -= Time.deltaTime;
        if (_remainingTimeSegment <= 0.0f)
        {
            SwitchToNextSprite();
            _remainingTimeSegment = _durationSegment;
        }

        _remainingTimeDuration -= Time.deltaTime;
        if (_remainingTimeDuration <= 0.0f)
        {
            DespawnObject();
        }
    }

    private void SwitchToNextSprite(bool update_index = true)
    {
        switch (_type)
        {
            case ExplosionType.CENTER:
                _sprite.sprite = _explosionCenter[_spriteIndex];
                break;
            
            case ExplosionType.EDGE:
                _sprite.sprite = _explosionEdge[_spriteIndex];
                break;
            
            case ExplosionType.MIDDLE:
                _sprite.sprite = _explosionMiddle[_spriteIndex];
                break;
            
            default:
                Debug.LogError("Unknown Explosion Type");
                break;
        }

        if (update_index && _spriteIndex < (_explosionCenter.Count - 2))
        {
            ++_spriteIndex;
        }
    }

    private void DespawnObject()
    {
        if (!IsServer)
        {
            return;
        }
        
        GetComponent<NetworkObject>().Despawn();
    }

    public void SetRotationAndType(float z_rotation, ExplosionType type)
    {
        _type = type;
        Vector3 rotation = transform.rotation.eulerAngles; 
        rotation.z = z_rotation;
        transform.rotation = Quaternion.Euler(rotation);
        SwitchToNextSprite();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer)
        {
            return;
        }

        Debug.Log("Hit Player with ID " + col.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        if (col.gameObject.CompareTag("Player"))
        {
            // TODO Give Player Hit Feedback
            col.gameObject.GetComponent<PlayerController>().DisableControls();
            col.gameObject.GetComponent<PlayerController>().killPlayer();
            //col.gameObject.GetComponent<NetworkObject>().Despawn();
            col.gameObject.GetComponent<PlayerController>().setSpeed(0.0f);
            ulong client_id = col.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            GameObject.FindWithTag("NetworkedMenuManager").GetComponent<NetworkedGameMenus>().RPC_SwitchToDeathMessageClientRPC(client_id);
            col.gameObject.GetComponent<PlayerController>().setAliveStatus(false);
        }
    }

    private void StartCameraShake()
    {
        Camera.main.GetComponent<CameraShake>().ShakeCamera();
    }
}
