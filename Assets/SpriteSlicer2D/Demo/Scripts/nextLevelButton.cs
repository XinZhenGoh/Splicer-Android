using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class nextLevelButton : MonoBehaviour {
    GameObject button;
    public static Animator anim;
    public static float highScore;
    public static int retryShowCount = 3;
    public static int played = 0;
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        button = GameObject.Find("nextLevelButton");
    }
	
	// Update is called once per frame
	void Update () {
        int scoreint1 = Mathf.RoundToInt(ScoreManager.score);
        int highScore1 = Mathf.RoundToInt(HighScoreManager.highScore);
        if (highScore1 >= GoalManager.goal)
            {
                button.GetComponent<Button>().interactable = true;
            }
        if (scoreint1 >= GoalManager.goal)
        {
            button.GetComponent<Button>().interactable = true;
        }

        if (scoreint1 >= GoalManager.goal)
        {
            if (played == 0) { 
            GetComponent<AudioSource>().Play();
            played += 1;
        }
            anim.SetTrigger("GameOver");
        }
        if (SpriteSlicer2DDemoManager.addCount>retryShowCount)
        {
            this.GetComponent<Animation>().Play("retryanimation");
            retryShowCount += 1;
        }

    }
}
