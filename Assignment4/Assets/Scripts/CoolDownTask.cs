using System;
using System.Collections;
using UnityEngine;

public class CoolDownTask : Task
{
    private int i=0;

    public CoolDownTask(MonsterPlanner _monster)
    {
        monsterPlanner = _monster;
        preConditions = new WorldState(monsterPlanner.controller,true,false,false,false,false);//dont care last 2
        postConditions = preConditions;

    }

    public override void execute()
    {
        
        if (i == 0)
        {
            monsterPlanner.StartCoroutine(wait());
        }
        i++;

    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        isDone = true;
        monsterPlanner.currentState = postConditions;
        i = 0;

    }


    public override void reset()
    {
        isDone = false;
        i = 0;
    }


    public override string ToString()
    {
        return "CoolDown";
    }
}
