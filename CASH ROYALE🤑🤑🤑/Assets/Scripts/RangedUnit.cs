using UnityEngine;

public class RangedUnit : MonoBehaviour
{
    [Header("Team Settings")]
    public UnitTeam team;

    [Header("Movement & Range")]
    public float moveSpeed = 3f;
    public float attackRange = 6f;

    [Header("Weapon Configuration")]
    [Tooltip("Drag the empty WeaponPivot GameObject here!")]
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private RangedWeapon equippedWeapon;

    private Rigidbody2D rb;
    private Health currentTarget;
    private float nextFireTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Fallbacks if things aren't dragged into the inspector slots
        if (equippedWeapon == null) equippedWeapon = GetComponentInChildren<RangedWeapon>();
        if (weaponPivot == null && equippedWeapon != null) weaponPivot = equippedWeapon.transform.parent;
    }

    private void Update()
    {
        if (equippedWeapon == null || weaponPivot == null) return;

        FindTarget();

        if (currentTarget == null)
        {
            MoveForward();
            ResetWeapon();
            return;
        }

        // Aim the pivot and flip the gun sprite
        AimWeaponPivot(currentTarget.transform.position);

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);

        if (distanceToTarget <= attackRange)
        {
            rb.linearVelocity = Vector2.zero; // Stop moving to fire

            if (Time.time >= nextFireTime)
            {
                equippedWeapon.Fire(team, currentTarget);
                nextFireTime = Time.time + (1f / equippedWeapon.fireRate);
            }
        }
        else
        {
            MoveTowardsTarget(); // Pursue target
        }
    }

    private void FindTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange + 10f);
        float closestDistance = Mathf.Infinity;
        Health potentialTarget = null;

        foreach (var col in colliders)
        {
            Health targetHealth = col.GetComponent<Health>();
            if (targetHealth != null)
            {
                bool isEnemy = false;

                MeleeUnit mUnit = col.GetComponent<MeleeUnit>();
                if (mUnit != null && mUnit.team != this.team) isEnemy = true;

                RangedUnit rUnit = col.GetComponent<RangedUnit>();
                if (rUnit != null && rUnit.team != this.team) isEnemy = true;

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

    private void MoveTowardsTarget()
    {
        float direction = (currentTarget.transform.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void AimWeaponPivot(Vector3 targetPos)
    {
        // 1. Calculate direction from the pivot point to the target enemy
        Vector3 dir = targetPos - weaponPivot.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 2. Rotate the empty pivot parent
        weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        // 3. Flip ONLY the weapon child sprite along its local Y axis based on look direction
        Vector3 weaponLocalScale = equippedWeapon.transform.localScale;

        if (targetPos.x < transform.position.x)
        {
            // Aiming Left: Invert local Y scale so the gun doesn't upside-down flip when rotated backward
            equippedWeapon.transform.localScale = new Vector3(weaponLocalScale.x, -Mathf.Abs(weaponLocalScale.y), weaponLocalScale.z);
        }
        else
        {
            // Aiming Right: Keep the weapon scale upright and normal
            equippedWeapon.transform.localScale = new Vector3(weaponLocalScale.x, Mathf.Abs(weaponLocalScale.y), weaponLocalScale.z);
        }
    }

    private void ResetWeapon()
    {
        weaponPivot.localRotation = Quaternion.identity;
        Vector3 wScale = equippedWeapon.transform.localScale;
        equippedWeapon.transform.localScale = new Vector3(wScale.x, Mathf.Abs(wScale.y), wScale.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (team == UnitTeam.Player) ? Color.green : Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}