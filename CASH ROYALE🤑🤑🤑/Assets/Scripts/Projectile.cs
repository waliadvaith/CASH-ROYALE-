using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    private Vector2 moveDirection;

    [Header("Combat")]
    private float damage;
    private UnitTeam teamToTarget;

    public void Setup(Vector2 direction, float weaponDamage, UnitTeam projectileTeam)
    {
        moveDirection = direction.normalized;
        damage = weaponDamage;

        // Target the opposite team
        teamToTarget = (projectileTeam == UnitTeam.Player) ? UnitTeam.Enemy : UnitTeam.Player;

        // Rotate projectile to face the direction it's flying
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        // Move forward constantly
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we hit something with a Health component
        Health targetHealth = collision.GetComponent<Health>();
        if (targetHealth != null)
        {
            // Make sure it belongs to the opposite team
            // (We check MeleeUnit or RangedUnit components to identify teams)
            bool isValidTarget = false;

            MeleeUnit melee = collision.GetComponent<MeleeUnit>();
            if (melee != null && melee.team == teamToTarget) isValidTarget = true;

            RangedUnit ranged = collision.GetComponent<RangedUnit>();
            if (ranged != null && ranged.team == teamToTarget) isValidTarget = true;

            if (isValidTarget)
            {
                targetHealth.TakeDamage(damage);
                Destroy(gameObject); // Pop! Destroy bullet on impact
            }
        }
    }
}