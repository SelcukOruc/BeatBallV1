using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BallTriggerController : MonoBehaviourPun
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "redpost")
            GameManager.ins.GetComponent<PhotonView>().RPC("OnGreenTeamScored", RpcTarget.AllViaServer);

        if (collision.gameObject.tag == "greenpost")
            GameManager.ins.GetComponent<PhotonView>().RPC("OnRedTeamScored", RpcTarget.AllViaServer);


    }








}
