using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
    [SerializeField] private GameObject playerListingElementPrefab;
    [SerializeField] private Transform RedList, GreenList;

    // 1- find players, 2- organize list.
    public void OrganizePlayerList()
    {
        foreach (Transform child in RedList)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in GreenList)
        {
            Destroy(child.gameObject);
        }

        foreach (var redplayer in PhotonManager.ins.RedTeamPlayers)
        {
            GameObject listElement = Instantiate(playerListingElementPrefab, RedList);
            listElement.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = redplayer.GetComponent<PhotonView>().Owner.NickName;
            listElement.GetComponent<Image>().color = Color.red;
        }

        foreach (var greenplayer in PhotonManager.ins.GreenTeamPlayers)
        {
            GameObject listElement = Instantiate(playerListingElementPrefab, GreenList);
            listElement.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = greenplayer.GetComponent<PhotonView>().Owner.NickName;
            listElement.GetComponent<Image>().color = Color.green;
        }


    }

}
