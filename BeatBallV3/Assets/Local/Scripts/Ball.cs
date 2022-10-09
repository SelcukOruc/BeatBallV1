using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Ball : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    [SerializeField] private PhotonView view;
    [SerializeField] private PhotonInGameController gameController;
    [SerializeField] private GameStat gameStat;

   public Transform m_Controller;
   

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

        if (collision.gameObject.tag == "Player" && m_Controller==null)
        {
            // OWNERSHIP 
            PhotonView _playerPhotonview = collision.gameObject.GetComponentInParent<PhotonView>();   
            if (view.Owner != _playerPhotonview.Owner)
                view.TransferOwnership(_playerPhotonview.Owner);
           
            BallController _ballController= collision.gameObject.GetComponent<BallController>();

            m_Controller = _ballController.BallPos;

     
         
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

        transform.position = new Vector3(0, 6, 3);

    }


    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != base.photonView)
            return;
       
        
        base.photonView.TransferOwnership(requestingPlayer);
   
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != base.photonView)
            return;
        
       
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        throw new System.NotImplementedException();
    }


    private void Update()
    {
        if (m_Controller != null)
        {
            this.transform.position = m_Controller.position;
        }


    }



}
















