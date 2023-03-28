using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class followPlayer : MonoBehaviour
{
    [SerializeField] Transform target;
    NavMeshAgent navMesh;

    [SerializeField] Transform followPos;
    [SerializeField] float wanderRadius;
    [SerializeField] float wanderTimer;
    private float timer;

    [SerializeField] GameObject[] collectables;
    [SerializeField] float[] objectDistances;
    [SerializeField] int findDistance;

    private bool setToFind = false;
    private bool setToHunt = false;
    private bool setToGo = false;
    private bool setToRecall = false;
    private bool setToFollow = false;

    // Enemy AI Stuff
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    [SerializeField] GameObject[] enemies;


    public float sightRange;
    public float attackRange;

    public bool enemyInSightRange;
    public bool enemyInAttackRange;

    public float currentSightRange, currentAttackRange;

    public float health;

    public NotificationManager notification;


    private void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        setToRecall = true;
        navMesh.SetDestination(followPos.position);
        timer = wanderTimer;
        collectables = GameObject.FindGameObjectsWithTag("Collectable");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        notification = GameObject.Find("Game Manager").GetComponent<NotificationManager>();

        currentAttackRange = attackRange;
        currentSightRange = sightRange;
    }

    private void Update()
    {
        //navMesh.SetDestination(target.position); 
        // if enabled, follows mouse and others follow player,
        // if disabled follows marker other do not follow
        //GetTarget(target);

        if (setToFollow)
        {
            navMesh.SetDestination(followPos.transform.position);
        }
        else
        {
            setToFollow = false;
        }

        if (setToFind)
        {
            objectDistances = new float[3];
            collectables = GameObject.FindGameObjectsWithTag("Collectable");
            if (collectables != null)
            {
                foreach (GameObject gameObject in collectables)
                {
                    float distance = Vector3.Distance(gameObject.transform.position, navMesh.transform.position);
                    if (objectDistances[0] == 0)
                    {
                        objectDistances[0] = distance;
                    }
                    else if (objectDistances[0] != 0)
                    {
                        objectDistances[1] = distance;
                    }
                    else if (objectDistances[0] != 0 && objectDistances[1] != 0)
                    {
                        objectDistances[2] = distance;
                    }
                    Vector3 collectableDestination = gameObject.transform.position;
                    if (distance < findDistance)
                    {
                        navMesh.SetDestination(collectableDestination);
                    }
                    if (distance < 3)
                    {
                        Debug.Log("Object collected");
                        Destroy(gameObject);
                        setToFind = false;
                        SetToRecall(followPos.transform.position);
                        notification.CallSend("Object Collected", 3);
                    }
                }
            }
            else if (collectables == null)
            {
                Debug.Log("Collectables are null");
                notification.CallSend("All Objects Collected", 3);
            }
        }

        if (setToHunt)
        {
            objectDistances = new float[3];
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies != null)
            {
                foreach (GameObject gameObject in enemies)
                {
                    float distance = Vector3.Distance(gameObject.transform.position, navMesh.transform.position);
                    if (objectDistances[0] == 0)
                    {
                        objectDistances[0] = distance;
                    }
                    else if (objectDistances[0] != 0)
                    {
                        objectDistances[1] = distance;
                    }
                    else if (objectDistances[0] != 0 && objectDistances[1] != 0)
                    {
                        objectDistances[2] = distance;
                    }
                    Vector3 enemyDestination = gameObject.transform.position;
                    navMesh.SetDestination(enemyDestination);
                    if (distance <= 3)
                    {
                        notification.CallSend(this + " is Attacking", 3);
                        Debug.Log("Enemy in attack range");
                        SetRange();
                        AttackEnemies();
                        DisableHunt();
                        SetToRecall(followPos.transform.position);
                    }

                    if (distance > 3)
                    {
                        ResetRanges();
                    }
                }
            }
            
        }

        enemyInSightRange = Physics.CheckSphere(transform.position, currentSightRange, enemyLayer);
        enemyInAttackRange = Physics.CheckSphere(transform.position, currentAttackRange, enemyLayer);
        if (!enemyInSightRange && !enemyInAttackRange)
        {
            // Do nothing
        }
        if (enemyInSightRange && !enemyInAttackRange)
        {
            // Chase Enemy
            HuntEnemies();
        }
        if (enemyInSightRange && enemyInAttackRange)
        {
            // Attack Enemy
            AttackEnemies();
        }

    }

    public Transform GetTarget(Transform targetPos)
    {
        return targetPos;
    }

    public void SetTarget(Vector3 targetPos)
    {
        notification.CallSend(this.name + " is going to waypoint...", 2);
        setToGo = true;
        DisableFind();
        DisableHunt();
        DisableRecall();
        DisableFollow();
        target.transform.position = targetPos;
        navMesh.SetDestination(targetPos);
        return;
    }

    public void SetToFollow()
    {
        setToFollow = true;
        DisableFind();
        DisableGo();
        DisableHunt();
        DisableRecall();
    }

    public void SetToRecall(Vector3 targetPos)
    {
        notification.CallSend("Regrouping...", 2);
        setToRecall = true;
        DisableFind();
        DisableHunt();
        DisableGo();
        DisableFollow();
        navMesh.SetDestination(targetPos);
    }

    public void SetToRoam()
    {
        //navMesh.SetDestination(RandomNavmeshLocation(10f));
        timer += Time.deltaTime;
        if (timer>= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            navMesh.SetDestination(newPos);
            timer = 0;
        }
    }

    public void FindCollectables()
    {
        notification.CallSend(this.name + " is finding...", 2);
        DisableHunt();
        DisableGo();
        DisableRecall();
        DisableFollow();
        setToFind = true;
    }

    public void HuntEnemies()
    {
        notification.CallSend(this.name + " is hunting...", 2);
        setToHunt = true;
        DisableRecall();
        DisableGo();
        DisableFind();
        DisableFollow();
    }

    public void AttackEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            navMesh.SetDestination(transform.position);
            transform.LookAt(enemies[i].transform);
            HuntEnemies();
        }
    }

    public void DisableHunt()
    {
        setToHunt = false;
    }
    public void DisableFind()
    {
        setToFind = false;
    }
    public void DisableGo()
    {
        setToGo = false;
    }
    public void DisableRecall()
    {
        setToRecall = false;
    }
    public void DisableFollow()
    {
        setToFollow = false;
    }

    public bool GetHunt()
    {
        if (setToHunt)
        {
            return true;
        }
        return false;
    }
    public bool GetFind()
    {
        if (setToFind)
        {
            return true;
        }
        return false;
    }
    public bool GetGo()
    {
        if (setToGo)
        {
            return true;
        }
        return false;
    }
    public bool GetRecall()
    {
        if (setToRecall)
        {
            return true;
        }
        return false;
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    public void ResetRanges()
    {
        currentAttackRange = attackRange;
        currentSightRange = sightRange;
    }

    public void SetRange()
    {
        currentAttackRange = 0f;
        currentSightRange = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, currentAttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentSightRange);
    }
}
