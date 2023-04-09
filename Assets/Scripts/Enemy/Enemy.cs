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

    //Idle �� --------------------------------------------------------------------------------------
    float waitTime = 3.0f;
    float timeCountDown = 3.0f;

    //Patrol �� ------------------------------------------------------------------------------------
    int childCount = 0; // patrolRoute�� �ڽ� ����
    int index = 0;      // ���� ��ǥ�� patrolRoute�� �ڽ�

    //���ݿ� -----------------------------------------------------------------------------------------
    float attackCoolTime = 1.0f;
    float attackSpeed = 1.0f;
    public IBattle attackTarget;

    //������------------------------------------------------------------------------------------------
    float sightRange = 10.0f;
    float closeSightRange = 2.5f;
    Vector3 targetPosition = new();
    WaitForSeconds oneSecond = new WaitForSeconds(1.0f);
    IEnumerator repeatChase = null;
    float sightAngle = 150.0f;   //-45 ~ +45 ����

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

    //����� -----------------------------------------------------------------------------------------
    bool isDead = false;

    private void Start()
    {
        if (patrolRoute)    // �̵� ��Ʈ�� ������ ��
        {
            childCount = patrolRoute.childCount;    // �ڽ� ���� ����
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
            case EnemyState.Idle:   // �⺻ ����
                IdleUpdate();
                break;
            case EnemyState.Patrol: // ���� ����
                PatrolUpdate();
                break;
            case EnemyState.Chase:  // �÷��̾ �Ѵ� ����
                ChaseUpdate();
                break;
            case EnemyState.Attack: // �÷��̾ �����ϴ� ����
                AttackUpdate();
                break;
            //case EnemyState.Dead:
            default:
                break;
        }
    }

    /// <summary>
    /// �÷��̾� ã��
    /// </summary>
    /// <returns></returns>
    bool SearchPlayer()
    {
        bool result = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange, LayerMask.GetMask("Player"));
        if (colliders.Length > 0)   // ������ �ݶ��̴��� �ϳ��� ���� ��
        {
            Vector3 pos = colliders[0].transform.position;
            if (InSightAngle(pos))  // ���� �ޱ۾ȿ� �����ϸ�
            {
                if (!BlockByWall(pos))  // ���� ���̿� �ΰ� ���� ���� ��
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
        if (SearchPlayer()) // �÷��̾ ã������
        {
            ChangeState(EnemyState.Chase);  // �Ѵ� ���·� ����
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

        if (agent.remainingDistance <= agent.stoppingDistance)  // �����ϸ�
        {
            index++;                // ���� �ε��� ����ؼ�
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
        // ���� ���¸� �����鼭 �ؾ��� �ϵ�
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

        // �� ���·� ���鼭 �ؾ��� �ϵ�
        switch (newState)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                timeCountDown = waitTime;
                break;
            case EnemyState.Patrol:
                agent.isStopped = false;
                agent.SetDestination(patrolRoute.GetChild(index).position); // ���� �ε����� �̵�
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
    /// �÷��̾ �þ߰���(sightAngle) �ȿ� ������ true�� ����
    /// </summary>
    /// <returns></returns>
    bool InSightAngle(Vector3 targetPosition)
    {
        // �� ������ ���̰�
        float angle = Vector3.Angle(transform.forward, targetPosition - transform.position);
        // ������ �þ߹��� �������̿� �ִ��� ������
        return (sightAngle * 0.5f) > angle;
    }

    /// <summary>
    /// ���� ����� ��� �Ⱥ��̴��� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="targetPosition">Ȯ���� ����� ��ġ</param>
    /// <returns>true�� ���� ������ �ִ� ��. false�� ������ �����ʴ�.</returns>
    bool BlockByWall(Vector3 targetPosition)
    {
        bool result = true;
        Ray ray = new(transform.position, targetPosition - transform.position); // ���� �����(������, ����)
        ray.origin += Vector3.up * 0.5f;    // ������ �����̷� ���� �������� ����
        if (Physics.Raycast(ray, out RaycastHit hit, sightRange))
        {
            if (hit.collider.CompareTag("Player"))     // ���̿� ���𰡰� �ɷȴµ� "Player"�±׸� ������ ������
            {
                result = false; // �ٷ� ���� ���̴� ���� ������ ���� �ʴ�.
            }
        }

        return result;  // true�� ���� ���Ȱų� �ƹ��͵� �浹���� �ʾҰų�
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
            //������� ��
            anim.SetTrigger("Hit");
            attackCoolTime = attackSpeed;
        }
        else
        {
            //����
            Die();
        }
    }

    void Die()      //�׾��� ��
    {
        if (isDead == false)
        {
            ChangeState(EnemyState.Dead);
        }
    }
}
