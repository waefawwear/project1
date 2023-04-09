using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour, IHealth, IBattle
{
    public Transform patrolRoute;
    protected NavMeshAgent agent;
    protected Animator anim;
    public GameObject hitDamage;
    public Transform hitDamagePos;

    EnemyState state = EnemyState.Idle;

    public System.Action OnDead;

    //Idle 용 --------------------------------------------------------------------------------------
    float waitTime = 3.0f;
    float timeCountDown = 3.0f;

    //Patrol 용 ------------------------------------------------------------------------------------
    int childCount = 0; // patrolRoute의 자식 개수
    int index = 0;      // 다음 목표인 patrolRoute의 자식

    //공격용 -----------------------------------------------------------------------------------------
    float attackCoolTime = 1.0f;
    float attackSpeed = 1.0f;
    public IBattle attackTarget;

    //추적용------------------------------------------------------------------------------------------
    float sightRange = 10.0f;
    float closeSightRange = 2.5f;
    Vector3 targetPosition = new();
    WaitForSeconds oneSecond = new WaitForSeconds(1.0f);
    IEnumerator repeatChase = null;
    float sightAngle = 150.0f;   //-45 ~ +45 범위

    //IHealth -------------------------------------------------------------------------------------
    public float hp = 100.0f;
    float maxHP = 100.0f;
    public float HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0.0f, maxHP);
            onHealthChange?.Invoke();
        }
    }

    public float MaxHP { get => maxHP; }

    public System.Action onHealthChange { get; set; }

    //IBattle -------------------------------------------------------------------------------------
    public float attackPower = 10.0f;
    public float defencePower = 10.0f;
    float criticalRate = 0.1f;
    bool isInvincibility = false;

    public bool IsInvincibility
    {
        get => isInvincibility;
        set
        {
            if (isInvincibility != value)
            {
                isInvincibility = value;
            }
        }
    }

    public float AttackPower
    {
        get => attackPower;
        set
        {
            if (attackPower != value)
            {
                attackPower = value;
            }
        }
    }

    public float DefencePower
    {
        get => defencePower;
        set
        {
            if (defencePower != value)
            {
                defencePower = value;
            }
        }
    }

    //사망용 -----------------------------------------------------------------------------------------
    bool isDead = false;

    private void Start()
    {
        if (patrolRoute)    // 이동 루트가 존재할 시
        {
            childCount = patrolRoute.childCount;    // 자식 개수 설정
        }
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        switch (state)
        {
            case EnemyState.Idle:   // 기본 상태
                IdleUpdate();
                break;
            case EnemyState.Patrol: // 정찰 상태
                PatrolUpdate();
                break;
            case EnemyState.Chase:  // 플레이어를 쫓는 상태
                ChaseUpdate();
                break;
            case EnemyState.Attack: // 플레이어를 공격하는 상태
                AttackUpdate();
                break;
            //case EnemyState.Dead:
            default:
                break;
        }
    }

    /// <summary>
    /// 플레이어 찾기
    /// </summary>
    /// <returns></returns>
    bool SearchPlayer()
    {
        bool result = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange, LayerMask.GetMask("Player"));
        if (colliders.Length > 0)   // 잡히는 콜라이더가 하나라도 있을 시
        {
            Vector3 pos = colliders[0].transform.position;
            if (InSightAngle(pos))  // 추적 앵글안에 존재하면
            {
                if (!BlockByWall(pos))  // 벽을 사이에 두고 있지 않을 때
                {
                    targetPosition = pos;
                    result = true;
                }
            }
            if (!result && (pos - transform.position).sqrMagnitude < closeSightRange * closeSightRange)
            {
                targetPosition = pos;
                result = true;
            }
        }

        return result;
    }

    void IdleUpdate()
    {
        if (SearchPlayer()) // 플레이어를 찾았으면
        {
            ChangeState(EnemyState.Chase);  // 쫓는 상태로 변경
            return;
        }

        timeCountDown -= Time.deltaTime;
        if (timeCountDown < 0)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }
    }

    void PatrolUpdate()
    {
        if (SearchPlayer())
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)  // 도착하면
        {
            index++;                // 다음 인덱스 계산해서
            index %= childCount;    // index = index % childCount;

            ChangeState(EnemyState.Idle);
            return;
        }
    }

    void ChaseUpdate()
    {
        if (!SearchPlayer())
        {
            ChangeState(EnemyState.Patrol);
            return;
        }
    }

    private void AttackUpdate()
    {
        attackCoolTime -= Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(attackTarget.transform.position - transform.position), 0.1f);
        if (attackCoolTime < 0.0f)
        {
            anim.SetTrigger("Attack");
            //Attack(attackTarget);
            attackCoolTime = attackSpeed;
        }
    }

    IEnumerator RepeatChase()
    {
        while (true)
        {
            yield return oneSecond;
            agent.SetDestination(targetPosition);
        }
    }

    protected virtual void DeadState()
    {
        anim.SetBool("Dead", true);
        anim.SetTrigger("Die");
        isDead = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        HP = 0;
        gameObject.layer = LayerMask.NameToLayer("Die");
        Destroy(this.gameObject, 5.0f);
    }

    public void ChangeState(EnemyState newState)
    {
        if (isDead)
        {
            return;
        }
        // 이전 상태를 나가면서 해야할 일들
        switch (state)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                break;
            case EnemyState.Patrol:
                agent.isStopped = true;
                break;
            case EnemyState.Chase:
                agent.isStopped = true;
                StopCoroutine(repeatChase);
                break;
            case EnemyState.Attack:
                agent.isStopped = true;
                attackTarget = null;
                break;
            case EnemyState.Dead:
                agent.isStopped = true;
                isDead = false;
                break;
            default:
                break;
        }

        // 새 상태로 들어가면서 해야할 일들
        switch (newState)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                timeCountDown = waitTime;
                break;
            case EnemyState.Patrol:
                agent.isStopped = false;
                agent.SetDestination(patrolRoute.GetChild(index).position); // 다음 인덱스로 이동
                break;
            case EnemyState.Chase:
                agent.isStopped = false;
                agent.SetDestination(targetPosition);
                repeatChase = RepeatChase();
                StartCoroutine(repeatChase);
                break;
            case EnemyState.Attack:
                agent.isStopped = true;
                attackCoolTime = attackSpeed;
                break;
            case EnemyState.Dead:
                DeadState();
                break;
            default:
                break;
        }

        state = newState;
        anim.SetInteger("EnemyState", (int)state);
    }

    /// <summary>
    /// 플레이어가 시야각도(sightAngle) 안에 있으면 true를 리턴
    /// </summary>
    /// <returns></returns>
    bool InSightAngle(Vector3 targetPosition)
    {
        // 두 백터의 사이각
        float angle = Vector3.Angle(transform.forward, targetPosition - transform.position);
        // 몬스터의 시야범위 각도사이에 있는지 없는지
        return (sightAngle * 0.5f) > angle;
    }

    /// <summary>
    /// 벽에 대상이 숨어서 안보이는지 확인하는 함수
    /// </summary>
    /// <param name="targetPosition">확인할 대상의 위치</param>
    /// <returns>true면 벽에 가려져 있는 것. false면 가려져 있지않다.</returns>
    bool BlockByWall(Vector3 targetPosition)
    {
        bool result = true;
        Ray ray = new(transform.position, targetPosition - transform.position); // 레이 만들기(시작점, 방향)
        ray.origin += Vector3.up * 0.5f;    // 몬스터의 눈높이로 레이 시작점을 높임
        if (Physics.Raycast(ray, out RaycastHit hit, sightRange))
        {
            if (hit.collider.CompareTag("Player"))     // 레이에 무언가가 걸렸는데 "Player"태그를 가지고 있으면
            {
                result = false; // 바로 보인 것이니 벽이 가리고 있지 않다.
            }
        }

        return result;  // true면 벽이 가렸거나 아무것도 충돌하지 않았거나
    }

    public void Attack(IBattle target)
    {
        if (target != null)
        {
            float damage = AttackPower;
            if (Random.Range(0.0f, 1.0f) < criticalRate)
            {
                damage *= 2.0f;
            }
            if (!target.IsInvincibility)
            {
                target.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = damage - defencePower;
        if (finalDamage < 1.0f)
        {
            finalDamage = 1.0f;
        }
        HP -= finalDamage;

        if (!isDead)
        {
            GameObject obj = Instantiate(hitDamage, transform);
            obj.transform.position = hitDamagePos.position;
            Destroy(obj, 0.3f);
        }


        if (HP > 0.0f)
        {
            //살아있을 때
            anim.SetTrigger("Hit");
            attackCoolTime = attackSpeed;
        }
        else
        {
            //죽음
            Die();
        }
    }

    void Die()      //죽었을 때
    {
        if (isDead == false)
        {
            ChangeState(EnemyState.Dead);
        }
    }
}
