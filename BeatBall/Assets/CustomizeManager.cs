using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] headItems;
    public int HeadItemIndex = 0;


    public void ChangeHeadItem()
    {

        if (HeadItemIndex < headItems.Length - 1)
        {
            HeadItemIndex++;
            headItems[HeadItemIndex].SetActive(true);

            if (HeadItemIndex > 0)
                headItems[HeadItemIndex - 1].SetActive(false);
        }
        else
        {
            HeadItemIndex = 0;
            headItems[headItems.Length - 1].SetActive(false);
            headItems[0].SetActive(true);

        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "HEADINDEX", HeadItemIndex } });


    }

    [SerializeField] private GameObject[] eyeItems;
    public int EyeItemIndex = 0;


    public void ChangeEyeItem()
    {

        if (EyeItemIndex < eyeItems.Length - 1)
        {
            EyeItemIndex++;
            eyeItems[EyeItemIndex].SetActive(true);

            if (EyeItemIndex > 0)
                eyeItems[EyeItemIndex - 1].SetActive(false);
        }
        else
        {
            EyeItemIndex = 0;
            eyeItems[eyeItems.Length - 1].SetActive(false);
            eyeItems[0].SetActive(true);

        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "EYEINDEX", EyeItemIndex } });


    }




}
