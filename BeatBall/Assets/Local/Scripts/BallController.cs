using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BallController : MonoBehaviour
{
    public Transform m_BallPos;
    Collider m_Collider;
    PhotonView m_View;
    public bool HasLostBall = false;
    private void Start()
    {
        m_Collider = GetComponent<Collider>();
        m_View = GetComponent<PhotonView>();
        StartCoroutine(Co_ActivateCollider());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ball" && Ball.ins.BallPos == null)
        {
            // Make it appear on network.
            m_View.RPC("RPC_GetBall", RpcTarget.AllViaServer);
        }        
    
    
    
    }
    [PunRPC]
    public void RPC_GetBall()
    {
        if(Ball.ins.m_View.Owner!=m_View.Owner)
        Ball.ins.m_View.RequestOwnership();
       
        Ball.ins.BallPos = m_BallPos;
        m_Collider.enabled = false;
    



    }
    [PunRPC]
    public void RPC_HitBall()
    {
        if (Ball.ins.m_View.Owner != m_View.Owner)
            Ball.ins.m_View.RequestOwnership();
       
        
        Ball.ins.BallPos = null;
        this.HasLostBall = true;
    }
    [PunRPC]
    public void ActivateCollider()
    {
        m_Collider.enabled = true;
       
    }

    IEnumerator Co_ActivateCollider()
    {
        while (true)
        {
            if (!m_Collider.enabled && Ball.ins.BallPos != m_BallPos)
            {
                yield return new WaitForSeconds(2);
               
                m_View.RPC("ActivateCollider", RpcTarget.AllViaServer);
               
            }

            yield return null;
        }
    }

    // in 2 secs. ins.ControllerCollider can be changed.


}
