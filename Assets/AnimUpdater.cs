using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimUpdater : NetworkBehaviour
{

    private Animator anim;
    public int _skin_variant = 1;
    private PlayerController _controller;
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponentInParent<PlayerController>();
        anim = GetComponent<Animator>();
        _skin_variant = _controller.skin_variant.Value;
        setPlayerVariant(_skin_variant); //Default Skin
        //TODO: Set Player skin at startup of lobby
    }

    public void setPlayerVariant(int variant)
    {
        switch (variant)
        {
            case 1:
                anim.SetInteger("variant", 1);
                break;
            case 2:
                anim.SetInteger("variant", 2);//Swap to debug
                break;
            case 3:
                anim.SetInteger("variant", 3);
                break;
            case 4:
                anim.SetInteger("variant", 4);
                break;
            default:
                break;
        }
    }

    public void updateAnim(PlayerController.Direction curr_direction)
    {
        anim.SetBool("up", false);
        anim.SetBool("down", false);
        anim.SetBool("left", false);
        anim.SetBool("right", false);
        Debug.Log("[KEK] direction: " + curr_direction);

        switch (curr_direction)
        {
            case PlayerController.Direction.Up:
                anim.SetBool("up", true);
                SFXPlayer.Instance.Step();
                break;
            case PlayerController.Direction.Down:
                anim.SetBool("down", true);
                SFXPlayer.Instance.Step();
                break;
            case PlayerController.Direction.Left:
                anim.SetBool("left", true);
                SFXPlayer.Instance.Step();
                break;
            case PlayerController.Direction.Right:
                anim.SetBool("right", true);
                SFXPlayer.Instance.Step();
                break;
            case PlayerController.Direction.Idle:
                SFXPlayer.Instance.StopStep();
                anim.SetBool("up", false);
                anim.SetBool("down", false);
                anim.SetBool("left", false);
                anim.SetBool("right", false);
                break;
            default:
                break;
        }
    }

    public void animPlayerDead()
    {
        Debug.Log("ANIM PLAYER DEAD CALLED");
        anim.SetBool("up", false);
        anim.SetBool("down", false);
        anim.SetBool("left", false);
        anim.SetBool("right", false);
        anim.SetBool("died", true);
    }
}
