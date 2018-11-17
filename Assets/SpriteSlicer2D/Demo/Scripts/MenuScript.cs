using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MenuScript : MonoBehaviour {
    public static float highScore1;
    public static float highScore2;
    public static float highScore3;
    public static float highScore4;
    public static float highScore5;
    public static float highScore6;
    public static float highScore7;
    public static float highScore8;
    public static float highScore9;
    public static float highScore10;
    public static float highScore11;
    public static float highScore12;
    public static float highScore13;
    public static float highScore14;
    public static float highScore15;
    public static float highScore16;
    public static float highScore17;
    public static float highScore18;
    public static float highScore19;
    public static float highScore20;
    public static float highScore21;
    public static int highScoreint1;
    public static int highScoreint2;
    public static int highScoreint3;
    public static int highScoreint4;
    public static int highScoreint5;
    public static int highScoreint6;
    public static int highScoreint7;
    public static int highScoreint8;
    public static int highScoreint9;
    public static int highScoreint10;
    public static int highScoreint11;
    public static int highScoreint12;
    public static int highScoreint13;
    public static int highScoreint14;
    public static int highScoreint15;
    public static int highScoreint16;
    public static int highScoreint17;
    public static int highScoreint18;
    public static int highScoreint19;
    public static int highScoreint20;
    public static int highScoreint21;
    GameObject button2;
    GameObject button3;
    GameObject button4;
    GameObject button5;
    GameObject button6;
    GameObject button7;
    GameObject button8;
    GameObject button9;
    GameObject button10;
    GameObject button11;
    GameObject button12;
    GameObject button13;
    GameObject button14;
    GameObject button15;
    GameObject button16;
    GameObject button17;
    GameObject button18;
    GameObject button19;
    GameObject button20;
    GameObject button21;
    // Use this for initialization
    void Start () {
        button2 = GameObject.Find("level2");
        button3 = GameObject.Find("level3");
        button4 = GameObject.Find("level4");
        button5 = GameObject.Find("level5");
        button6 = GameObject.Find("level6");
        button7 = GameObject.Find("level7");
        button8 = GameObject.Find("level8");
        button9 = GameObject.Find("level9");
        button10 = GameObject.Find("level10");
        button11 = GameObject.Find("level11");
        button12 = GameObject.Find("level12");
        button13 = GameObject.Find("level13");
        button14 = GameObject.Find("level14");
        button15 = GameObject.Find("level15");
        button16 = GameObject.Find("level16");
        button17 = GameObject.Find("level17");
        button18 = GameObject.Find("level18");
        button19 = GameObject.Find("level19");
        button20 = GameObject.Find("level20");
        button21 = GameObject.Find("level21");
        highScore1 = PlayerPrefs.GetFloat("highScore1");
        highScore2 = PlayerPrefs.GetFloat("highScore2");
        highScore3 = PlayerPrefs.GetFloat("highScore3");
        highScore4 = PlayerPrefs.GetFloat("highScore4");
        highScore5 = PlayerPrefs.GetFloat("highScore5");
        highScore6 = PlayerPrefs.GetFloat("highScore6");
        highScore7 = PlayerPrefs.GetFloat("highScore7");
        highScore8 = PlayerPrefs.GetFloat("highScore8");
        highScore9 = PlayerPrefs.GetFloat("highScore9");
        highScore10 = PlayerPrefs.GetFloat("highScore10");
        highScore11= PlayerPrefs.GetFloat("highScore11");
        highScore12 = PlayerPrefs.GetFloat("highScore12");
        highScore13 = PlayerPrefs.GetFloat("highScore13");
        highScore14 = PlayerPrefs.GetFloat("highScore14");
        highScore15 = PlayerPrefs.GetFloat("highScore15");
        highScore16 = PlayerPrefs.GetFloat("highScore16");
        highScore17 = PlayerPrefs.GetFloat("highScore17");
        highScore18 = PlayerPrefs.GetFloat("highScore18");
        highScore19 = PlayerPrefs.GetFloat("highScore19");
        highScore20 = PlayerPrefs.GetFloat("highScore20");
        highScore21 = PlayerPrefs.GetFloat("highScore21");
        highScoreint1 = Mathf.RoundToInt(highScore1);
        highScoreint2 = Mathf.RoundToInt(highScore2);
        highScoreint3 = Mathf.RoundToInt(highScore3);
        highScoreint4 = Mathf.RoundToInt(highScore4);
        highScoreint5 = Mathf.RoundToInt(highScore5);
        highScoreint6 = Mathf.RoundToInt(highScore6);
        highScoreint7 = Mathf.RoundToInt(highScore7);
        highScoreint8 = Mathf.RoundToInt(highScore8);
        highScoreint9 = Mathf.RoundToInt(highScore9);
        highScoreint10 = Mathf.RoundToInt(highScore10);
        highScoreint11 = Mathf.RoundToInt(highScore11);
        highScoreint12 = Mathf.RoundToInt(highScore12);
        highScoreint13 = Mathf.RoundToInt(highScore13);
        highScoreint14= Mathf.RoundToInt(highScore14);
        highScoreint15 = Mathf.RoundToInt(highScore15);
        highScoreint16 = Mathf.RoundToInt(highScore16);
        highScoreint17 = Mathf.RoundToInt(highScore17);
        highScoreint18 = Mathf.RoundToInt(highScore18);
        highScoreint19 = Mathf.RoundToInt(highScore19);
        highScoreint20 = Mathf.RoundToInt(highScore20);
        highScoreint21 = Mathf.RoundToInt(highScore21);
    }
	
	// Update is called once per frame
	void Update () {
        if (highScoreint1 >= 90)
        {
            button2.GetComponent<Button>().interactable = true;
        }
       
        if (highScoreint2 >= 80)
        {
            button3.GetComponent<Button>().interactable = true;
        }
       
        if (highScoreint3 >= 60)
        {
            button4.GetComponent<Button>().interactable = true;
        }
        
        if (highScoreint4 >= 100)
        {
            button5.GetComponent<Button>().interactable = true;
        }
        if (highScoreint5 >= 90)
        {
            button6.GetComponent<Button>().interactable = true;
        }

        if (highScoreint6 >= 100)
        {
            button7.GetComponent<Button>().interactable = true;
        }

        if (highScoreint7 >= 100)
        {
            button8.GetComponent<Button>().interactable = true;
        }

        if (highScoreint8 >= 100)
        {
            button9.GetComponent<Button>().interactable = true;
        }
        if (highScoreint9 >= 100)
        {
            button10.GetComponent<Button>().interactable = true;
        }

        if (highScoreint10 >= 80)
        {
            button11.GetComponent<Button>().interactable = true;
        }

        if (highScoreint11 >= 80)
        {
            button12.GetComponent<Button>().interactable = true;
        }

        if (highScoreint12 >= 100)
        {
            button13.GetComponent<Button>().interactable = true;
        }
        if (highScoreint13 >= 100)
        {
            button14.GetComponent<Button>().interactable = true;
        }

        if (highScoreint14 >= 100)
        {
            button15.GetComponent<Button>().interactable = true;
        }

        if (highScoreint15 >= 100)
        {
            button16.GetComponent<Button>().interactable = true;
        }

        if (highScoreint16 >= 100)
        {
            button17.GetComponent<Button>().interactable = true;
        }
        if (highScoreint17 >= 100)
        {
            button18.GetComponent<Button>().interactable = true;
        }

        if (highScoreint18 >= 100)
        {
            button19.GetComponent<Button>().interactable = true;
        }

        if (highScoreint19 >= 100)
        {
            button20.GetComponent<Button>().interactable = true;
        }
        if (highScoreint20 >= 100)
        {
            button21.GetComponent<Button>().interactable = true;
        }

    }
}
