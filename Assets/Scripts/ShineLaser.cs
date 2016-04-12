using UnityEngine;
using System.Collections;

public class ShineLaser : MonoBehaviour {
    public GameObject laserDot;
    public GameObject groundQuad;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetButton("Fire1"))
        {
            /*
            Vector3 mousePos = Input.mousePosition;

            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            //mousePos.y = 1.0f;
            */
            Plane camPlane = new Plane(Vector3.up, groundQuad.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            Vector3 mousePos;
            if (camPlane.Raycast(ray, out distance))
            {
                mousePos = ray.GetPoint(distance);


                mousePos.y = laserDot.transform.position.y;

                laserDot.transform.position = mousePos;
                laserDot.SetActive(true);
            }
        }
        else
        {
            laserDot.SetActive(false);
        }
        

        
    }
}
