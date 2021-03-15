using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;

public class MonsterPlanner : MonoBehaviour
{
    public CharacterController controller;
    private float speed = 4f;

    public WorldState currentState;
    private Task currentTask = null;
    private MonsterHTN htn;
    private  LinkedList<Task> currentPlan = new LinkedList<Task>();
    public Text planText;
    public Text taskText;

    private bool isInFront = false;
    private Transform front;
    public List<Transform> helperDestroy = new List<Transform>(); //Tasks are not Monobehaviours so I cant destroy in Task
    private bool hasNewPlan = false;

    private void Start()
    {
        front = GameObject.Find("FrontCave").transform;

    }

   

    void Emerge()
    {
        if (Mathf.Abs(transform.position.x - (front.position.x + front.localScale.x / 2f)) >= 0.1f
            && Mathf.Abs(transform.position.z - (front.position.z + front.localScale.z / 2f)) >= 0.1f)
        {
            moveTo(front.position);
        }
        else
        {
            isInFront = true;
            //now start HTN
            currentState = new WorldState(controller);
            htn = new MonsterHTN(this);
            currentPlan = forwardPlanner();
            currentTask = currentPlan.First.Value;
            taskText.text = "Executing " + currentTask.ToString();
            currentPlan.RemoveFirst();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //emerge if player in zone for the first time
        if (TerrainManager.playerDetected && !isInFront)
            Emerge();
        if (isInFront && hasNewPlan)  //to wait for new plan
        {
            if (!currentTask.isDone) 
            {
                currentTask.execute();
            }
            else if( currentPlan.Count > 0)
            {
                currentTask.reset();
                currentTask = currentPlan.First.Value;
                currentPlan.RemoveFirst();
                taskText.text = "Executing " + currentTask.ToString();
            }
            else //find new plan
            {
                hasNewPlan = false;
                currentPlan = forwardPlanner();
                currentTask = currentPlan.First.Value; 
                taskText.text = "Executing " + currentTask.ToString();
                currentPlan.RemoveFirst();
               
            }
        }

        if (helperDestroy.Count != 0)
        {
            foreach (Transform obs in helperDestroy)
                Destroy(obs.gameObject);
            helperDestroy.Clear();
        }

    }


    public LinkedList<Task> forwardPlanner() 
    {
        string careDontcare = "";
        LinkedList <Task> plan  = new LinkedList<Task>();
        currentState.updateObstacleNear();
        WorldState state = currentState;
        LinkedList<(Task,LinkedList<Task>,Task, WorldState)> statesSaved = new LinkedList<(Task, LinkedList<Task>, Task, WorldState)>();
        LinkedList<Task> plannerTasks = new LinkedList<Task>();
        int i = 0;
        //random choice
        float random = Random.value;
        if (random > 0.5f) i = 2;   //idle behviour occurs more often

        plannerTasks.AddFirst(htn.root);

        while (plannerTasks.Count != 0)
        {
            Task t = plannerTasks.Last.Value;
            plannerTasks.RemoveLast();
            statesSaved.AddLast((t, plan, null, state));  

            if (t.GetType().BaseType.Equals(typeof(CompoundTask)))
            {
                while (statesSaved.Count != 0 || i==0)
                {
                    CompoundTask compoundT = (CompoundTask) t;
                    Task m = compoundT.tasks.ElementAt(i);
                    if (i == compoundT.tasks.Count - 1) careDontcare = "dontcare"; //idle behaviour
                    else if (m.ToString().Equals("AttackRock")) careDontcare = "Rock";
                    else if (m.ToString().Equals("AttackCrate")) careDontcare = "Crate";
                    else careDontcare = "dontcare";

                    if (m.preConditions.Equals(state,careDontcare) && m != null)  //"is there rock or crate near" is checked at runtime
                    {
                        statesSaved.AddLast((t, plan, m, state));
                        if (m.GetType().BaseType.Equals(typeof(CompoundTask)))
                        {
                            CompoundTask compoundM = (CompoundTask) m;
                            foreach (Task sub in compoundM.tasks) 
                            {
                                plannerTasks.AddFirst(sub); 
                            }
                        }
                        else //m is primitive
                            plannerTasks.AddFirst(m);

                        break;
                    }
                    else
                    {
                        //restore saved state   
                        t = statesSaved.Last.Value.Item1;
                        state = statesSaved.Last.Value.Item4;
                        i++;
                        if (i >= compoundT.tasks.Count) statesSaved.RemoveLast(); ;  //if with root = error ? = no idle behaviour chosen
                    }
                }
            }
            else //primitive
            {
                if (t.ToString().Equals("PickRock") || t.ToString().Equals("ThrowRock")) careDontcare = "Rock";
                else if (t.ToString().Equals("PickCrate") || t.ToString().Equals("ThrowCrate")) careDontcare = "Crate";
                else careDontcare = "dontcare";

                if (t.preConditions.Equals(state, careDontcare)) //"is there rock or crate near" is checked at runtime
                {
                    state = t.postConditions;
                    plan.AddLast(t);
                }
                else
                {
                    //restore saved state 
                    Debug.Log(state.ToString());
                    Debug.Log(t.ToString() + " "+t.preConditions.ToString());
                    Debug.Log("should never get here in this htn");
                    t = statesSaved.Last.Value.Item1;
                    statesSaved.RemoveLast();
                }
            }
            
        }

        planText.text = "Plan :";
        foreach(Task t in plan)
        {
            planText.text += " -> " + t.ToString();
        }
        hasNewPlan = true;
        return plan;
    }

    public void moveTo(Vector3 obsPos)
    {
        controller.Move((obsPos - transform.position).normalized * speed * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.layer == 8 && ! hit.collider.gameObject.CompareTag("Pick")) //collide with obstacle
        {
            //unfreeze obstacle position if they re pushed by monster
            Rigidbody body = hit.collider.attachedRigidbody;    
            body.constraints = RigidbodyConstraints.None;
            body.constraints = RigidbodyConstraints.FreezePositionY;
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDir *10*Time.deltaTime;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == 8 && ! collision.collider.gameObject.CompareTag("Thrown")) //collide with obstacle
        {
            collision.collider.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void helperThrowObstacle(string rockOrCrate)
    {
        Vector3 angle = new Vector3(0f, 1f, 0f);
        Obstacle o = gameObject.AddComponent<Obstacle>();
        o.instantiateThrownObs(this.controller.transform.position, rockOrCrate);
        Rigidbody rb = o.obs.GetComponent<Rigidbody>();
        rb.velocity = (controller.transform.forward + angle) * 10f * Time.deltaTime;
        
    }


}
