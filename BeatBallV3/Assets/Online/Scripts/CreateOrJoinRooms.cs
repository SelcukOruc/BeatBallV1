using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class CreateOrJoinRooms : MonoBehaviourPunCallbacks
{
    RoomOptions options = new RoomOptions();
  
    public TMP_InputField RoomNameInputField;
    public TMP_InputField NickNameInputField;

    public void CreateOrJoinRoom()
    {

        options.MaxPlayers = 4;
        PhotonNetwork.LocalPlayer.NickName = NickNameInputField.text;
        PhotonNetwork.JoinOrCreateRoom(RoomNameInputField.text, options, TypedLobby.Default);

    }



    public override void OnJoinedRoom()
    {


        PhotonNetwork.LoadLevel("Game");

    }

}
