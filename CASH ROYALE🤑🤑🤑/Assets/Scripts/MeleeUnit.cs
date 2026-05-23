using UnityEngine;

public enum UnitTeam { Player, Enemy }


public class MeleeUnit : MonoBehaviour
{
    [Header("Team Settings")]
    public UnitTeam team;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    [Header("Combat Settings")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    public float damage = 15f;

    private Rigidbody2D rb;
    private Health currentTarget;
    private float nextAttackTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        FindTarget();

        // If there's no enemy in front of us, just keep marching down the lane
        if (currentTarget == null)
        {
            MoveForward();
            return;
        }

        // If an enemy is found, check if they are close enough to hit
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);

        if (distanceToTarget <= attackRange)
        {
            rb.linearVelocity = Vector2.zero; // Stop moving to attack

            if (Time.time >= nextAttackTime)
            {
                Attack(currentTarget);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            // If we see them but aren't quite close enough, keep walking forward
            MoveForward();
        }
    }

    private void FindTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange + 1f);
        float closestDistance = Mathf.Infinity;
        Health potentialTarget = null;

        foreach (var col in colliders)
        {
            Health targetHealth = col.GetComponent<Health>();
            if (targetHealth != null && col.gameObject != this.gameObject)
            {
                bool isEnemy = false;

                MeleeUnit otherMelee = col.GetComponent<MeleeUnit>();
                if (otherMelee != null && otherMelee.team != this.team) isEnemy = true;

                RangedUnit otherRanged = col.GetComponent<RangedUnit>();
                if (otherRanged != null && otherRanged.team != this.team) isEnemy = true;

                // Structure target fallback
                if (otherMelee == null && otherRanged == null)
                {
                    if (this.team == UnitTeam.Player && (col.name.Contains("Enemy") || col.CompareTag("Enemy"))) isEnemy = true;
                    if (this.team == UnitTeam.Enemy && (col.name.Contains("Player") || col.CompareTag("Player"))) isEnemy = true;
                }

                if (isEnemy)
                {
                    float dist = Vector2.Distance(transform.position, targetHealth.transform.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        potentialTarget = targetHealth;
                    }
                }
            }
        }
        currentTarget = potentialTarget;
    }

    private void MoveForward()
    {
        float direction = (team == UnitTeam.Player) ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void Attack(Health target)
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (team == UnitTeam.Player) ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}