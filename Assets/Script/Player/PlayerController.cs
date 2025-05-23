using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
//    �÷��̾� �̵� ����
{
    [Header("�̵�����")]
    public float moveSpeed;
    public float runSpeed;
    private bool isRun = false;
    public float runStaminaUse;
    private Vector2 curMovementInput;

    [Header("���� ����")]
    public float jumpForce;
    public LayerMask groundLayerMask;

    [Header("����")]
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
    

    [Header("��Ȱ ����")]
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
        //    Ŀ�� �߾ӿ� ��ױ�
        Cursor.lockState = CursorLockMode.Locked;
        //    None, Locked�̷��� �ִµ�
        //    None�� ���콺 �߾� ��� ����!
        //    �׸��� Locked�� �߾ӿ� ���!
        //    ��, ���콺�� �Ⱥ��̰� ���ִ°ž�!
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
            //    �� �̷��� �Ǵ����� �̵� ������ �����ϸ� ����!
            //    ������ y���� �̿��ϸ� ���� ���� ������ �̸�
            //    ���� ���������� �����ذ���
            //    �׷��� x�� �������� ������ �˰���?
            //    �� �� ���� �������ڸ� x���� ���������� ������
            //    Ŀ���ݾ�? ���⿡ ����� ���� ���ϰ�

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
        //   ���콺�� Y���� �ΰ����� ���ؼ� ���Ʒ� ���� ������ ������ �ž�.
        //   �� Y���̳İ�? ���Ʒ��� ������ ���콺�� ���Ʒ��� �������ݾ�?
        //   �׷��� ���콺�� Y�� �̵����� �������� �ž�!

        camCurXRot = Mathf.Clamp(camCurXRot, minXlook, maxXlook);
        //   ���Ʒ��� �� �� �ִ� ���� ������ �������� �ž�.
        //   �̰� �� �ϸ� 360���� ���ۺ��� ���ư����� ���� �־ ������ ���������,
        //   �̵ֹ� �� �� �ְŵ�!

       
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
            //   ���� ī�޶� ȸ���� ���⼭ ó����.
            //   �ٵ� �� -�� �ٿ��ĸ�, ���콺�� �Ʒ��� ������ Y���� +�� ������.
            //   �ٵ� �����δ� ���콺�� �Ʒ��� ������ ī�޶�� '�Ʒ�'�� ���� ���ݾ�?
            //   �׷��� ���Ʒ� ������ �츮�� �ͼ��� ������� ����� ���� -�� ���̴� �ž�.
        


        transform.eulerAngles += new Vector3(0, mouseDelta.x * LookSensitivity, 0);
        //   �̰� �¿� ȸ��!
        //   ���콺�� �¿�� �����̸� X���� �ٲ���.
        //   �¿� ���� = Y�� ȸ���̴ϱ�, ���콺�� X���� Y�� ȸ���� �����ϴ� �ž�.
        //   ���� ���콺�� X���� X�� ȸ���� �������...
        //   ĳ���Ͱ� �յڷ� �������� �̻��� ���� ���� �ž�!
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //    ������ �� �޾ƿ���
            curMovementInput = context.ReadValue<Vector2>();
            //    InputSystem�� ���� ���� �����ϸ� � ���� ��������
            //    �ٷ� �� �� �־�! �𸣰ڴٸ� �ٽ� ������°͵� ����!
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }


    //    ��������
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
                //    Impulse�� ���������� ���� �ִ°� ����
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
