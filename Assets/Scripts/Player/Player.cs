using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IHealth, IBattle, IMana, IEquipTarget, IGrowth
{
    GameObject weapon;
    GameObject shield;
    public ParticleSystem levelUpEffect;

    Animator anim;

    bool isDead = false;

    public GameObject hitDamage;

    public System.Action onStatsChange { get; set; }
    public System.Action onDamageChange { get; set; }
    public System.Action onStaminaChange { get; set; }
    public System.Action onManaStatChange { get; set; }
    public System.Action onDefenseChange { get; set; }
    public System.Action onLuckChange { get; set; }

    public System.Action onDeadChange { get; set; }

    float damage = 0.0f;

    public float Damage
    {
        get => damage;
        set
        {
            if (damage != value)
            {
                damage = value;
                onStatsChange?.Invoke();
            }
        }
    }

    //IHealth--------------------------------------------------------------------------------------------------------------------------------------------
    public float hp = 100.0f;
    float maxHP = 100.0f;

    public float HP
    {
        get => hp;
        set
        {
            if (hp != value)
            {
                hp = Mathf.Clamp(value, 0, MaxHP);
                onHealthChange?.Invoke();
            }
        }
    }

    public float MaxHP
    {
        get => maxHP;
    }

    public System.Action onHealthChange { get; set; }
    //IMana --------------------------------------------------------------------------------------------------------------------------------------------
    public float mp = 150.0f;
    float maxMP = 150.0f;

    public float MP
    {
        get => mp;
        set
        {
            if (mp != value)
            {
                mp = Mathf.Clamp(value, 0, MaxMP);
                onManaChange?.Invoke();
            }
        }
    }

    public float MaxMP => maxMP;

    public System.Action onManaChange { get; set; }
    //IBattle--------------------------------------------------------------------------------------------------------------------------------------------
    float attackPower = 20.0f;
    float defencePower = 0.0f;
    float criticalRate = 0.0f;
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
            if(attackPower != value)
            {
                attackPower = value;
                onStatsChange?.Invoke();
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
                onStatsChange?.Invoke();
            }
        }
    }

    public float CriticalRate
    {
        get => criticalRate;
        set
        {
            if (criticalRate != value)
            {
                criticalRate = value;
                onStatsChange?.Invoke();
            }
        }
    }

    //IGrowth --------------------------------------------------------------------------------------------------------------------------------------------
    public float exp = 0.0f;
    float maxEXP = 100.0f;
    public int level = 1;
    int maxLevel = 99;


    public float EXP
    {
        get => exp;
        set
        {
            if (exp != value)
            {
                exp = value;
                onEXPChange?.Invoke();
                while(exp >= MaxEXP)
                {
                    Levelup();
                }
            }
        }
    }

    public float MaxEXP
    {
        get => maxEXP;
        set
        {
            if (maxEXP != value)
            {
                maxEXP = value;
            }
        }
    }

    public int Level
    {
        get => level;
        set
        {
            if (level != value)
            {
                level = Mathf.Clamp(value, 1, MaxLevel);
                onLevelChange?.Invoke();
            }
        }
    }

    public int MaxLevel => maxLevel;

    public System.Action onEXPChange { get; set; }
    public System.Action onLevelChange { get; set; }

    // ItemPickup----------------------------------------------------------------------------------------------------------------------------------------

    int money = 0;
    public int Money
    {
        get => money;
        set
        {
            if (money != value)
            {
                money = value;
                OnMoneyChange?.Invoke(money);
            }

        }
    }

    float itemPickupRange = 2.0f;
    float dropRange = 1.0f;
    public System.Action<int> OnMoneyChange;

    // �κ��丮�� -----------------------------------------------------------------------------------------------------------------------------------------
    Inventory inven;

    ItemSlot equipWeaponSlot;     // ������ ������ ����(����)

    ItemSlot equipShieldSlot;   // ������ ������ ����(����)

    public Inventory Inven => inven;

    public ItemSlot EquipWeaponSlot => equipWeaponSlot;

    public ItemSlot EquipShieldSlot => equipShieldSlot;

    // ĳ����â�� -----------------------------------------------------------------------------------------------------------------------------------------

    public System.Action OnWeaponChange { get; set; }
    public System.Action OnShieldChange { get; set; }

    // ĳ���� ���� �������� --------------------------------------------------------------------------------------------------------------------------------
    PlayerWeaponMode weaponMode = PlayerWeaponMode.Noweapons;
    PlayerShieldMode shieldMode = PlayerShieldMode.NoShield;

    public PlayerWeaponMode WeaponMode => weaponMode;
    public PlayerShieldMode ShieldMode => shieldMode;

    public bool onShield = false;

    ItemData_Weapon weaponData;

    ItemData_Shield shieldData;

    public ItemData_Weapon WeaponData => weaponData;

    public ItemData_Shield ShieldData => shieldData;
    // ----------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        anim = GetComponent<Animator>();
        weapon = GetComponentInChildren<FindWeapon>().gameObject;
        shield = GetComponentInChildren<FindShield>().gameObject;
        inven = new Inventory();

        onStatsChange += DamageRefresh;
        onStatsChange += StaminaRefresh;
        onStatsChange += ManaRefresh;
        onStatsChange += DefenseRefresh;
        onStatsChange += CriticalRateRefresh;
    }

    private void Start()
    {
        GameManager.Inst.InvenUI.InitializeInventory(inven);
        DamageRefresh();
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Roll")) // ������ ��
        {
            isInvincibility = true;
            anim.SetBool("Invincibility", true);
        }
        else
        {
            isInvincibility = false;
            anim.SetBool("Invincibility", false);
        }

        switch (weaponMode)
        {
            case PlayerWeaponMode.Noweapons:
                anim.SetBool("OnWeapon", false);
                break;
            case PlayerWeaponMode.OneHandWeapon:
                anim.SetBool("OnWeapon", true);
                break;
            default:    
                break;
        }

        switch (shieldMode)
        {
            case PlayerShieldMode.NoShield:
                onShield = false;
                break;
            case PlayerShieldMode.Shield:
                onShield = true;
                break;
            default:
                break;
        }
    }

    void DamageRefresh()    // ������ ��������
    {
        if (weaponData != null)
        {
            damage = AttackPower + weaponData.attackPower + (GameManager.Inst.StrStats.Stat_Player * 4.0f);
        }
        else
        {
            damage = AttackPower + (GameManager.Inst.StrStats.Stat_Player * 4.0f);
        }
        onDamageChange?.Invoke();
    }

    void StaminaRefresh()   // ���¹̳� ��������
    {
        maxHP = 100.0f + GameManager.Inst.StaStats.Stat_Player * 15.0f;
        onStaminaChange?.Invoke();
    }
    void ManaRefresh()      // ���� ��������
    {
        maxMP = 150.0f + GameManager.Inst.StaStats.Stat_Player * 10.0f;
        onManaStatChange?.Invoke();
    }

    void DefenseRefresh()       // ���� ��������
    {
        defencePower = 0.0f + GameManager.Inst.DefStats.Stat_Player * 3.0f;
        onDefenseChange?.Invoke();
    }

    void CriticalRateRefresh()      // ũ��Ƽ�� Ȯ�� ��������
    {
        criticalRate = 0.0f + GameManager.Inst.LuckStats.Stat_Player * 5.0f;
        onLuckChange?.Invoke();
    }

    public void Attack(IBattle target)  // ����
    {
        if (target != null)     // Ÿ���� ���� ��
        {
            if (Random.Range(0.0f, 100.0f) < criticalRate) // ġ��Ÿ Ȯ�����
            {
                damage *= 2.0f;   
            }
            if (!target.IsInvincibility)    // �������°� �ƴ� ���� ���ݰ���
            {
                target.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = damage - defencePower;  // ����� ���ݷ¿��� ������ �� ��ġ�� ���ظ� �Դ´�.
        if (finalDamage < 1.0f)
        {
            finalDamage = 1.0f;
        }
        HP -= finalDamage;

        if (HP > 0.0f)
        {
            anim.SetTrigger("Hit");     // ü���� ������ �ǰݸ��
        }
        else
        {
            if (isDead == false)
            {
                Die();                  // ü���� 0 ���ϸ� �״´�.
            }
        }
    }

    public void Die()
    {
        isDead = true;              // ���� ���� Ȱ��ȭ
        anim.SetTrigger("Die");     // �׾��� �� ���
        onDeadChange?.Invoke();     // �׾��� �� ��������Ʈ Ȱ��ȭ
    }

    public Vector3 ItemDropPosition(Vector3 inputPos)
    {
        Vector3 result = Vector3.zero;
        Vector3 toInputPos = inputPos - transform.position;
        if (toInputPos.sqrMagnitude > dropRange * dropRange)
        {
            result = transform.position + toInputPos.normalized * dropRange;
        }
        else
        {
            result = inputPos;
        }
        return result;
    }

    public void UnEquipWeapon()
    {
        equipWeaponSlot.ItemEquiped = false;    // ���⽽�� ������ ������ü ����
        equipWeaponSlot = null;   // ���⽽�� ����
        Transform weaponChild = weapon.transform.GetChild(0);   
        weaponChild.parent = null;           
        Destroy(weaponChild.gameObject);       // ���� ������Ʈ ����
        weaponMode = PlayerWeaponMode.Noweapons;    // ���⸦ ���� ���� ���·� ����
        weaponData = null;                          // ���Ⱑ ���ϴ� ������ ����
        OnWeaponChange?.Invoke();                   // �������� ��ȭ�� ��������Ʈ
        onStatsChange?.Invoke();                    // ���Ⱑ ������ �ִ� ���� ��ȭ ��������Ʈ
    }

    public void EquipWeapon(ItemSlot weaponSlot)
    {
        ShowWeapons(true);  // ����ϸ� ������ ���̵���
        GameObject obj = Instantiate(weaponSlot.SlotItemData.prefab, weapon.transform);  // ���� ����� ������ �����ϱ�
        obj.transform.localPosition = new(0, 0, 0);       // �θ𿡰� ��Ȯ�� �ٵ��� ������ 0,0,0���� ���� 
        equipWeaponSlot = weaponSlot;                     // ����� ������ ǥ��
        equipWeaponSlot.ItemEquiped = true;               // ���⽽�� ������ ��������
        weaponMode = PlayerWeaponMode.OneHandWeapon;      // �Ѽչ��� ���� ���·� ����
        weaponData = equipWeaponSlot.SlotItemData as ItemData_Weapon;   // ����ִ� ������ ������ ��������
        OnWeaponChange?.Invoke();
        onStatsChange?.Invoke();
    }

    public void UnEquipShield()
    {
        equipShieldSlot.ItemEquiped = false;
        equipShieldSlot = null;                 // ��� �����ƴٴ� ���� ǥ���ϱ� ����
        Transform shieldChild = shield.transform.GetChild(0);
        shieldChild.parent = null;       
        Destroy(shieldChild.gameObject);      
        shieldMode = PlayerShieldMode.NoShield;     // ���и� ���� ���� ���·� ����
        shieldData = null;                          // ���а� ���ϴ� ������ ����
        OnShieldChange?.Invoke();
        onStatsChange?.Invoke();
    }

    public void EquipShield(ItemSlot shieldSlot)
    {
        ShowWeapons(true);  // ����ϸ� ������ ���̵���
        GameObject obj = Instantiate(shieldSlot.SlotItemData.prefab, shield.transform);  // ���� ����� ������ �����ϱ�
        obj.transform.localPosition = new(0, 0, 0);             // �θ𿡰� ��Ȯ�� �ٵ��� ������ 0,0,0���� ���� 
        equipShieldSlot = shieldSlot;                     // ����� ������ ǥ��
        equipShieldSlot.ItemEquiped = true;
        shieldMode = PlayerShieldMode.Shield;
        shieldData = equipShieldSlot.SlotItemData as ItemData_Shield;
        OnShieldChange?.Invoke();
        onStatsChange?.Invoke();
    }

    public void ShowWeapons(bool isShow)
    {
        weapon.SetActive(isShow);
        shield.SetActive(isShow);
    }

    public void ItemPickup()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, itemPickupRange, LayerMask.GetMask("Item"));
        foreach (var col in cols)
        {
            Item item = col.GetComponent<Item>();

            // as : as ���� ������ as ���� Ÿ������ ĳ������ �Ǹ� ĳ���� �� ����� �ְ� �ȵǸ� null�� �ش�.
            // is : is ���� ������ is ���� Ÿ������ ĳ������ �Ǹ� true, �ƴϸ� false.
            IConsumable consumable = item.data as IConsumable;

            if (consumable != null)
            {
                consumable.Consume(this);   // ���ڸ��� �Һ��ϴ� ������ �������� ������ ȿ���� �°� ����
                Destroy(col.gameObject);
            }
            else
            {
                if (inven.AddItem(item.data))
                {
                    Destroy(col.gameObject);
                }
            }
        }
    }

    void Levelup()
    {
        EXP -= MaxEXP;
        Level++;
        MaxExpup();
        GameManager.Inst.StatPoints.StatPoint++;
        levelUpEffect.Play();
    }

    void MaxExpup()
    {
        MaxEXP *= 1.1f;
    }

    public void Test()
    {
        inven.AddItem(ItemIDCode.OneHandSword1);
        inven.AddItem(ItemIDCode.OneHandSword1);
        inven.AddItem(ItemIDCode.Shield);
        inven.AddItem(ItemIDCode.OneHandAxe);
    }
}
