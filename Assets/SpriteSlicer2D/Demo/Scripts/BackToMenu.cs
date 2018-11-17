using UnityEngine;
using System.Collections;

public class BackToMenu : MonoBehaviour
{

    public void LoadScene(int level)
    {
        Application.LoadLevel(level);

    }
}
