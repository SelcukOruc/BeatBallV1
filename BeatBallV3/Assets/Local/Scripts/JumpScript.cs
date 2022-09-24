using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class JumpScript : MonoBehaviour
{
    public bool IsGrounded;
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private PhotonView view;
    [SerializeField] private Transform hips;
   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            IsGrounded = true;

        if (view.IsMine)
        {
            if (collision.gameObject.tag == "RedTeamArea")
            {
                view.RPC("TeamRed", RpcTarget.All);  // IS RPC NECESSARY HERE? , I WILL TRY ANOTHER. (22.09.2022)

            }

            if (collision.gameObject.tag == "YellowTeamArea")
            {
                view.RPC("TeamYellow", RpcTarget.All);

            }
            if (collision.gameObject.tag == "DeathPanel")
            {
                hips.position = Vector3.zero;
            }
            
        
        }
       
       
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            IsGrounded = false;

    }




}
