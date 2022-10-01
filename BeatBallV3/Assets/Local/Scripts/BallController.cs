using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BallController : MonoBehaviourPun 
{
    [SerializeField] private Rigidbody hipsRigid;
    [SerializeField] private PhotonView playerPhotonView;

    // We managed to take over and sync ownership steadily.
    // next step is to sync connected rigidbody (spring joint) in intense collision scenarios.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            
            PhotonView _ballPhotonview = other.gameObject.GetComponent<PhotonView>();
            _ballPhotonview.TransferOwnership(playerPhotonView.Owner);

        }





    }




}
   
    
    
    
    
    


