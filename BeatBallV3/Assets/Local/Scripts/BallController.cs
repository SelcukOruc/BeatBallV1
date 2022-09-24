using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody hipsRigid;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            Debug.Log("BALL ON ME");
            if (other.gameObject.TryGetComponent<SpringJoint>(out SpringJoint _joint))
            {
                Debug.Log("Joint exists.");
                // Owner touches.
                if (hipsRigid==_joint.connectedBody)
                {

                }
                // Other Player touches.
                else
                {
                    _joint.connectedBody = hipsRigid;
                }
           
            
            }
            else
            {
                Debug.Log("Placed.");
                _joint = other.gameObject.AddComponent<SpringJoint>();
                _joint.connectedBody = hipsRigid;
                _joint.autoConfigureConnectedAnchor = false;
                _joint.connectedAnchor = new Vector3(0, 0, 1);
                _joint.spring = 900;
            }
        
        }
    }
   

}
