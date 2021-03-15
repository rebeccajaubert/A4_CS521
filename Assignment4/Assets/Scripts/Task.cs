using System;
public abstract class Task
{
    public WorldState preConditions;
    public WorldState postConditions;
    public bool isDone = false;

    protected MonsterPlanner monsterPlanner;

    public abstract void execute();
    public abstract override string ToString();

    public virtual void reset()
    {
        isDone = false;
    }

}
