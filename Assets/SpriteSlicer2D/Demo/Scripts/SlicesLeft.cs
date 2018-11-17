using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SlicesLeft : MonoBehaviour {
    Text text;
    int slicesleft;
    // Use this for initialization
    void Start () {
        text = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        slicesleft = 3 - SpriteSlicer2DDemoManager.sliceCount;
        text.text = ""+slicesleft;
    }
}
