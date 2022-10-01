using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BallController : MonoBehaviourPun 
{
    [SerializeField] private Rigidbody hipsRigid;
    [SerializeField] private PhotonView playerPhotonView;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            PhotonView _ballPhotonview = other.gameObject.GetComponent<PhotonView>();
            _ballPhotonview.TransferOwnership(playerPhotonView.Owner);

            // there is spring joint.
            if (other.gameObject.TryGetComponent<SpringJoint>(out SpringJoint _joint))
            {
                Debug.Log("Joint exists.");
                // Owner touches.
                if (hipsRigid == _joint.connectedBody)
                {
                    _joint.connectedBody = hipsRigid;

                }
                // Other Player touches.
                else
                {
                    _joint.connectedBody = hipsRigid;

                }


            }
            // there is no spring joint.
            else
            {
                Debug.Log("Placed.");

                _joint = other.gameObject.AddComponent<SpringJoint>();
                _joint.connectedBody = hipsRigid;
                _joint.autoConfigureConnectedAnchor = false;
                _joint.connectedAnchor = new Vector3(0, 0, 1);
                _joint.spring = 1800;

            }


        }


    }




}
   
    
    
    
    
    


