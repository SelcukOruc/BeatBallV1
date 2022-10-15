using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class CreateOrJoinRooms : MonoBehaviourPunCallbacks
{
    RoomOptions options = new RoomOptions();
  
    public TMP_InputField RoomNameInputField_Create;
    public TMP_InputField RoomNameInputField_Join;

    public TMP_InputField TimeLimitInputField;
    public TMP_InputField ScoreLimitInputField;

    public TMP_InputField NickNameInputField;

    public int TeamIndex = 0;

    public GameObject RoomMenu;
    public GameObject playerElement;
    public Transform redLayout, greenLayout;


    [SerializeField] CustomizeManager customizeManager;
    public void CreateRoom()
    {
        options.MaxPlayers = 4;
        //options.BroadcastPropsChangeToAll = true;
        Hashtable RoomCutomProps = new Hashtable();
        int _timeLimitInput = int.Parse(TimeLimitInputField.text);
        int _scoreLimitInput = int.Parse(ScoreLimitInputField.text);

        RoomCutomProps.Add("TIMELIMIT", _timeLimitInput);
        RoomCutomProps.Add("SCORELIMIT", _scoreLimitInput);


        options.CustomRoomProperties = RoomCutomProps;


        PhotonNetwork.LocalPlayer.NickName = NickNameInputField.text;
        PhotonNetwork.CreateRoom(RoomNameInputField_Create.text, options, TypedLobby.Default);
    }
   
    public void JoinRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInputField.text;
        PhotonNetwork.JoinRoom(RoomNameInputField_Join.text);

    }


    public override void OnJoinedRoom()
    {

        PhotonNetwork.LoadLevel("Game");

    }

    private void Start()
    {

        Hashtable PlayerCustomProp = new Hashtable();
        PlayerCustomProp.Add("HEADINDEX", 0);
        PlayerCustomProp.Add("EYEINDEX", 0);
        PhotonNetwork.LocalPlayer.CustomProperties = PlayerCustomProp;
    }

}


























