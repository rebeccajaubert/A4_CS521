using System;
public class IdleCoumpoundTask : CompoundTask
{
    public IdleCoumpoundTask(MonsterPlanner _monsterPlanner)
    {
        monsterPlanner = _monsterPlanner;
        tasks.AddFirst(new MoveTask(monsterPlanner));
        tasks.AddLast(new CoolDownTask(monsterPlanner));
        base.setConditions();
    }

    public override string ToString()
    {
        return "Idle";
    }
}
