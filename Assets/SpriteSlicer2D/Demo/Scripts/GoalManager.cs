using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour {
    Text text;
    public static int goal;
    int thislevel;
    // Use this for initialization
    void Start () {
        thislevel = Application.loadedLevel;
        text = GetComponent<Text>();
        if(thislevel == 2)
        {
            goal = 90;
        }
        if (thislevel == 3)
        {
            goal = 80;
        }
        if (thislevel == 4)
        {
            goal = 60;
        }
        if (thislevel == 5)
        {
            goal = 100;
        }
        if (thislevel == 6)
        {
            goal = 90;
        }
        if (thislevel == 7)
        {
            goal = 100;
        }
        if (thislevel == 8)
        {
            goal = 100;
        }
        if (thislevel == 9)
        {
            goal = 100;
        }
        if (thislevel == 10)
        {
            goal = 100;
        }
        if (thislevel == 11)
        {
            goal = 80;
        }
        if (thislevel == 12)
        {
            goal = 100;
        }
        if (thislevel == 13)
        {
            goal = 100;
        }
        if (thislevel == 14)
        {
            goal = 100;
        }
        if (thislevel == 15)
        {
            goal = 100;
        }
        if (thislevel == 16)
        {
            goal = 100;
        }
        if (thislevel == 17)
        {
            goal = 100;
        }
        if (thislevel == 18)
        {
            goal = 100;
        }
        if (thislevel == 19)
        {
            goal = 100;
        }
        if (thislevel == 20)
        {
            goal = 100;
        }
        if (thislevel == 21)
        {
            goal = 100;
        }
        if (thislevel == 22)
        {
            goal = 100;
        }


    }

    // Update is called once per frame
    void Update () {
        text.text = "goal: " + goal + "%";

    }
}
