using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TargetRotationSerializer : MonoBehaviour, IPunObservable
{
    public ConfigurableJoint hipscj;
    GameObject ball;
    private void Start()
    {
        ball = GameObject.Find("Ball");
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // try to send ball's connectedbody info via here. 26.09.2022
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
