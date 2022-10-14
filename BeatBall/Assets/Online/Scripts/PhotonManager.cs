using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{

    public static PhotonManager ins { get; private set; }
    private void Awake()
    {

        if (ins != null && ins != this)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }

    }



   
    public bool HasGameBegun = false;

    [SerializeField] private GameObject redPlayerPrefab;
    [SerializeField] private GameObject ball;

    public List<GameObject> Players,RedTeamPlayers,GreenTeamPlayers = new List<GameObject>();
    [SerializeField] private PlayerListing playerList;
    void Start()
    {
     
        
            GameObject player = PhotonNetwork.Instantiate(redPlayerPrefab.name, GameManager.ins.StartPos, Quaternion.identity);
            player.transform.GetChild(0).gameObject.SetActive(true);
            player.transform.GetChild(1).gameObject.SetActive(true);
            player.transform.GetChild(2).gameObject.SetActive(true);
        
       
        
        
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.InstantiateRoomObject(ball.name, GameManager.ins.BallInsPos, Quaternion.identity);

      
        
    }
    
    
    [PunRPC]
    public void FindPlayers()
    {
        Players.Clear();
        RedTeamPlayers.Clear();
        GreenTeamPlayers.Clear();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject _playerFound = GameObject.Find(player.ActorNumber.ToString());
            Players.Add(_playerFound);

            if (_playerFound.GetComponent<PlayerStat>().IsRedTeamMember)
                RedTeamPlayers.Add(_playerFound);
            else
                GreenTeamPlayers.Add(_playerFound);

        }
        
       
    }
    public void MoveToPostions()
    {
        foreach (var player in Players)
        {
            PlayerMovment _playerMovment = player.GetComponent<PlayerMovment>();
            _playerMovment.TeleportToPos();

        }
    }
    [PunRPC]
    public void StartGame()
    {
        base.photonView.RPC("FindPlayers", RpcTarget.All);
        playerList.OrganizePlayerList();
        MoveToPostions();
        HasGameBegun = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && PhotonNetwork.IsMasterClient && !HasGameBegun )
        {
            base.photonView.RPC("StartGame", RpcTarget.All);
        }


    }

   
    
  
    
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.photonView.RPC("FindPlayers", RpcTarget.All);
        playerList.OrganizePlayerList();
    }

}
