using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;


public class PhotonInGameController : MonoBehaviourPunCallbacks
{
    public List<GameObject> Players = new List<GameObject>();
    
    public List<GameObject> RedTeamPlayers = new List<GameObject>();
    public List<GameObject> YellowTeamPlayers = new List<GameObject>();

    public LayerMask RedTeamLayer;
    public LayerMask YellowTeamLayer;

    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private Transform SpawnPos;
    [SerializeField] private TextMeshProUGUI StartButton;
    public bool HasGameBegun = false;
    [SerializeField] private GameObject ball;

    PhotonView view;
    private void Awake()
    {
        view = GetComponent<PhotonView>();

        
        GameObject _player=  PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPos.position, Quaternion.identity);
        // Finding Childs with index is not recommended and not efficient, i will change this approach when i revise project. 24/09/2022.
        _player.transform.GetChild(0).gameObject.SetActive(true);
        _player.transform.GetChild(1).gameObject.SetActive(true);
        _player.transform.GetChild(2).gameObject.SetActive(true);
       
        
     
    }




    #region StartTheGame
    /// <summary>
        // Sequence IS...
             // Find players in the room,
                // Move them to their places,
                     // Start the game.[This method called inside PlayerMovment,when Master presses down 'X']
                        // (inside this method set game as begun)
    /// </summary>



    // I use this method when game beguns and when a player leaves the room.
    // When sth is meant to be applied to all players i intend to find players via this method.
    [PunRPC]
    public void FindPlayersInRoom()
    {


        Players.Clear();
        YellowTeamPlayers.Clear();
        RedTeamPlayers.Clear();

        foreach (var player in PhotonNetwork.PlayerList)
        {
          

            GameObject _playerFound = GameObject.Find(player.NickName);
            Players.Add(_playerFound);

            if (_playerFound.GetComponent<PlayerStat>().IsPlayerInYellowTeam)
                YellowTeamPlayers.Add(_playerFound);
            else
                RedTeamPlayers.Add(_playerFound);
          


       
        }


    }

   


    [PunRPC]
    public void MovePlayersToPos()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].GetComponent<PlayerStat>().IsPlayerInYellowTeam)
                Players[i].GetComponent<PhotonView>().RPC("Initial_TeleportToFieldYellow", RpcTarget.All); // This Rpc is in PlayerMovmentScript.
            else
                Players[i].GetComponent<PhotonView>().RPC("Initial_TeleportToFieldRed", RpcTarget.All);

        }
    }

    public void StartTheGame()
    {
        view.RPC("FindPlayersInRoom", RpcTarget.All);
        MovePlayersToPos();

        view.RPC("GameBegun", RpcTarget.All);
    }


    [PunRPC]
    void GameBegun()
    {
        HasGameBegun = true; 

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.InstantiateRoomObject(ball.name, new Vector3(0, 6, 3), Quaternion.identity);
    }

    #endregion






}
