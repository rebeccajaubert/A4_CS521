using UnityEngine;
public class MoveTask : Task
{
    private bool hasDestination = false; //has a location to reach been chosen
    private Vector3 dest;

    public MoveTask(MonsterPlanner _monsterPlanner)
    {
        monsterPlanner = _monsterPlanner;
        preConditions = new WorldState(monsterPlanner.controller, true, false, false, false, false); //dont care last 2
        postConditions = preConditions;
    }

    public override void execute()
    {
        //pick a random location
        if (!hasDestination) setRandomLocation();
        else
        {
            monsterPlanner.moveTo(dest);
            if (Mathf.Abs(monsterPlanner.transform.position.x - dest.x) <= 0.5f 
                && Mathf.Abs(monsterPlanner.transform.position.z - dest.z) <= 0.5f)
            {
                isDone = true;
                monsterPlanner.currentState = this.postConditions;
            }
        }
       
    }

    private void setRandomLocation()
    {
        System.Random rnd = new System.Random();
        Transform ground = TerrainManager.terrain.GetChild(0);
        int signX = rnd.Next(0, 2) == 0 ? 1 : -1; int signZ = rnd.Next(0, 2) == 0 ? 1 : -1;
        float randomX = signX * (UnityEngine.Random.value * (ground.localScale.x - 10f) / 2);
        float randomZ = signZ * (UnityEngine.Random.value * (ground.localScale.z - 10f) / 2);
        Vector3 randPos = new Vector3(randomX, 0, randomZ);
        dest = randPos;
        hasDestination = true;
    }

    public override void reset()
    {
        isDone = false;
        hasDestination = false;
    }

    public override string ToString()
    {
        return "Move";
    }
}
