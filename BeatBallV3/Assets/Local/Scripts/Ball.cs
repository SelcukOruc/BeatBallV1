using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Ball : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView view;
    [SerializeField] private PhotonInGameController gameController;
    [SerializeField] private GameStat gameStat;
    public Transform player = null;

    private void Start()
    {
        gameController = GameObject.FindObjectOfType<PhotonInGameController>();
        gameStat = GameObject.FindObjectOfType<GameStat>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.gameObject.tag == "YellowInside")
                view.RPC("RPC_OnRedScored_Ball", RpcTarget.All);
           
            if (collision.gameObject.tag == "RedInside")
                view.RPC("RPC_OnYellowScored_Ball", RpcTarget.All);

        }


    }



    // Raise event seems to be better option 1.10.2022
    [PunRPC]
    public void RPC_OnRedScored_Ball()
    {
        RevertBallPos();
        gameStat.RedScore++;


        foreach (var player in gameController.Players)
        {

            player.GetComponent<PlayerMovment>().OnRedScoredP_Player();
        }

    }

    [PunRPC]
    public void RPC_OnYellowScored_Ball()
    {
        RevertBallPos();
        gameStat.YellowScore++;


        foreach (var player in gameController.Players)
        {

            player.GetComponent<PlayerMovment>().OnYellowScored_Player();
        }

    }


    public void RevertBallPos()
    {
        if (TryGetComponent<SpringJoint>(out SpringJoint _joint))
        {
            _joint.connectedBody = null;
        }
        transform.position = new Vector3(0, 6, 3);
    }


}
















