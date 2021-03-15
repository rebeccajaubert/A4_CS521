
using UnityEngine;

public class ThrowTask : Task
{
    private string rockOrCrate;

    public ThrowTask(MonsterPlanner _monsterPlanner, string _rockOrCrate)
    {
        monsterPlanner = _monsterPlanner;
        rockOrCrate = _rockOrCrate;
        bool pickRock; bool pickCrate;
        if (rockOrCrate.Equals("Rock"))
        {
            pickRock = true;
            pickCrate = false;
        }
        else
        {
            pickRock = false;
            pickCrate = true;
        }
        preConditions = new WorldState(monsterPlanner.controller, false, pickRock, pickCrate, false, false);    //don't care last two
        postConditions = new WorldState(monsterPlanner.controller, true, false, false, false, false);   //dont care last two
    }

    public override void execute()
    {
        if (!monsterPlanner.currentState.Equals(preConditions, "dontcare")) //TODO delete should be in planner
        {
            //replan: should never reach here
            Debug.Log("throw replan");
        }
        else 
        {
            monsterPlanner.helperThrowObstacle(rockOrCrate);
            isDone = true;
            // update current state
            monsterPlanner.currentState = this.postConditions;
        }
       
    }

    public override void reset()
    {
        isDone = false;
    }

    public override string ToString()
    {
        return "Throw"+rockOrCrate;
    }
}
