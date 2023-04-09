using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class PlayerInput : MonoBehaviour
{
    PlayerInputActions input;
    Rigidbody rigid;
    Animator anim;
    Player player;
    Weapon weapon;
    Fist fist;

    [SerializeField]
    Transform cam;

    Vector3 dir;
    Vector3 lookForward;
    Vector3 lookRight;
    Vector3 moveDir;

    [SerializeField]
    private float moveSpeed = 3.0f;
    [SerializeField]
    private float jumppower = 5.0f;

    bool isJumping = false;
    bool isAttack = false;
    bool isShield = false;

    public Weapon Weapon
    {
        get => weapon;
        set
        {
            weapon = value;
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        input = new PlayerInputActions();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
        fist = GetComponentInChildren<Fist>();

    }

    private void Start()
    {
        GameManager.Inst.MainPlayer.onDeadChange = OnDead;
    }

    private void OnEnable()
    {
        input.Player.Enable();
        input.Player.Move.performed += OnMove;      // 이동
        input.Player.Move.canceled += OnMove;
        input.Player.Jump.performed += OnJump;      // 점프
        input.Player.Attack.performed += OnAttack;  // 공격
        input.Player.Roll.performed += OnRoll;      // 구르기
        input.Player.Pickup.performed += ItemPickUp;
        input.Player.Shield.performed += ShieldAttack;
        input.Player.Shield.canceled += NoShieldAttack;
        input.ShortCut.Enable();
        input.ShortCut.InventoryOnOff.performed += OnInventoryShortCut;
        input.ShortCut.StatBoxOnOff.performed += OnCharacterBoxShortCut;

    }

    private void OnDisable()
    {
        input.ShortCut.StatBoxOnOff.performed -= OnCharacterBoxShortCut;
        input.ShortCut.InventoryOnOff.performed -= OnInventoryShortCut;
        input.ShortCut.Disable();
        input.Player.Shield.canceled -= NoShieldAttack;
        input.Player.Shield.performed -= ShieldAttack;
        input.Player.Pickup.performed -= ItemPickUp;
        input.Player.Roll.performed -= OnRoll;
        input.Player.Attack.performed -= OnAttack;
        input.Player.Jump.performed -= OnJump;
        input.Player.Move.canceled -= OnMove;
        input.Player.Move.performed -= OnMove;
        input.Player.Disable();
    }

    /// <summary>
    /// 쉴드를 들어 막는다
    /// </summary>
    /// <param name="obj"></param>
    private void ShieldAttack(InputAction.CallbackContext obj)
    {
        if (player.onShield)    // 쉴드를 가지고 있으면
        {
            anim.SetBool("OnShield", true);
            isShield = true;
        }
    }

    /// <summary>
    /// 쉴드를 내린다
    /// </summary>
    /// <param name="obj"></param>
    private void NoShieldAttack(InputAction.CallbackContext obj)
    {
        if (player.onShield)
        {
            anim.SetBool("OnShield", false);
            isShield = false;
        }
    }

    private void ItemPickUp(InputAction.CallbackContext obj)
    {
        player.ItemPickup();
    }

    private void OnRoll(InputAction.CallbackContext obj)
    {
        if (!isJumping)
        {
            anim.SetTrigger("Roll");
        }
    }

    private void OnCharacterBoxShortCut(InputAction.CallbackContext obj)
    {
        GameManager.Inst.CharacterBoxUI.CharacterBoxOnOffSwitch();
    }

    private void OnInventoryShortCut(InputAction.CallbackContext obj)
    {
        GameManager.Inst.InvenUI.InventoyOnOffSwitch();
    }

    private void OnAttack(InputAction.CallbackContext obj)
    {
        if(GameManager.Inst.CharacterBoxUI.Canvas.alpha == 0 && GameManager.Inst.InvenUI.Canvas.alpha == 0)
        {
            anim.ResetTrigger("IsAttack");
            anim.SetTrigger("IsAttack");
            anim.SetFloat("AttackStack", Mathf.Repeat(anim.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f));
        }
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        if (!isJumping)
        {
            rigid.AddForce(transform.up * jumppower, ForceMode.Impulse);
            anim.SetTrigger("Jump");
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector3>();

        anim.SetFloat("Dir.x", dir.z);
        anim.SetFloat("Dir.y", dir.x);
        anim.SetBool("IsMove", true);

        if(dir.x == 0.0f && dir.z == 0.0f)
        {
            anim.SetBool("IsMove", false);
        }
    }

    private void OnCollisionStay(Collision collision)   // 땅을 밟고 있는 도중에만 점프 가능
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    private void OnCollisionExit(Collision collision)   // 점프 하고 있는 동안에는 재 점프 불가능
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = true;
        }
    }

    void AttackCheck()
    {
        if (!isAttack)   // 공격 중이거나 방패 사용 중이 아닐때만 움직이도록 한다
        {
            transform.position += moveDir * Time.deltaTime * moveSpeed;
        }
        else  // 공격 중일 때
        {
            if (player.WeaponMode == PlayerWeaponMode.Noweapons) // 플레이어 무기가 없을 때
            {
                if (fist.weaponCol != null)
                {
                    fist.weaponCol.enabled = true;
                }
            }
            else  // 무기가 있을 때
            {
                if (weapon.weaponCol != null)
                {
                    weapon.weaponCol.enabled = true;
                }
            }
        }

        // 공격 중인지 체크

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("PunchLeft") || anim.GetCurrentAnimatorStateInfo(0).IsName("PunchRight")
            || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_OneHanded") || isShield)
        {
            isAttack = true;
        }
        else
        {
            if (player.WeaponMode == PlayerWeaponMode.Noweapons)
            {
                if (fist.weaponCol != null)
                {
                    fist.weaponCol.enabled = false;
                }
            }
            else
            {
                if (weapon.weaponCol != null)
                {
                    weapon.weaponCol.enabled = false;
                }
            }
            isAttack = false;
        }
    }

    void OnDead()
    {
        input.Player.Disable();     // 움직임 비활성화
    }

    private void Update()
    {
        lookForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
        lookRight = new Vector3(cam.right.x, 0f, cam.right.z).normalized;
        moveDir = lookForward * dir.z + lookRight * dir.x;

        transform.LookAt(transform.position + moveDir);

        AttackCheck();
    }
}
