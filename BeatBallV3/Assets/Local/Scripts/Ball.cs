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
                view.RPC("OnRedScored", RpcTarget.All);
           
            if (collision.gameObject.tag == "RedInside")
                view.RPC("OnYellowScored", RpcTarget.All);

        }


    }







 




    [PunRPC]
    public void OnRedScored()
    {
        RevertBallPos();
        gameStat.RedScore++;


        foreach (var player in gameController.Players)
        {
            player.GetComponent<PhotonView>().RPC("OnRedScoredP", RpcTarget.All);
        }

    }
    
    [PunRPC]
    public void OnYellowScored()
    {
        RevertBallPos();
        gameStat.YellowScore++;


        foreach (var player in gameController.Players)
        {
            player.GetComponent<PhotonView>().RPC("OnYellowScoredP", RpcTarget.All);
        }

    }

    public void RevertBallPos()
    {
        if(TryGetComponent<SpringJoint>(out SpringJoint _joint))
        {
            _joint.connectedBody = null;
        }
        transform.position = new Vector3(0, 6, 3);
    }

   








}
