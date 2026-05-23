using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    [Header("Weapon Configuration")]
    public GameObject projectilePrefab; // Drag your Bullet/Dart prefab here
    public Transform firePoint;          // Empty child object where bullets spawn

    [Header("Weapon Stats")]
    public float damage = 10f;
    public float attackRange = 5f;
    public float fireRate = 1.5f; // Shots per second

    public void Fire(UnitTeam team, Health target)
    {
        if (projectilePrefab == null || firePoint == null) return;

        // 1. Spawn the projectile at the firePoint
        GameObject bulletObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = bulletObj.GetComponent<Projectile>();

        if (projectile != null)
        {
            // 2. Calculate direction towards the enemy target
            Vector2 direction = (target.transform.position - firePoint.position).normalized;

            // 3. Initialize the bullet
            projectile.Setup(direction, damage, team);
        }
    }
}