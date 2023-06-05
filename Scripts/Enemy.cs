using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //Estado del enemigo
    enum EnemyStatus
    {
        Patrol,
        ClosePlayer,
        Combat,
        Chasing,
        Alert,
        toKnowedLastPosition,
        Searching
    }
    public Transform playerTransform;
    EnemyStatus status = EnemyStatus.Patrol;

    //Variables de patrulla
    public Transform[] patrolPoints; //Array con los puntos de patrulla del enemigo
    int actualPoint = 0;
    Vector3 originalPosition; //Posición original del enemigo
    Quaternion originalRotation; //Rotación original del enemigo
    public float SearchPatrolRadius = 10;
    float patrolTimer = 10;
    Vector3 randomPoint;

    //Componente agent del enemigo
    NavMeshAgent agent;

    //Variables relativas a movimiento
    Vector3 lastKnowPosition = Vector3.positiveInfinity;

    //Variable de animación
    Animator anim;

    //Variables de combate
    ShootingController shootingController;
    bool isAimAnimationEnded = false;
    bool isReverseAimAnimationEnded = false;
    bool isInShootCD = false;
    bool isLookingForEnemy = false;

    //FOVs del enemigo
    FieldOfView combatFieldOfView;
    FieldOfView chaseFieldOfView;
    FieldOfView closeFieldOfView;

    private void Start()
    {
        anim = GetComponent<Animator>();
        FieldOfView[] fieldofViews = GetComponents<FieldOfView>();
        combatFieldOfView = fieldofViews[0];
        chaseFieldOfView = fieldofViews[1];
        closeFieldOfView = fieldofViews[2];
        shootingController = GetComponent<ShootingController>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        switch(status)
        {
            case EnemyStatus.Patrol:
                Patrol();
                break;
            case EnemyStatus.ClosePlayer:
                SearchClosePlayer();
                break;
            case EnemyStatus.Alert:
                EnemyAlert();
                break;
            case EnemyStatus.Combat:
                Combat();
                break;
            case EnemyStatus.Chasing:
                Chasing();
                break;
            case EnemyStatus.toKnowedLastPosition:
                AdvanceToLastPlayerPosition();
                break;
            case EnemyStatus.Searching:
                Searching();
                break;
        }
    }
    void UpdateRotation()
    {
        Vector3 movementDirection = agent.velocity.normalized;
        if (movementDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle + 90);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 8);
        }
    }


    private Quaternion RotateToDirection(Vector3 destiny)
    {
        Vector3 direction = destiny - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle + 90);
        return rotation;
    }
    void Patrol()
    {
        DetectPlayer();
        DetectPlayerClose();
        agent.speed = 3;
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[actualPoint].position);
            UpdateRotation();
            if (!agent.pathPending && agent.remainingDistance <= 0)
            {
                actualPoint = (actualPoint + 1) % patrolPoints.Length;
            }
            if (agent.velocity.magnitude > 0)
            {
                anim.Play("Walking");
            } else
            {
                anim.Play("Idle");
            }
        } else
        {
            ReturnToOriginalPosition();
        }
    }
    void ReturnToOriginalPosition()
    {
        if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            UpdateRotation();
            agent.SetDestination(originalPosition);
            anim.Play("Walking");
        } else
        {
            anim.Play("Idle");
            if (transform.rotation != originalRotation)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * 8);
            }
        }
    }
    void DetectPlayer()
    {
        if(combatFieldOfView.visibleTargets.Count > 0)
        {
            status = EnemyStatus.Alert;
        }
    }
    void SearchingDetectPlayer()
    {
        if (chaseFieldOfView.visibleTargets.Count > 0)
        {
            status = EnemyStatus.Alert;
        }
    }
    void DetectPlayerClose()
    {
        if (closeFieldOfView.visibleTargets.Count > 0)
        {
            lastKnowPosition = closeFieldOfView.visibleTargets[0].position;
            status = EnemyStatus.ClosePlayer;
        }
    }
    void SearchClosePlayer()
    {
        DetectPlayer();
        agent.ResetPath();
        transform.rotation = Quaternion.Lerp(transform.rotation, RotateToDirection(lastKnowPosition), Time.deltaTime * 8);
        if (!isLookingForEnemy)
        {
            StartCoroutine("ReturnRotationWithDelay", 2f);
        }  
    }
    void EnemyAlert()
    {
        agent.speed = 5;
        bool isPlayerMissing = true;
        if (combatFieldOfView.visibleTargets.Count > 0)
        {
            isPlayerMissing = false;
            status = EnemyStatus.Combat;
        }
        if (chaseFieldOfView.visibleTargets.Count > 0 && combatFieldOfView.visibleTargets.Count == 0)
        {
            isPlayerMissing = false;
            status = EnemyStatus.Chasing;
        }

        if (isPlayerMissing)
        {
            status = EnemyStatus.toKnowedLastPosition;
        }
    }
    void Combat()
    {
        if (combatFieldOfView.visibleTargets.Count > 0)
        {
            agent.ResetPath();
            anim.Play("Aim");
            lastKnowPosition = combatFieldOfView.visibleTargets[0].position;
            transform.rotation = Quaternion.Lerp(transform.rotation, RotateToDirection(combatFieldOfView.visibleTargets[0].position), Time.deltaTime * 8);
            if (isAimAnimationEnded && !isInShootCD)
            {
                StartCoroutine("ShootWithDelay", .5f);
            }
        } else
        {
            isAimAnimationEnded = false;
            anim.Play("AimReverse");
            if (isReverseAimAnimationEnded)
            {
                isReverseAimAnimationEnded = false;
                status = EnemyStatus.Alert;
            }  
        }
    }
    void Chasing()
    {
        if (chaseFieldOfView.visibleTargets.Count > 0 && combatFieldOfView.visibleTargets.Count == 0)
        {
            lastKnowPosition = chaseFieldOfView.visibleTargets[0].position;
            UpdateRotation();
            agent.SetDestination(chaseFieldOfView.visibleTargets[0].position);
            anim.Play("Forward");
        } else 
        {
            status = EnemyStatus.Alert;
        }
    }
    void AdvanceToLastPlayerPosition()
    {
        SearchingDetectPlayer();
        agent.SetDestination(lastKnowPosition);
        UpdateRotation();
        anim.Play("Forward");
        if (!agent.pathPending && agent.remainingDistance <= 0)
        {
            lastKnowPosition = playerTransform.position;
            randomPoint = GetRandomPointAroundPosition(lastKnowPosition, 1, 2);
            status = EnemyStatus.Searching;
        }
    }
    void Searching()
    {
        SearchingDetectPlayer();
        agent.speed = 3;
        UpdateRotation();
        if (patrolTimer <= 0)
        {
            patrolTimer = 10;
            status = EnemyStatus.Patrol;
        } else
        {
            patrolTimer -= Time.deltaTime;
            if (!agent.pathPending && agent.remainingDistance <= 0)
            {
                anim.Play("Walking");
                randomPoint = GetRandomPointAroundPosition(lastKnowPosition, 2, 4);
                agent.SetDestination(randomPoint);
            }
        }
    }
    Vector3 GetRandomPointAroundPosition(Vector3 center, float min, float max)
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        float randomDistance = Random.Range(min, max);
        Vector3 randomPoint = center + randomDirection * randomDistance;

        NavMeshHit hit;
        while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas) || Vector3.Distance(randomPoint, center) < min)
        {
            randomDirection = Random.insideUnitSphere.normalized;
            randomDistance = Random.Range(min, max);
            randomPoint = center + randomDirection * randomDistance;
        }

        return randomPoint;
    }


    //Método provisional para evitar colisiones indefinidas
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 avoidanceDirection = transform.position - collision.transform.position;
            avoidanceDirection.y = 0f;
            avoidanceDirection.Normalize();
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = avoidanceDirection * agent.speed * 5;
        }
    }
    //...
    IEnumerator ShootWithDelay(float delay)
    {
        isInShootCD = true;
        yield return new WaitForSeconds(delay);
        if (isAimAnimationEnded)
        {
            shootingController.pistolShoot();
        }
        isInShootCD = false;
    }

    IEnumerator ReturnRotationWithDelay(float delay)
    {
        isLookingForEnemy = true;
        yield return new WaitForSeconds(delay);
        isLookingForEnemy = false;
        if(status == EnemyStatus.ClosePlayer)
        {
            status = EnemyStatus.Patrol;
        }
        
    }
    void aimAnimationEnded(AnimationEvent animationEvent)
    {
        isAimAnimationEnded = true;
    }
    void reverseAimAnimationEnded(AnimationEvent animationEvent)
    {
        isReverseAimAnimationEnded = true;
    }
}
