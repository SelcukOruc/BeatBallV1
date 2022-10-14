using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager ins { get; private set; }
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

    public bool CanPlayerFunction = true;

    int scoreLimit,timeLimit;
    private float second,minute;
    [SerializeField] private TextMeshProUGUI timeText,timeLimitText;

    public int RedTeamScore, GreenTeamScore;
    [SerializeField] private TextMeshProUGUI redTeamScoreText, greenTeamScoreText,scoreLimitText;

    public Vector3 RedTeamPos, GreenTeamPos,StartPos;
    public Vector3 BallPos;

    private void Start()
    {
        scoreLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["SCORELIMIT"];
        timeLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["TIMELIMIT"];

        timeLimitText.text ="Time Limit : " + timeLimit.ToString();
        scoreLimitText.text = "Score Limit : " + scoreLimit.ToString();
        
        StartCoroutine(Co_timer());
    }


    public void OnRedTeamScored()
    {
        RedTeamScore++;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "REDTEAMSCORE", RedTeamScore } });

    }





    public void OnGreenTeamScored()
    {
        GreenTeamScore++;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "GREENTEAMSCORE", GreenTeamScore } });

    }




    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
       
        // ON SCORED.
        if(propertiesThatChanged.ContainsKey("GREENTEAMSCORE") || propertiesThatChanged.ContainsKey("REDTEAMSCORE"))
        {
            
           
            foreach (var player in PhotonManager.ins.Players)
            {
                player.GetComponent<PlayerMovment>().TeleportToPos();
            }
            
            Ball.ins.RevertBallPos();
            Debug.Log("scored.");
            OnReachedScoreLimit();
            SetScoreText();
        }
    
    }


    void SetScoreText()
    {
        redTeamScoreText.text = PhotonNetwork.CurrentRoom.CustomProperties["REDTEAMSCORE"].ToString();
        greenTeamScoreText.text = PhotonNetwork.CurrentRoom.CustomProperties["GREENTEAMSCORE"].ToString();
    
    }

    public void OnReachedScoreLimit()
    {
        int _totalScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["REDTEAMSCORE"] + (int)PhotonNetwork.CurrentRoom.CustomProperties["GREENTEAMSCORE"];
        int _scoreLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["SCORELIMIT"];

        int _redTeamScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["REDTEAMSCORE"];
        int _greenTeamScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["GREENTEAMSCORE"];

        if ( _totalScore == _scoreLimit || minute == (int)PhotonNetwork.CurrentRoom.CustomProperties["TIMELIMIT"])
        {
            PhotonManager.ins.HasGameBegun = false;
            

         
            
                if (_greenTeamScore > _redTeamScore)
                {
                    Debug.Log("Green Team Won");

                }
                if (_redTeamScore > _greenTeamScore)
                {
                    Debug.Log("Red Team Won");
                }
                if (_greenTeamScore == _redTeamScore)
                {
                    Debug.Log("Teams Are Equal.");

                }


            //foreach (var player in PhotonManager.ins.Players)
            //{
            //    player.SetActive(false);
            //}
           

        }
       

    }

    IEnumerator Co_timer()
    {
        while (true)
        {
            if (PhotonManager.ins.HasGameBegun)
            {
                yield return new WaitForSeconds(1);
                second++;

                if (second == 60)
                {
                    minute++;
                    second = 0;

                }


                OnReachedScoreLimit();
                timeText.text = string.Format("{0:00} : {1:00}", minute, second);

            }

            yield return null;

        }

    }









}
