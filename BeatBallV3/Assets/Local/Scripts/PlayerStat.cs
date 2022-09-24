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

    [SerializeField] private Image staminaBar;
    float staminaStartValue;
    private float stamina = 99;
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


    private void Awake()
    {
        view = GetComponent<PhotonView>();
        view.RPC("UpdateName", RpcTarget.All);
        
        staminaStartValue = stamina;
    
    }

    [PunRPC]
    public void UpdateName()
    {
        gameObject.name = view.Controller.NickName;
        nickNameText.text = view.Controller.NickName;
    }


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
