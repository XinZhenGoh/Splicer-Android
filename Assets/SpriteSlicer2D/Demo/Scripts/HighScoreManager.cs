using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoreManager : MonoBehaviour
{
    public static float highScore;
    public static float currentScore;
    int thislevel;
    Text text;
    // Use this for initialization
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        text = GetComponent<Text>();
        thislevel = Application.loadedLevel;
        if (thislevel == 2) { 
            //highScore1
        highScore = PlayerPrefs.GetFloat("highScore1");
        }
        if (thislevel == 3)
        {
            highScore = PlayerPrefs.GetFloat("highScore2");
        }
        if (thislevel == 4)
        {
            highScore = PlayerPrefs.GetFloat("highScore3");
        }
        if (thislevel == 5)
        {
            highScore = PlayerPrefs.GetFloat("highScore4");
        }
        if (thislevel == 6)
        {
            highScore = PlayerPrefs.GetFloat("highScore5");
        }
        if (thislevel == 7)
        {
            highScore = PlayerPrefs.GetFloat("highScore6");
        }
        if (thislevel == 8)
        {
            highScore = PlayerPrefs.GetFloat("highScore7");
        }
        if (thislevel == 9)
        {
            highScore = PlayerPrefs.GetFloat("highScore8");
        }
        if (thislevel == 10)
        {
            highScore = PlayerPrefs.GetFloat("highScore9");
        }
        if (thislevel == 11)
        {
            highScore = PlayerPrefs.GetFloat("highScore10");
            Debug.Log("high score is " + highScore);
        }
        if (thislevel == 12)
        {
            highScore = PlayerPrefs.GetFloat("highScore11");
        }
        if (thislevel == 13)
        {
            highScore = PlayerPrefs.GetFloat("highScore12");
        }
        if (thislevel == 14)
        {
            highScore = PlayerPrefs.GetFloat("highScore13");
        }
        if (thislevel == 15)
        {
            highScore = PlayerPrefs.GetFloat("highScore14");
        }
        if (thislevel == 16)
        {
            highScore = PlayerPrefs.GetFloat("highScore15");
        }
        if (thislevel == 17)
        {
            highScore = PlayerPrefs.GetFloat("highScore16");
        }
        if (thislevel == 18)
        {
            highScore = PlayerPrefs.GetFloat("highScore17");
        }
        if (thislevel == 19)
        {
            highScore = PlayerPrefs.GetFloat("highScore18");
        }
        if (thislevel == 20)
        {
            highScore = PlayerPrefs.GetFloat("highScore19");
        }
        if (thislevel == 21)
        {
            highScore = PlayerPrefs.GetFloat("highScore20");
        }
        if (thislevel == 22)
        {
            highScore = PlayerPrefs.GetFloat("highScore21");
        }
    }

    // Update is called once per frame
    void Update()
    {
        int highScoreint = Mathf.RoundToInt(highScore);
        text.text = "High Score: " + highScoreint+"%";
        int scoreint2 = Mathf.RoundToInt(ScoreManager.score);
        if (ScoreManager.count == 3 || scoreint2>=GoalManager.goal)
        {
            currentScore=ScoreManager.score;
            if (currentScore > highScore)
            {
                highScore = currentScore;
                if (thislevel == 2) { 
                PlayerPrefs.SetFloat("highScore1", highScore);
                }
                if(thislevel == 3) {
                    PlayerPrefs.SetFloat("highScore2", highScore);
                }
                if (thislevel == 4)
                {
                    PlayerPrefs.SetFloat("highScore3", highScore);
                }
                if (thislevel == 5)
                {
                    PlayerPrefs.SetFloat("highScore4", highScore);
                }
                if (thislevel == 6)
                {
                    PlayerPrefs.SetFloat("highScore5", highScore);
                }
                if (thislevel == 7)
                {
                    PlayerPrefs.SetFloat("highScore6", highScore);
                }
                if (thislevel == 8)
                {
                    PlayerPrefs.SetFloat("highScore7", highScore);
                }
                if (thislevel == 9)
                {
                    PlayerPrefs.SetFloat("highScore8", highScore);
                }
                if (thislevel == 10)
                {
                    PlayerPrefs.SetFloat("highScore9", highScore);
                }
                if (thislevel == 11)
                {
                    PlayerPrefs.SetFloat("highScore10", highScore);
                }
                if (thislevel == 12)
                {
                    PlayerPrefs.SetFloat("highScore11", highScore);
                }
                if (thislevel == 13)
                {
                    PlayerPrefs.SetFloat("highScore12", highScore);
                }
                if (thislevel == 14)
                {
                    PlayerPrefs.SetFloat("highScore13", highScore);
                }
                if (thislevel == 15)
                {
                    PlayerPrefs.SetFloat("highScore14", highScore);
                }
                if (thislevel == 16)
                {
                    PlayerPrefs.SetFloat("highScore15", highScore);
                }
                if (thislevel == 17)
                {
                    PlayerPrefs.SetFloat("highScore16", highScore);
                }
                if (thislevel == 18)
                {
                    PlayerPrefs.SetFloat("highScore17", highScore);
                }
                if (thislevel == 19)
                {
                    PlayerPrefs.SetFloat("highScore18", highScore);
                }
                if (thislevel == 20)
                {
                    PlayerPrefs.SetFloat("highScore19", highScore);
                }
                if (thislevel == 21)
                {
                    PlayerPrefs.SetFloat("highScore20", highScore);
                }
                if (thislevel == 22)
                {
                    PlayerPrefs.SetFloat("highScore21", highScore);
                }
                PlayerPrefs.Save();
            }
        }
    }
}
