using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Cat : MonoBehaviour {
    public Slider interestSlider;
    public Slider staminaSlider;
    protected float stamina = 1.0f;
    protected float baseSpeed = 3.5f;
    protected float baseAccel = 8.0f;
    public float interest = 0.0f;
    protected float visionAngle = 180.0f;
    protected float visionDistance = 10.0f;
    protected float speedModifier = 1.0f;
    protected float originalSpeed;
    protected float commotionDistance = 10f;
    protected Vector3 lastPos;
    protected float rate;
    protected bool tired;
    protected float staminaRegen;
    protected float staminaLoss;

    private GameObject target;
    private NavMeshAgent navComponent;

    private Vector3 lastSeen;
    public bool lastSeenValid;
    private float lastTargetUpdateTime;
    public int lastSeenID;
    private int laserID;
    public int myID;
    //XXX I think we can convert transform.getINstanceID to just getInstanceID as long as we don't cache the value in Start()
    //invokerepeating
    public virtual void Start()
    {

        rate = 0.0f;
        staminaRegen = 0.25f;
        staminaLoss = 0.07f;
        baseSpeed = 5.5f;
        baseAccel = 36.0f;
        speedModifier = (float)Random.Range(0.7f, 1.3f);


        setInit();
    }
    public virtual void setInit()
    {
        // I don't know why but this is 1 even though it is initialized to zero;
        interest = 0;
        navComponent = transform.GetComponent<NavMeshAgent>();
        originalSpeed = baseSpeed;

        tired = false;
        navComponent.speed = baseSpeed * speedModifier;
        navComponent.acceleration = baseAccel;

        lastTargetUpdateTime = Time.time;


        GameObject[] objList = GameObject.FindGameObjectsWithTag("LaserID");
        laserID = objList[0].GetComponent<ShineLaser>().laserDot.transform.GetInstanceID();
        lastSeenID = -1;
        lastSeenValid = false;
    }

    public virtual void calculateSpeed()
    {
        float timeDiff = Time.time - lastTargetUpdateTime;
        float dist = (lastPos - transform.position).magnitude;
        rate = dist / timeDiff;
        if (stamina > 0f)
        {
            stamina -= timeDiff * staminaLoss;
        }
       
        if (rate < 1.0f)
        {

            stamina += timeDiff*staminaRegen;
            if (stamina > 1.0f)
            {
                stamina = 1.0f;
            }
            if (tired && stamina > 0.80f)
            {
                tired = false;
            }
        }
       
        navComponent.speed = originalSpeed * speedModifier;
        if (stamina < 0.2f || tired)
        {
            tired = true;
            navComponent.speed *= .2f;
        }
    }
    // Update is called once per frame
    public virtual void Update()
    {
        updateSliders();
        myID = transform.GetInstanceID();
        if (Time.time - lastTargetUpdateTime <= 0.25)
        {
            return;
        }
        

        /* degrade our interest over time 
        this doesn't affect our speed   but if interest drops too low we might change targets*/
        if (lastSeenValid == true)
        {
            interest *= 0.99f;
            if (interest < 0.10f)
            {
                interest = 0;
                lastSeenValid = false;
            }
            //navComponent.speed = originalSpeed * speedModifier * interest * stamina;
        }
        target = getTarget("Laser");
        if (target != null)
        {
          
            interest = 1.0f;

            lastSeen = target.transform.position;
            Vector2 variation = Random.insideUnitCircle.normalized;
            lastSeen += new Vector3(variation.x, 0f, variation.y);
            lastSeenID = target.transform.GetInstanceID();
            lastSeenValid = true;
        }
        /* if we saw the laser recently, go to where we last saw it */
        if (lastSeenValid && lastSeenID == laserID)
        {
            goto SetMotion;
        }

        target = getTarget("Cat");
        if (target != null)
        {
            /* we should probably do this check by class of cat or cats in general
                and not by a single cat instance */
            if (lastSeenValid && lastSeenID != target.transform.GetInstanceID())
            {
                interest = 1.0f;
            }
            lastSeen = target.transform.position;
            Vector2 variation = Random.insideUnitCircle.normalized;
            lastSeen += new Vector3(variation.x, 0f, variation.y);
            lastSeenID = target.transform.GetInstanceID();
            lastSeenValid = true;
            goto SetMotion;
        }
        /* if we're idle, check if we feel something going on behind us */
        if (!lastSeenValid)
        {
            Vector3 newDir = findCommotion();
            if (newDir != Vector3.zero)
            {

                Quaternion lookRotation = Quaternion.LookRotation(newDir - transform.position, new Vector3(0f, 1f, 0f));
 
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 30 * Time.deltaTime);
                // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, .9f);

            }
        }
        if (lastSeenValid)
        {
            goto SetMotion;
        }

        calculateSpeed();
        lastPos = transform.position;
        lastTargetUpdateTime = Time.time;
        return;

        SetMotion:
        if (lastSeenValid == true)
        {
            if (Vector3.Distance(transform.position, lastSeen) < 1f)
            {
                print("GOT THERE");
                navComponent.ResetPath();
                lastSeenValid = false;
                interest = 0f;
            }
            else
            {
                navComponent.SetDestination(lastSeen);
            }
        }
        calculateSpeed();
 
        lastPos = transform.position;
        lastTargetUpdateTime = Time.time;
    }

    public virtual void updateSliders()
    {
        if (interestSlider != null)
        {
            interestSlider.value = Mathf.MoveTowards(interestSlider.value, interest, 0.15f);
            staminaSlider.value = Mathf.MoveTowards(staminaSlider.value, stamina, 0.15f);

            interestSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 0f, 2f));
            staminaSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 0f, 2.5f));
        }
    }
    public virtual GameObject getTarget(string label)
    {
        GameObject bestTarget = null;
        float bestTargetDist = Mathf.Infinity;


        GameObject[] objList = GameObject.FindGameObjectsWithTag(label);

        foreach (GameObject oneObj in objList)
        {
            if (!oneObj.activeSelf)
            {
                continue;
            }
            if (oneObj.transform.GetInstanceID() == transform.GetInstanceID())
            {
                continue;
            }
            Vector3 dirToTarget = oneObj.transform.position - transform.position;
            float visionDist2 = visionDistance * visionDistance;
            float dSqrToTarget = dirToTarget.sqrMagnitude;

            if (label == "Laser")
            {
               // print(myID + " " + Time.time + "checking laser " + canSee(oneObj) + " " + dSqrToTarget + " " + visionDist2 + " " + bestTargetDist);

            }

            if (dSqrToTarget < visionDist2 &&
                dSqrToTarget < bestTargetDist &&
                canSee(oneObj, visionAngle))
            {
                //XXX this script component type differs depending on the cat type
                if (label == "Cat" && oneObj.GetComponent<Cat>().interest <= 0.8f)
                {
                    continue;
                }
                bestTarget = oneObj;
                bestTargetDist = dSqrToTarget;
            }
        }

        return bestTarget;
    }

    public virtual bool canSee(GameObject target, float visionAngle)
    {
        RaycastHit hit;
        // first see if the target is within the visual cone
        //adding transform.up moves the location up 1 unit. We should use the height of the cylinder somehow
        //Vector3 eyeLocation = transform.position + (transform.up*0.5);
        float height = 1.0f;
        height = transform.localScale.y;
        Vector3 eyeLocation = transform.position + new Vector3(0f, height * 0.8f, 0f);
        Vector3 targetDir = target.transform.position - transform.position;
        Vector3 targetDirFromEye = target.transform.position - eyeLocation;

        //float angle = Vector3.Angle(new Vector3(targetDir.x, 0f, targetDir.z), transform.forward);
        float angle = Vector3.Angle(targetDir, transform.forward);

        //bool isHit = Physics.Raycast(eyeLocation, targetDirFromEye.normalized, out hit, visionDistance);
        //Debug.DrawLine(eyeLocation, target.transform.position, Color.red, 2, false);
        //print("From " + eyeLocation + " to " + target.transform.position);
        //print(myID + "see: " + angle + " " + target.transform.GetInstanceID() + " " + isHit);
        //if (isHit)
        //{
        //    print(myID + "hit" + hit + " target " + hit.transform.GetInstanceID() + " at " + hit.transform);
        //}; 
        if (angle < visionAngle * 0.5f &&
            Physics.Raycast(eyeLocation, targetDirFromEye.normalized, out hit, visionDistance, ~(0), QueryTriggerInteraction.Ignore) &&
            hit.transform.GetInstanceID() == target.transform.GetInstanceID())
        {
            // lastSeen = target.transform.position;

            return true;

        }
        return false;

    }

    /* 
    * Consider every excited nearby cat that we can't see and rotate to see what the commotion is about
    */
    public virtual Vector3 findCommotion()
    {
        Vector3 avgDir = Vector3.zero;
        int count = 0;
        Collider[] nearObj = Physics.OverlapSphere(transform.position, commotionDistance);
        foreach (Collider oneCollider in nearObj)
        {
            GameObject oneObj = oneCollider.gameObject;
            if (oneObj.transform.GetInstanceID() == transform.GetInstanceID())
            {
                // don't include ourselves
                continue;
            }
            // we want to check that we can't see it
            // but that if we had 360 degree vision we could (that is, the cats behind us aren't
            // blocked by a wall or something
            if (oneObj.tag == "Cat" && !canSee(oneObj, visionAngle) && canSee(oneObj, 360f))
            {
                float oneInterest = oneObj.GetComponent<Cat>().interest;
                if (oneInterest >= 0.8f)
                {
                    count++;
                    avgDir += oneObj.transform.position;
                }
            }

        }
        if (count == 0)
        {
            return Vector3.zero;
        }
        Vector3 final = new Vector3(avgDir.x / count, 0, avgDir.z / count);
        //Debug.DrawLine(transform.position, final, Color.red, 2, false);
        return final.normalized;
    }
}
