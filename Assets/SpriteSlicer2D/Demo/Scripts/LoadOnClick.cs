using UnityEngine;
using System.Collections;

public class LoadOnClick : MonoBehaviour {


    public void LoadScene(int level)
    {
        Application.LoadLevel(level);
        SpriteSlicer2DDemoManager.sliceCount = 0;
        SpriteSlicer2DDemoManager.addCount = 0;
    }
}
