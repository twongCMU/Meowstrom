using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinCheck : MonoBehaviour {
    private int catCount;
    private int goalCount;
    public Text winText;

    void Start()
    {
        catCount = 0;
        goalCount = GameObject.FindGameObjectsWithTag("Cat").Length;
        print("Goal is " + goalCount);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cat"))
        {
            catCount++;
        }
        if (catCount == goalCount)
        {
            winText.text = "You Win";
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Cat"))
        {
            catCount--;
        }
    }
}
