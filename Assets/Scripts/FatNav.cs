using UnityEngine;
using System.Collections;

public class FatNav : Cat
{

    // Use this for initialization
    public override void Start()
    {
        rate = 0.0f;
        staminaRegen = 0.25f;
        staminaLoss = 0.05f;
        baseSpeed = 6.0f;
        baseAccel = 25.0f;
        speedModifier = (float)Random.Range(0.7f, 1.3f);


        setInit();
    }


}
