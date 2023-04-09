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

    // 인벤토리용 -----------------------------------------------------------------------------------------------------------------------------------------
    Inventory inven;

    ItemSlot equipWeaponSlot;     // 장착한 아이템 슬롯(무기)

    ItemSlot equipShieldSlot;   // 장착한 아이템 슬롯(방패)

    public Inventory Inven => inven;

    public ItemSlot EquipWeaponSlot => equipWeaponSlot;

    public ItemSlot EquipShieldSlot => equipShieldSlot;

    // 캐릭터창용 -----------------------------------------------------------------------------------------------------------------------------------------

    public System.Action OnWeaponChange { get; set; }
    public System.Action OnShieldChange { get; set; }

    // 캐릭터 무기 장착상태 --------------------------------------------------------------------------------------------------------------------------------
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
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Roll")) // 굴렀을 때
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

    void DamageRefresh()    // 데미지 리프레쉬
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

    void StaminaRefresh()   // 스태미너 리프레쉬
    {
        maxHP = 100.0f + GameManager.Inst.StaStats.Stat_Player * 15.0f;
        onStaminaChange?.Invoke();
    }
    void ManaRefresh()      // 마나 리프레쉬
    {
        maxMP = 150.0f + GameManager.Inst.StaStats.Stat_Player * 10.0f;
        onManaStatChange?.Invoke();
    }

    void DefenseRefresh()       // 방어력 리프레쉬
    {
        defencePower = 0.0f + GameManager.Inst.DefStats.Stat_Player * 3.0f;
        onDefenseChange?.Invoke();
    }

    void CriticalRateRefresh()      // 크리티컬 확률 리프레쉬
    {
        criticalRate = 0.0f + GameManager.Inst.LuckStats.Stat_Player * 5.0f;
        onLuckChange?.Invoke();
    }

    public void Attack(IBattle target)  // 공격
    {
        if (target != null)     // 타겟이 있을 때
        {
            if (Random.Range(0.0f, 100.0f) < criticalRate) // 치명타 확률계산
            {
                damage *= 2.0f;   
            }
            if (!target.IsInvincibility)    // 무적상태가 아닐 때만 공격가능
            {
                target.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = damage - defencePower;  // 상대의 공격력에서 방어력을 뺀 수치로 피해를 입는다.
        if (finalDamage < 1.0f)
        {
            finalDamage = 1.0f;
        }
        HP -= finalDamage;

        if (HP > 0.0f)
        {
            anim.SetTrigger("Hit");     // 체력이 있으면 피격모션
        }
        else
        {
            if (isDead == false)
            {
                Die();                  // 체력이 0 이하면 죽는다.
            }
        }
    }

    public void Die()
    {
        isDead = true;              // 죽은 상태 활성화
        anim.SetTrigger("Die");     // 죽었을 시 모션
        onDeadChange?.Invoke();     // 죽었을 시 델리게이트 활성화
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
        equipWeaponSlot.ItemEquiped = false;    // 무기슬롯 아이템 장착해체 상태
        equipWeaponSlot = null;   // 무기슬롯 비우기
        Transform weaponChild = weapon.transform.GetChild(0);   
        weaponChild.parent = null;           
        Destroy(weaponChild.gameObject);       // 무기 오브젝트 삭제
        weaponMode = PlayerWeaponMode.Noweapons;    // 무기를 들지 않은 상태로 변경
        weaponData = null;                          // 무기가 지니는 데이터 삭제
        OnWeaponChange?.Invoke();                   // 무기장착 변화시 델리게이트
        onStatsChange?.Invoke();                    // 무기가 가지고 있는 스탯 변화 델리게이트
    }

    public void EquipWeapon(ItemSlot weaponSlot)
    {
        ShowWeapons(true);  // 장비하면 무조건 보이도록
        GameObject obj = Instantiate(weaponSlot.SlotItemData.prefab, weapon.transform);  // 새로 장비할 아이템 생성하기
        obj.transform.localPosition = new(0, 0, 0);       // 부모에게 정확히 붙도록 로컬을 0,0,0으로 설정 
        equipWeaponSlot = weaponSlot;                     // 장비한 아이템 표시
        equipWeaponSlot.ItemEquiped = true;               // 무기슬롯 아이템 장착상태
        weaponMode = PlayerWeaponMode.OneHandWeapon;      // 한손무기 장착 상태로 변경
        weaponData = equipWeaponSlot.SlotItemData as ItemData_Weapon;   // 들고있는 무기의 데이터 가져오기
        OnWeaponChange?.Invoke();
        onStatsChange?.Invoke();
    }

    public void UnEquipShield()
    {
        equipShieldSlot.ItemEquiped = false;
        equipShieldSlot = null;                 // 장비가 해제됐다는 것을 표시하기 위함
        Transform shieldChild = shield.transform.GetChild(0);
        shieldChild.parent = null;       
        Destroy(shieldChild.gameObject);      
        shieldMode = PlayerShieldMode.NoShield;     // 방패를 들지 않은 상태로 변경
        shieldData = null;                          // 방패가 지니는 데이터 삭제
        OnShieldChange?.Invoke();
        onStatsChange?.Invoke();
    }

    public void EquipShield(ItemSlot shieldSlot)
    {
        ShowWeapons(true);  // 장비하면 무조건 보이도록
        GameObject obj = Instantiate(shieldSlot.SlotItemData.prefab, shield.transform);  // 새로 장비할 아이템 생성하기
        obj.transform.localPosition = new(0, 0, 0);             // 부모에게 정확히 붙도록 로컬을 0,0,0으로 설정 
        equipShieldSlot = shieldSlot;                     // 장비한 아이템 표시
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

            // as : as 앞의 변수가 as 뒤의 타입으로 캐스팅이 되면 캐스팅 된 결과를 주고 안되면 null을 준다.
            // is : is 앞의 변수가 is 뒤의 타입으로 캐스팅이 되면 true, 아니면 false.
            IConsumable consumable = item.data as IConsumable;

            if (consumable != null)
            {
                consumable.Consume(this);   // 먹자마자 소비하는 형태의 아이템은 각자의 효과에 맞게 사용됨
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
