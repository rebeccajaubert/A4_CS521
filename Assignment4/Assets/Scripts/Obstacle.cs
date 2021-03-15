
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GameObject obs;
    
    public Obstacle(string type)
    {
        GameObject o = TerrainManager.rock;
        switch (type){
            case "Rock":
                o = TerrainManager.rock;
                break;
            case "Crate":
                o = TerrainManager.crate;
                break;
            case "Mouse":
                o = TerrainManager.mouse;
                break;
        }

        this.obs = o;
        if (type.Equals("Mouse")) TerrainManager.mice.Add((Mouse)this);
        else TerrainManager.obstacles.Add(this);
    }
    public void instantiateObs()
    {
        Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        this.obs = Instantiate(obs, Vector3.zero, randomRotation, TerrainManager.terrain) as GameObject;
        obs.transform.position = spawnedPosition();
        
    }
    //choose randomly between Rock or Crate and force position
    public void instantiateObs(Vector3 pos)
    {
        string random = UnityEngine.Random.value > 0.5 ? "Rock" : "Crate";
        GameObject o = TerrainManager.rock;
        switch (random)
        {
            case "Rock":
                o = TerrainManager.rock;
                break;
            case "Crate":
                o = TerrainManager.crate;
                break;
        }
        this.obs = o;
        Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        this.obs = Instantiate(obs, Vector3.zero, randomRotation, TerrainManager.terrain) as GameObject;
       
        if (isOverlapping(pos))
        {
            Destroy(obs);
            return;
        }
        obs.transform.position = pos;
        TerrainManager.obstacles.Add(this);

    }

    //used for thrown obstacles : instantiate given type of obstacle at position, without any rotation and without constraints
    public void instantiateThrownObs(Vector3 pos, string rockOrCrate)
    {
        GameObject o = TerrainManager.rock;
        switch (rockOrCrate)
        {
            case "Rock":
                o = TerrainManager.rock;
                break;
            case "Crate":
                o = TerrainManager.crate;
                break;
        }
        this.obs = o;
        this.obs = Instantiate(obs, Vector3.zero, Quaternion.identity, TerrainManager.terrain) as GameObject;
        obs.gameObject.tag = "Thrown";
        Rigidbody rb = obs.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        obs.transform.position = pos;
        TerrainManager.obstacles.Add(this);

    }

    public Obstacle()
    {
        
    }

    //verify there is no other obstacle at this position
    //NB: it does not work perfectly with too many obstacles but after seeing with TA Aidan it is acceptable bc Unity doesn't notice for an unknown reason
    protected bool isOverlapping(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapBox(pos, obs.transform.localScale , obs.transform.rotation );    //obs.transform.GetComponent<BoxCollider>().size 

        foreach (Collider col in colliders)
        {
            if ( col.CompareTag("Rock") || col.CompareTag("Crate") || col.CompareTag("Mouse")) 
            {
                return true;
            }
        }
        return false;
    }

    protected Vector3 spawnedPosition()
    {
        Transform ground = this.obs.transform.parent.GetChild(0);//GameObject.Find("Ground");
        Vector3 groundLevel = ground.position + new Vector3(0, ground.localScale.y, 0);
        System.Random rnd = new System.Random();
        bool collide = true;
        Vector3 pos = Vector3.zero;
        while (collide)
        {
            int signX = rnd.Next(0, 2) == 0 ? 1 : -1; int signZ = rnd.Next(0, 2) == 0 ? 1 : -1;
            float randomX = signX * (UnityEngine.Random.value * (ground.localScale.x-5f) / 2);
            float randomZ = signZ * (UnityEngine.Random.value * (ground.localScale.z-5f) / 2);
            Vector3 randPos = new Vector3(randomX, 0, randomZ);
            pos = randPos+groundLevel;
            if (!isOverlapping(pos))
                collide = false;
        }
        return pos;
    }

    
}
