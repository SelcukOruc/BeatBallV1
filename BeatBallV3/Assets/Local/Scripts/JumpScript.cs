using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class JumpScript : MonoBehaviour
{
    public bool IsGrounded;
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private PhotonView Main_view;
    [SerializeField] private Transform hips;

    // 24.09.2022 Jump logic is only a place holder for the time being.
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
            IsGrounded = true;

        if (Main_view.IsMine)
        {
            
            if (other.gameObject.tag == "RedTeamArea")
            {
                Main_view.RPC("TeamRed", RpcTarget.All);  // This RPC is in PlayerStat.

            }

            if (other.gameObject.tag == "YellowTeamArea")
            {
                Main_view.RPC("TeamYellow", RpcTarget.All);

            }
            if (other.gameObject.tag == "DeathPanel")
            {
                hips.position = Vector3.zero;
            }


        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground")
            IsGrounded = false;
    }



}




