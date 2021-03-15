using UnityEngine;

public class WorldState
{
    //corresopnds to :
    //< monsterHasø, mHasRock, mHasCrate, rockNearBy, crateNearBy>
    public bool[] worldState = new bool[5];
    private CharacterController monster;

    public WorldState(CharacterController _monster) //initial worldState
    {
        monster = _monster;
        worldState[0] = true;
        worldState[1] = false;
        worldState[2] = false;
        worldState[3] = isThereObstacleNear("Rock");
        worldState[4] = isThereObstacleNear("Crate");
    }

    public WorldState(CharacterController _monster, bool monsterHasø,bool mHasRock,
        bool mHasCrate, bool rockNearBy, bool crateNearBy)
    {
        monster = _monster;
        worldState[0] = monsterHasø;
        worldState[1] = mHasRock;
        worldState[2] = mHasCrate;
        worldState[3] = rockNearBy;
        worldState[4] = crateNearBy;
    }

    public void updateObstacleNear()
    {
        worldState[3] = isThereObstacleNear("Rock");
        worldState[4] = isThereObstacleNear("Crate");
    } 

    private bool isThereObstacleNear(string rockOrCrate)
    {
        Collider[] colliders = Physics.OverlapSphere(monster.transform.position, 8);   //radius detection = 8

        foreach (Collider col in colliders)
        {
            if (col.CompareTag(rockOrCrate))
            {
                return true;
            }
        }
        return false;
    }


    public  bool Equals(WorldState obj, string rockOrCrate)
    {
        WorldState otherState = (WorldState) obj;
        for(int i=0; i<3; i++)
        {
            if (!worldState[i].Equals(otherState.worldState[i])) return false;
        }
        if (rockOrCrate.Equals("Rock"))
            if (!worldState[3].Equals(otherState.worldState[3])) return false;
        if(rockOrCrate.Equals("Crate"))
        {
                if (!worldState[4].Equals(otherState.worldState[4])) return false;
        }
        //if string is something else then don't care about those values
        return true;
    }

    public override string ToString()
    {
        string state = "<";
        foreach(bool b in worldState)
        {
            state += b.ToString() + " ,";
        }
        state += " >";
        return state;
    }
}
