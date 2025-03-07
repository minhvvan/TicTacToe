using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
     [SerializeField] TMP_Text userNameText;
     [SerializeField] TMP_Text scoreText;
     [SerializeField] TMP_Text rankText;
     [SerializeField] Image backgroundImage;
     [SerializeField] List<Color> colors;

     public int rank;
     
     public void SetUsername(string username)
     {
          userNameText.text = username;
     }
     
     public void SetScore(int score)
     {
          scoreText.text = score.ToString();
     }
     
     public void SetRank(int newRank)
     {
          rank = newRank;
          rankText.text = newRank.ToString();
     }

     public void SetColor(bool bOwn)
     {
          var color = bOwn ? colors[0] : colors[1];
          backgroundImage.color = color;
     }
}
