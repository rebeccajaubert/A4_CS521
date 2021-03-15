
using UnityEngine;

public class PickTask : Task
{
    private string rockOrCrate;
    private Transform targetObstacle=null;

    public PickTask(MonsterPlanner _monsterPlanner, string _rockOrCrate)
    {
        monsterPlanner = _monsterPlanner;
        rockOrCrate = _rockOrCrate;
        bool pickRock; bool pickCrate;
        if (rockOrCrate.Equals("Rock")) {
            pickRock = true;
            pickCrate = false;
        }
        else
        {
            pickRock = false;
            pickCrate = true;
        }
        preConditions = new WorldState(monsterPlanner.controller, true, false, false, pickRock, pickCrate);
        postConditions = new WorldState(monsterPlanner.controller, false, pickRock, pickCrate, false, false);
    }

    public override void execute()
    {
        //at runtime update current state of monster
       if(targetObstacle==null) monsterPlanner.currentState.updateObstacleNear();
        //assert pre conditions
        if (targetObstacle == null && monsterPlanner.currentState.Equals(preConditions, rockOrCrate))
        {
            //redundant with isThereObstacleNear() -> could be improved for efficiency
            Collider[] colliders = Physics.OverlapSphere(monsterPlanner.transform.position, 8);   //radius detection = 8
            foreach (Collider col in colliders)
            {
                if (col.CompareTag(rockOrCrate))
                {
                    col.transform.gameObject.tag = "Pick";
                    targetObstacle = col.transform;
                    break;
                }
            }
        }
        else if (targetObstacle != null)
        { 
            //move next to obstacle
            monsterPlanner.moveTo(targetObstacle.transform.position);

            float maxScale = Mathf.Max(targetObstacle.localScale.x, targetObstacle.localScale.z);
            if (Mathf.Abs( monsterPlanner.transform.position.x - targetObstacle.transform.position.x) <= 1f + maxScale / 2f
                && Mathf.Abs( monsterPlanner.transform.position.z - targetObstacle.transform.position.z ) <= 1f + maxScale / 2f)
            {
                monsterPlanner.helperDestroy.Add(targetObstacle);
                isDone = true;
                //update worldState
                monsterPlanner.currentState = this.postConditions;

            }

        }
        else
        {
            //replan : should never arrive in this htn
            Debug.Log("replan");
        }

    }

    public override void reset()
    {
        isDone = false;
        targetObstacle = null;
    }

    public override string ToString()
    {
        return "Pick"+rockOrCrate;
    }
}
