using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStat : MonoBehaviour
{
  
   
    private int yellowScore;

    public int YellowScore
    {
        get
        {
            return yellowScore;
        }
        set
        {
            yellowScore = value;
            yellowScoreText.text = yellowScore.ToString();

      
        }
    
    }


    private int redScore;

    public int RedScore
    {
        get
        {
            return redScore;
        }
        set
        {
            redScore = value;
            redScoreText.text = redScore.ToString();

          
            
        }

    }

    [SerializeField] private TextMeshProUGUI yellowScoreText;
    [SerializeField] private TextMeshProUGUI redScoreText;

  

   



}
