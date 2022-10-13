using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Step : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private PhotonView mainView;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "redarea")
            mainView.RPC("ChangeTeam", RpcTarget.All, true);
        if (collision.gameObject.tag == "greenarea")
            mainView.RPC("ChangeTeam", RpcTarget.All, false);
    }
}
