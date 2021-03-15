
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public readonly static int nbRocks = 10;
    public readonly static int nbCrates = 8;
    public readonly static int nbMice = 5;
    public static List<Obstacle> obstacles = new List<Obstacle>();  //to delete
    public static List<Mouse> mice = new List<Mouse>();

    public static Transform terrain;
    public static GameObject rock;
    public static GameObject crate;
    public static GameObject mouse;
    public static LayerMask obstacleMask;
    public static LayerMask agentMask;

    public static bool playerDetected = false;


    void Awake()
    {
        terrain = GameObject.Find("Terrain").transform;
        rock = Resources.Load("Rock") as GameObject;
        crate = Resources.Load("Crate") as GameObject;
        mouse = Resources.Load("Mouse") as GameObject;
        obstacleMask = LayerMask.GetMask("Obstacle");
        agentMask = LayerMask.GetMask("Agent");
        createObstacles();
    }

    private void Update()
    {
        foreach(Mouse mouse in mice)
        {
            mouse.UpdatePosition();
        }
    }


    private void createObstacles()
    {
        for(int i=0; i<nbRocks; i++)
        {
            Rock r = gameObject.AddComponent<Rock>();
            r.instantiateObs();
        }
        for (int i = 0; i < nbCrates; i++)
        {
            Crate c = gameObject.AddComponent<Crate>();
            c.instantiateObs();
        }
        for (int i = 0; i < nbMice; i++)
        {
            Mouse m = gameObject.AddComponent<Mouse>();
            m.instantiateMouse();
        }

        //player cannot walk directly to cave
        GameObject blockpath = GameObject.Find("ObstaclesWall");
        foreach (Transform child in blockpath.transform)
        {
            Obstacle o = gameObject.AddComponent<Obstacle>();
            o.instantiateObs(child.position);
            Destroy(child.gameObject);
        }

    }
}
