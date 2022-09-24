using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class PlayerStat : MonoBehaviour
{
    public PhotonView view;
    public bool IsPlayerInYellowTeam;
    [SerializeField] private TextMeshProUGUI NickNameText;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        view.RPC("UpdateName", RpcTarget.All);
    }

    [PunRPC]
    public void UpdateName()
    {
        gameObject.name = view.Controller.NickName;
        NickNameText.text = view.Controller.NickName;
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
