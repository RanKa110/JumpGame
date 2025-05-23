using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
//    플레이어 이동 관련
{
    [Header("이동관련")]
    public float moveSpeed;
    public float runSpeed;
    private bool isRun = false;
    public float runStaminaUse;
    private Vector2 curMovementInput;

    [Header("점프 관련")]
    public float jumpForce;
    public LayerMask groundLayerMask;

    [Header("시점")]
    public Transform cameraContainer;

    public Transform FirstPerson;
    public Transform ThirdPerson;
    public bool isFirstPerson = true;

    public float minXlook;
    public float maxXlook;
    private float camCurXRot;
    public float LookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;
    

    [Header("부활 관련")]
    public Transform savePoint;
    public bool isDead = false;
    public TextMeshProUGUI gameOverMassage;

    public Action inven;

    private Rigidbody _rb;



    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //    커서 중앙에 잠그기
        Cursor.lockState = CursorLockMode.Locked;
        //    None, Locked이렇게 있는데
        //    None은 마우스 중앙 잠금 해제!
        //    그리고 Locked는 중앙에 잠금!
        //    즉, 마우스가 안보이게 해주는거야!
        gameOverMassage.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    void Move()
    {
        if(!isDead)
        {
            Vector3 dir = transform.forward * curMovementInput.y
            + transform.right * curMovementInput.x;
            //    왜 이렇게 되는지는 이동 방향을 생각하면 편해!
            //    원래의 y값을 이용하면 위로 가게 되지만 이를
            //    전방 움직임으로 맞춰준거지
            //    그러면 x가 오른쪽인 이유는 알겠지?
            //    모를 것 같아 설명하자면 x값은 오른쪽으로 갈수록
            //    커지잖아? 방향에 맞춰다 봐도 편하고

            if (!isRun)
            {
                dir *= moveSpeed;
            }
            else
            {
                if (CharacterManager.Instance.Player.condition.UseStamina(runStaminaUse))
                {
                    dir *= runSpeed;
                }
            }

            dir.y = _rb.velocity.y;

            _rb.velocity = dir;
        }
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * LookSensitivity;
        //   마우스의 Y값에 민감도를 곱해서 위아래 보는 각도를 조절할 거야.
        //   왜 Y값이냐고? 위아래를 보려면 마우스를 위아래로 움직이잖아?
        //   그래서 마우스의 Y축 이동량을 가져오는 거야!

        camCurXRot = Mathf.Clamp(camCurXRot, minXlook, maxXlook);
        //   위아래로 볼 수 있는 각도 범위를 제한해줄 거야.
        //   이걸 안 하면 360도로 빙글빙글 돌아가버릴 수도 있어서 조절이 힘들어지고,
        //   멀미도 날 수 있거든!

       
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
            //   실제 카메라 회전은 여기서 처리돼.
            //   근데 왜 -를 붙였냐면, 마우스를 아래로 내리면 Y값이 +로 증가해.
            //   근데 실제로는 마우스를 아래로 내리면 카메라는 '아래'를 봐야 하잖아?
            //   그래서 위아래 방향을 우리가 익숙한 감각대로 만들기 위해 -를 붙이는 거야.
        


        transform.eulerAngles += new Vector3(0, mouseDelta.x * LookSensitivity, 0);
        //   이건 좌우 회전!
        //   마우스를 좌우로 움직이면 X값이 바뀌지.
        //   좌우 보기 = Y축 회전이니까, 마우스의 X값을 Y축 회전에 적용하는 거야.
        //   만약 마우스의 X값을 X축 회전에 써버리면...
        //   캐릭터가 앞뒤로 뒤집히는 이상한 일이 생길 거야!
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //    움직임 값 받아오기
            curMovementInput = context.ReadValue<Vector2>();
            //    InputSystem을 만들 때를 생각하면 어떤 값을 가져올지
            //    바로 알 수 있어! 모르겠다면 다시 보고오는것도 좋지!
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }


    //    시점관련
    public void OnLook(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            mouseDelta = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            if (context.phase == InputActionPhase.Started && IsGrounded())
            {
                _rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
                //    Impulse는 순간적으로 힘을 주는걸 말해
            }
        }
        
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void OnOpenInven(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inven?.Invoke();
            ToggleCursor();
        }
    }

    public void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                isRun = true;
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                isRun = false;
            }
        }
       
    }

    public void OnRespawn(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && isDead)
        {
            transform.position = savePoint.position;
            gameOverMassage.gameObject.SetActive(false);
            isDead = false;
            ToggleCursor();
        }
    }

    public void OnSwicthPerson(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (isFirstPerson)
            {
                Camera.main.transform.position = ThirdPerson.position;
                isFirstPerson = false;
            }
            else
            {
                Camera.main.transform.position = FirstPerson.position;
                isFirstPerson = true;
            }
            
        }
    }
}
