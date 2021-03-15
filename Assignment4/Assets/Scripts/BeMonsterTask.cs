using System;
public class BeMonsterTask : CompoundTask
{
    public BeMonsterTask(MonsterPlanner _monsterPlanner)
    {
        monsterPlanner = _monsterPlanner;
        tasks.AddFirst(new AttackCompoundTask(monsterPlanner, "Rock"));
        tasks.AddLast(new  AttackCompoundTask(monsterPlanner, "Crate"));
        tasks.AddLast(new IdleCoumpoundTask(monsterPlanner));
        base.setConditions();
    }

    public override string ToString()
    {
        return "BeMonster";
    }
}
