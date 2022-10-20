using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefences : MonoBehaviourPun
{
    [SerializeField] private Transform headItemsParent;
    [HideInInspector]  public int HeadItemIndex;

    [SerializeField] private Transform eyeItemsParent;
    [HideInInspector] public int EyeItemIndex;
    public void SetItems()
    {
        HeadItemIndex = 0;
        EyeItemIndex = 0;

        if (base.photonView.IsMine)
        {
            // Set Index.
            HeadItemIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["HEADINDEX"];
            EyeItemIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["EYEINDEX"];

            // Activate Index Item.
            headItemsParent.GetChild(HeadItemIndex).gameObject.SetActive(true);
            eyeItemsParent.GetChild(EyeItemIndex).gameObject.SetActive(true);
        }

        foreach (var player in PhotonManager.ins.Players)
        {
            player.GetComponent<PhotonView>().RPC("UpdateItems", RpcTarget.AllBufferedViaServer,
                player.GetComponent<PlayerPrefences>().HeadItemIndex,
                player.GetComponent<PlayerPrefences>().EyeItemIndex);
        
        }
   
    }
    
    [PunRPC]
    public void UpdateItems(int _headitemindex,int _eyeitemindex)
    {
        headItemsParent.GetChild(_headitemindex).gameObject.SetActive(true);
        eyeItemsParent.GetChild(_eyeitemindex).gameObject.SetActive(true);
    }
}
