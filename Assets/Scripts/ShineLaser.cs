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
            /* get the point on the plane where the mouse is clicking */
            Plane camPlane = new Plane(Vector3.up, groundQuad.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            Vector3 mousePos;
            if (camPlane.Raycast(ray, out distance))
            {
                mousePos = ray.GetPoint(distance);
                /* At this point mousePos is where the mouse is on the ground. Now we
                project a ray from the camera at the point on the ground and the first thing we hit is where we place the laser dot */
                Vector3 rayDir = mousePos - Camera.main.transform.position;
                RaycastHit hit;

                // this debug ray connects the camera to where in the world a laser dot would land
                Debug.DrawRay(Camera.main.transform.position, rayDir, Color.red, 2, false);
                bool isHit = Physics.Raycast(Camera.main.transform.position, rayDir.normalized, out hit, Vector3.Distance(Camera.main.transform.position, mousePos),
                     ~(0), QueryTriggerInteraction.Ignore);
                if (isHit)
                {
                    mousePos = hit.point;
                    mousePos.y = hit.point.y + (laserDot.transform.localScale.y / 2.0f);

                }
                else {
                    mousePos.y = laserDot.transform.position.y;
                }
                laserDot.transform.position = mousePos;
                laserDot.SetActive(true);
            }
        }
        else
        {
            laserDot.SetActive(false);
        }
        print("Laser is at " + laserDot.transform.position);

        
    }
}
