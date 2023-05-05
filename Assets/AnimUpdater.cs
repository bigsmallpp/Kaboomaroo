using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimUpdater : MonoBehaviour
{

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        setPlayerVariant(1); //Default Skin
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
                anim.SetInteger("variant", 2);
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
                break;
            case PlayerController.Direction.Down:
                anim.SetBool("down", true);
                break;
            case PlayerController.Direction.Left:
                anim.SetBool("left", true);
                break;
            case PlayerController.Direction.Right:
                anim.SetBool("right", true);
                break;
            case PlayerController.Direction.Idle:
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
        anim.SetBool("up", false);
        anim.SetBool("down", false);
        anim.SetBool("left", false);
        anim.SetBool("right", false);
        anim.SetBool("died", true);
    }
}
