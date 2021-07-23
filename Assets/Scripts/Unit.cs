using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [System.NonSerialized] public Tile tile;
    public int currentHP, maxHP, currentMoveRange, moveRange, damage, patrolRange;
    public bool enemy;
    public Vector2Int gridPos;
    public bool attacked;
    public HelthBarController HBC;
    public Animator animator;
    float deathTime;
    public float deathTimeCooldown = 2;
    public bool spellImmune;
    public float attackDelay = 0.5f;

    private void Update()
    {
        UnitUpdate();
    }

    public void ResetUnit()
    {
        currentMoveRange = moveRange;
        attacked = false;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        HBC.TakeDamage(amount);

        animator.SetTrigger("TakeDamage");
        if (currentHP <= 0)
        {
            Kill();
        }
    }
    public virtual void Kill()
    {
        tile.unit = null;
        animator.SetTrigger("Die");
        //Destroy(gameObject);
        deathTime = Time.time + deathTimeCooldown;
    }

    private void Start()
    {
        HBC.maxHP = maxHP;
        HBC.currentHP = maxHP;
    }

    protected virtual void UnitUpdate()
    {
        if(currentHP <= 0 && Time.time > deathTime)
        {
            Destroy(gameObject);
        }
    }

    public void Attack(Unit unit)
    {
        //transform.LookAt(unit.transform);
        LookAt(transform.position, unit.transform.position);
        animator.SetTrigger("Attack");
    }

    public void LookAt(Vector3 pos1, Vector3 pos2)
    {
        Vector3 v = pos2 - pos1;
        v.y = 0;
        Quaternion canvasRotation = HBC.transform.rotation;
        Vector3 position = HBC.transform.position;
        transform.rotation = Quaternion.LookRotation(v, new Vector3(0, 1, 0));
        HBC.transform.rotation = canvasRotation;
        HBC.transform.position = position;
    }
}
