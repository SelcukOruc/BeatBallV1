using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Ball : MonoBehaviourPun, IPunOwnershipCallbacks 
{
    public static Ball ins { get; private set; }
    private void Awake()
    {

        if (ins != null && ins != this)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }

    }

    public Transform BallPos;
    public PhotonView m_View;
    



    private void Update()
    {
        if (BallPos != null)
        {
            float y = Mathf.Clamp(BallPos.position.y, 0.5f, BallPos.position.y);
            transform.position = new Vector3(BallPos.position.x, y , BallPos.position.z);
        }
           
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != base.photonView)
            return;

        m_View.TransferOwnership(requestingPlayer);
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

    public void RevertBallPos()
    {
        transform.position = Vector3.zero;
      
    }

  
}
