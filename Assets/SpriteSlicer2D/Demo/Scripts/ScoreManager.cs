using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static float score;        // The player's score.
    public static float mainArea;
    public static float objArea;
    public static int count;

    Text text;
    int thislevel;
    void Awake()
    {
        // Set up the reference.
        text = GetComponent<Text>();

        thislevel = Application.loadedLevel;
        score = 0;
        if (thislevel == 4)
        {
            mainArea = 12.3556f;
        }
        mainArea = 12.3556f;
        count = 0;
    }


    void Update()
    {
        score = objArea / mainArea * 100;
        if (count < 1)
        {
            objArea = 0;
            score = 0;
        }
        // Set the displayed text to be the word "Score" followed by the score value.
        int scoreint = Mathf.RoundToInt(score);
        text.text = "Score: " + scoreint+"%";
        
    }
}