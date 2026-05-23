using System.Collections;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 15f;
    public float attackRange = 1.5f;
    public float attackRate = 1f;

    [Header("Visual Swing Settings")]
    public float swingSpeed = 500f; // Fast swing!
    public float maxSwingAngle = -60f; // Target angle relative to start

    private Quaternion startRotation;
    private bool isSwinging = false;

    private void Start()
    {
        // Remember the local resting rotation
        startRotation = transform.localRotation;
    }

    public bool CanAttack()
    {
        return !isSwinging;
    }

    public void ExecuteAttack(Health target)
    {
        if (isSwinging) return;
        StartCoroutine(SwingRoutine(target));
    }

    private IEnumerator SwingRoutine(Health target)
    {
        isSwinging = true;
        Debug.Log($"{gameObject.name} started visual swing coroutine!");

        // Calculate our exact target angle
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, maxSwingAngle);

        // 1. Swing Downwards towards target angle
        float t = 0;
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 1f)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                targetRotation,
                swingSpeed * Time.deltaTime
            );

            t += Time.deltaTime;
            if (t > 1f) break; // Safety timeout: don't get stuck forever if math misses
            yield return null;
        }
        transform.localRotation = targetRotation;

        // 2. Mid-swing impact point: Apply the damage
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.05f); // Short pause at full extension

        // 3. Return weapon back up to start rotation
        t = 0;
        while (Quaternion.Angle(transform.localRotation, startRotation) > 1f)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                startRotation,
                swingSpeed * Time.deltaTime
            );

            t += Time.deltaTime;
            if (t > 1f) break; // Safety timeout
            yield return null;
        }
        transform.localRotation = startRotation;

        isSwinging = false;
    }
}