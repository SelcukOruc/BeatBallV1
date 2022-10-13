using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class PlayerStat : MonoBehaviourPun
{
   
    // Movement variables
    public float MoveSpeed = 30000;
    public float JumpForce = 300000;

    // Stamina
    [SerializeField] private Image staminaBar;
    [HideInInspector] public float staminaStartValue;
    [SerializeField] private float stamina = 300;
    public float Stamina
    {
        set
        {


            stamina = value;
            staminaBar.fillAmount = stamina / staminaStartValue;




        }

        get
        {
            return stamina;
        }
    }

    // HitForce
    [SerializeField] private Image hitForceBar;
    private float hitForce;
    float hitForceStartValue; // Maximum force reachable.
    public float HitForceLimit; //40
    public float HitForce
    {
        get
        {
            return hitForce;
        }
        set
        {
            hitForce = value;
            hitForceBar.fillAmount = hitForce / hitForceStartValue;

        }

    }

   

    [SerializeField] private TextMeshProUGUI m_nickName;
    public bool IsRedTeamMember;
   
    private void Awake()
    {
        staminaStartValue = stamina;
        hitForceStartValue = HitForceLimit;

        base.photonView.RPC("UpdateName", RpcTarget.All);
        
       
    }
    [PunRPC]
    public void UpdateName()
    {
        this.gameObject.name = base.photonView.Owner.ActorNumber.ToString();
        m_nickName.text = base.photonView.Owner.NickName;
    }

    [PunRPC]
    public void ChangeTeam(bool _isRedTeamMember)
    {
        IsRedTeamMember = _isRedTeamMember;
    }

}
