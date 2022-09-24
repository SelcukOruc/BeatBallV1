using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
public class PhotonInGameController : MonoBehaviourPunCallbacks
{
    public List<GameObject> Players = new List<GameObject>();

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
        _player.transform.GetChild(0).gameObject.SetActive(true);
        _player.transform.GetChild(1).gameObject.SetActive(true);
       
        
     
    }

   


    #region StartTheGame
   
    [PunRPC]
    public void FindPlayersInRoom()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {

            GameObject _playerFound = GameObject.Find(player.NickName);
            Players.Add(_playerFound);
            Debug.LogError(_playerFound);

        }


    }



    [PunRPC]
    public void MovePlayersToPos()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].GetComponent<PlayerStat>().IsPlayerInYellowTeam)
                Players[i].GetComponent<PhotonView>().RPC("TeleportToFieldYellow", RpcTarget.All);
            else
                Players[i].GetComponent<PhotonView>().RPC("TeleportToFieldRed", RpcTarget.All);

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
        HasGameBegun = true; Debug.Log("GameBegun!");

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate(ball.name, new Vector3(0, 6, 3), Quaternion.identity);
    }

    #endregion






}
