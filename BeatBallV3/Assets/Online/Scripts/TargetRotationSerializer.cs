using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TargetRotationSerializer : MonoBehaviour, IPunObservable
{
    public ConfigurableJoint hipscj;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hipscj.targetRotation);
         
         
        }
        else
        {
            hipscj.targetRotation = (Quaternion)stream.ReceiveNext();
           
        }
    }
}
