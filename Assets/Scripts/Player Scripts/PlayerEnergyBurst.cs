using Unity.VisualScripting;
using UnityEngine;

public class PlayerEnergyBurst : MonoBehaviour
{
    public float aoeRadius = 5f;
    public float damage = 50f;
    public float knockbackForce = 100f;
    public LayerMask enemyLayer;
    public KeyCode aoeKey = KeyCode.E;

    public GameObject aoeEffectPrefab;
    public Transform effectSpawn;
    public waveSpawner waveSpawner;

    private float cooldownMax = 0f;
    private float cooldownTimer = 0f;
    private bool canEnergyBlast = true;

    void Update()
    {
        if(!canEnergyBlast)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
                canEnergyBlast = true;
        }

        if (Input.GetKeyDown(aoeKey) && canEnergyBlast)
            DoAOEAttack();
    }

    void DoAOEAttack()
    {
        canEnergyBlast = false;
        cooldownTimer = cooldownMax;
        Vector3 spawnPos = effectSpawn ? effectSpawn.position : transform.position;

        if (aoeEffectPrefab != null)
            Instantiate(aoeEffectPrefab, effectSpawn ? effectSpawn.position : transform.position, Quaternion.identity);

        if (waveSpawner != null)
            waveSpawner.TriggerShock(spawnPos);

        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius, enemyLayer);
        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 knockDir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
            }

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}