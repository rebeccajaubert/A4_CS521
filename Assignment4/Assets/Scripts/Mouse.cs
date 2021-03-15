using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I inspired myself from The Coding Train videos on the steering behaviours
//https://www.youtube.com/watch?v=4zhJlkGQTvU&list=PLRqwX-V7Uu6YHt0dtyf4uiw8tKOxQLvlW&index=2

public class Mouse : Obstacle
{
    private List<float> eyeDirection = new List<float>();
   
    Vector3 target;
    Vector3 currentVelocity;
    Vector3 forwardDirection;   //TODO delet
    float maxSpeed = 5f;
    float maxForce = 0.1f;
    bool atTarget = false;
    int helperStuckCounter = 0;

    public Mouse() : base("Mouse")
    {
        
    }

    public void instantiateMouse()
    {
        base.instantiateObs();
        currentVelocity = Vector3.zero;
        forwardDirection = obs.transform.rotation * Quaternion.Euler(0, 90f , 0) * Vector3.forward ;//obs.transform.forward;
        target = chooseTarget();
        for (int i=0; i<10; i++)
            eyeDirection.Add(0f);
       
       
    }

    private Vector3 chooseTarget()  //pick random target
    {
         Vector3 target = base.spawnedPosition();
        return target;
    }

    public void UpdatePosition()
    {
        if (!atTarget)
        {
            Vector3 steering = seek();
            Vector3 fleeing = flee();
            currentVelocity = (steering - fleeing); //desired - repulsive
            
            if (!fleeing.Equals(Vector3.zero))
            {
                currentVelocity =  Vector3.ClampMagnitude(currentVelocity, Mathf.Abs(steering.magnitude - fleeing.magnitude));
            }

            Vector3 prevPos = this.obs.transform.position;

            this.obs.transform.position += currentVelocity;
            rotateMouse(currentVelocity);

            if (Mathf.Abs(obs.transform.position.x - target.x) < 0.1 && Mathf.Abs(obs.transform.position.z - target.z) < 0.1)   //reached target
            {
                atTarget = true;
                StartCoroutine(waitBeforeNewTarget());
            }

            if (Mathf.Abs(prevPos.x - this.obs.transform.position.x) <= 0.05
                && Mathf.Abs(prevPos.z - this.obs.transform.position.z) <= 0.05)
            {
                helperStuckCounter++;
            }
            else helperStuckCounter = 0;
        }
    }

    private IEnumerator waitBeforeNewTarget()
    {
        yield return new WaitForSeconds(3);
        target = chooseTarget();
        atTarget = false;
    }

    private Vector3 flee()
    {
        Vector3 repulsiveForce = currentVelocity;   //fleeing will be 0 if no collision detected
        float fleeSpeed = maxSpeed;
        if (helperStuckCounter > 20)
        {
            fleeSpeed = fleeSpeed * 2f;
            if (helperStuckCounter>50) 
                fleeSpeed =  fleeSpeed * 3f;
        }
        //flee from obstacles
        bool futurCollision = Physics.Raycast(this.obs.transform.position, this.currentVelocity, 4f, TerrainManager.obstacleMask);  
        // flee from mice, player, monster
        bool movingAgentInFront = Physics.Raycast(this.obs.transform.position, this.currentVelocity, 2f, TerrainManager.agentMask);

        if (futurCollision || movingAgentInFront)
        {
            repulsiveForce = Vector3.Cross(currentVelocity, Vector3.up);  //go perpendicular and slow down
            float r = repulsiveForce.magnitude;
            if (r > 0.02) //mouse goes too fast toward obstacle = fleeing force must be strong
            {
                if (r > 0.05)
                {
                    repulsiveForce = repulsiveForce.normalized *1.5f *fleeSpeed;
                }
                else
                    repulsiveForce = repulsiveForce.normalized *1.8f * fleeSpeed;  
            }
            else //decrease force: mouse is already turning
            {
                float f = helperRemap(r, 0, 0.02f, 0.005f, 0.02f); 
                repulsiveForce = repulsiveForce.normalized * f;
            }
        }
        Vector3 fleeing = (repulsiveForce - currentVelocity) * Time.deltaTime;

        return fleeing;
    }

    
    private Vector3 seek()
    {
        Vector3 desiredForce = this.target - this.obs.transform.position;
        float d = desiredForce.magnitude;
        if (d > 7) //mouse still far from target
        {
            desiredForce = desiredForce.normalized * maxSpeed;
        }
        else //slow down
        {
            float f = helperRemap(d, 0, 7, 0.5f, maxSpeed);
            desiredForce = desiredForce.normalized * f;
        }
        Vector3 steering = (desiredForce - this.currentVelocity) * Time.deltaTime;  
        return steering;
    }

    private static float helperRemap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    
    private void rotateMouse(Vector3 force)
    {
        float targetAngle = Mathf.Atan2(-(force.z), (force.x)) * Mathf.Rad2Deg;
        eyeDirection.Add(targetAngle);
        eyeDirection.RemoveAt(0);
        float finalAngle =0f;
        for(int i=0; i<eyeDirection.Count; i++)
        {
            finalAngle += eyeDirection[i];
        }
        finalAngle /= eyeDirection.Count;   //smooth turn
         
        Quaternion rotation = Quaternion.Euler(new Vector3(0, finalAngle, 0));
        obs.transform.rotation = rotation ;
    }

}
