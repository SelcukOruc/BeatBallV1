using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class BallController : MonoBehaviour
{

    [SerializeField] private Rigidbody hipsRigid;
    [SerializeField] private PhotonView playerPhotonView;
    public Collider mycol;
    [SerializeField] private PlayerMovment playerMovment;
    public GameObject HasBallImage;
    private void Start()
    {
        mycol = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            Debug.Log("BALL ON ME");
            if (other.gameObject.TryGetComponent<SpringJoint>(out SpringJoint _joint))
            {
               
               // the problem as i suppouse that because of the fact that i tried to make other players take control of the ball
               // both when they touch it and hit it. Therefore, i couldn't convey breakeforce value (in PlayerMovment Script) of Spring joint
               // even if i was able convey connectedbody of springJoint via here. && collisions don't sync well (!)


            }
            else
            {
                // We take over ownership
                PhotonView _ballPhotonview = other.gameObject.GetComponent<PhotonView>();
                playerMovment.Ball = other.gameObject;


                if (_ballPhotonview.Owner != playerPhotonView.Owner)
                    _ballPhotonview.TransferOwnership(playerPhotonView.Owner);
                
                this.HasBallImage.SetActive(true);
                Debug.Log("Placed.");
                
                // Collider is set off
                mycol.enabled = false;
                
                // Joint settings
                _joint = other.gameObject.AddComponent<SpringJoint>();
                _joint.connectedBody = hipsRigid;
                _joint.autoConfigureConnectedAnchor = false;
                _joint.connectedAnchor = new Vector3(0, 0, 1);
                _joint.spring = 900;

            }


        }

    }





}




