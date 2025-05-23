using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float atkRate;
    public bool attacking;
    public float atkDistance;
    public float useStamina;

    [Header("자원 수집 관련")]
    public bool doesGetherResources;

    [Header("전투 관련")]
    public bool doesDealDamage;
    public int damage;

    private Animator _animator;
    private Camera _camera;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            if(CharacterManager.Instance.Player.condition.UseStamina(useStamina))
            {
                attacking = true;

                _animator.SetTrigger("Attack");
                Invoke("OnCanAttack", atkRate);
            }
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2,
            Screen.height / 2, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, atkDistance))
        {
            if(doesGetherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal);
            }
            
            if(hit.collider.TryGetComponent(out NPC npc))
            {
                npc.GetComponent<NPC>().TakePhysicalDamage(damage);
            }
        }
    }
}
