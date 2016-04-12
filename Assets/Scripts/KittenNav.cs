using UnityEngine;
using System.Collections;

public class KittenNav : Cat {

	// Use this for initialization
	public override void Start () {
        rate = 0.0f;
        staminaRegen = 0.35f;
        staminaLoss = 0.15f;
        baseSpeed = 9.0f;
        baseAccel = 50.0f;
        speedModifier = (float)Random.Range(0.7f, 1.3f);


        setInit();
    }
	

}
