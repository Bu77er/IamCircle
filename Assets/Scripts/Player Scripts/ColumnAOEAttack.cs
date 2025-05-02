using UnityEngine;

public class ColumnAOEAttack : MonoBehaviour
{
    public float damage = 50f;
    public float range = 6f;
    public float radius = 2f;
    public float cooldownTime = 5f;
    public KeyCode attackKey = KeyCode.Q;
    public LayerMask enemyLayer;
    public GameObject columnVFXPrefab;

    private float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(attackKey))
        {
            PerformColumnAOE();
            cooldownTimer = cooldownTime;
        }
    }
    void PerformColumnAOE()
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 castPosition = transform.position + transform.forward * range * 0.5f;
        castPosition.y = transform.position.y;

        Collider[] hits = Physics.OverlapCapsule(
            castPosition + Vector3.up * 2f,
            castPosition - Vector3.up * 2f,
            radius,
            enemyLayer);

        foreach (Collider hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // Spawn VFX
        if (columnVFXPrefab != null)
        {
            Vector3 vfxPosition = castPosition;
            Instantiate(columnVFXPrefab, vfxPosition, Quaternion.identity);
        }
    }
    void OnDrawGizmosSelected()
    {
        Vector3 castPosition = transform.position + transform.forward * range * 0.5f;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(castPosition + Vector3.up * 1f, radius);
        Gizmos.DrawWireSphere(castPosition + Vector3.forward * 4f + (Vector3.up * 1f), radius);
    }
}
