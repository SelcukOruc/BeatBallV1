using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

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
    public Vector3 BallInsPos;
    
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private AudioSource finishSFX,scoreSFX;
    private void Start()
    {
        scoreLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["SCORELIMIT"];
        timeLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["TIMELIMIT"];

        timeLimitText.text ="Time Limit : " + timeLimit.ToString();
        scoreLimitText.text = "Score Limit : " + scoreLimit.ToString();
        
        StartCoroutine(Co_timer());
    }

    [PunRPC]
    public void OnRedTeamScored()
    {


        RedTeamScore++;
        OnScored();

    }




    [PunRPC]
    public void OnGreenTeamScored()
    {

        GreenTeamScore++;
        OnScored();

    }









    void OnScored()
    {
        scoreSFX.Play();
        SetScoreText();
        OnReachedScoreLimit();
        foreach (var player in PhotonManager.ins.Players)
        {
            player.GetComponent<PlayerMovment>().TeleportToPos();
        }

        Ball.ins.RevertBallPos();
       
       
      
    }


    void SetScoreText()
    {
        redTeamScoreText.text = RedTeamScore.ToString();
        greenTeamScoreText.text = GreenTeamScore.ToString();
    
    }

    public void OnReachedScoreLimit()
    {
        int _totalScore = RedTeamScore + GreenTeamScore;
        int _scoreLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["SCORELIMIT"];

        int _redScore = RedTeamScore;
        int _greenScore = GreenTeamScore;

        if ( _redScore == _scoreLimit || _greenScore == _scoreLimit || minute == (int)PhotonNetwork.CurrentRoom.CustomProperties["TIMELIMIT"])
        {
            PhotonManager.ins.HasGameBegun = false;

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            finishPanel.SetActive(true);
            TextMeshProUGUI _panelText = finishPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            
            finishSFX.Play();

            if (_greenScore > _redScore)
            {
                 _panelText.text = "Green Team Won !";
                 _panelText.color = Color.green;

            }
            if (_redScore > _greenScore)
            {
                _panelText.text = "Red Team Won !";
                _panelText.color = Color.red;
            }
            if (_redScore == _greenScore)
            {
                _panelText.text = "Teams Are Equal !";
                _panelText.color = Color.gray;

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
