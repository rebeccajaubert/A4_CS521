using System;
public class AttackCompoundTask : CompoundTask
{
    private string rockOrCrate;

    public AttackCompoundTask(MonsterPlanner _monsterPlanner, string _rockOrCrate)
    {
        monsterPlanner = _monsterPlanner;
        rockOrCrate = _rockOrCrate;
        tasks.AddFirst(new PickTask(monsterPlanner, rockOrCrate));
        tasks.AddLast(new ThrowTask(monsterPlanner, rockOrCrate));
        tasks.AddLast(new CoolDownTask(monsterPlanner));
        base.setConditions();
    }

    public override string ToString()
    {
        return "Attack"+rockOrCrate;
    }
}
