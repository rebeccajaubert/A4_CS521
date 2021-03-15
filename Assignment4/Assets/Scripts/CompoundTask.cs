
using System.Collections.Generic;

public abstract class CompoundTask : Task
{
    public LinkedList<Task> tasks = new LinkedList<Task>();
    public Task currentTask;

    public void setConditions()
    {
        preConditions = tasks.First.Value.preConditions;
        postConditions = tasks.Last.Value.postConditions;
    }
    
    public override void execute()  //TODO unuseful method TO DELETE : will never call execute on CompoundTask
    {
        currentTask.execute();
        if (currentTask.Equals(tasks.Last))
        {
            if (currentTask.isDone) //all previous tasks must be done to reach here
            {
                this.isDone = true;
                monsterPlanner.currentState = this.postConditions;
            }
        }
        else if (currentTask.isDone)    //move to next task
        {
            Task next = tasks.Find(currentTask).Next.Value;
            currentTask = next;
        }
    }

    public abstract override string ToString();
    
}
