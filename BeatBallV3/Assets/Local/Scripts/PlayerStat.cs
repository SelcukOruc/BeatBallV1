using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
public class PlayerStat : MonoBehaviour
{
    public PhotonView view;
    public bool IsPlayerInYellowTeam;
    [SerializeField] private TextMeshProUGUI nickNameText;

    // Movement variables
    public float MoveSpeed = 30000;
    public float jumpForce = 300000;

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

    
    private void Awake()
    {
        view = GetComponent<PhotonView>();
        view.RPC("UpdateName", RpcTarget.All);
        
        staminaStartValue = stamina;
        hitForceStartValue = HitForceLimit;
        

    }

    [PunRPC]
    public void UpdateName()
    {
        gameObject.name = view.Controller.NickName;
        nickNameText.text = view.Controller.NickName;
    }

    // Methods for team selection in the begining.
    [PunRPC]
    public void TeamYellow()
    {
        IsPlayerInYellowTeam = true;
    }
    [PunRPC]
    public void TeamRed()
    {
       IsPlayerInYellowTeam = false;
    }


}
