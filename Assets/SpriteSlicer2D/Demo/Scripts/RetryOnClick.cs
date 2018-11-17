using UnityEngine;
using System.Collections;

public class RetryOnClick : MonoBehaviour {

    public void LoadScene(int level)
    {
        Application.LoadLevel(Application.loadedLevel);
        ScoreManager.score = 0;
        ScoreManager.count = 0;
        SpriteSlicer2DDemoManager.sliceCount = 0;
        SpriteSlicer2DDemoManager.addCount = 0;
        nextLevelButton.played = 0;
        nextLevelButton.retryShowCount = 3;
    }
}
