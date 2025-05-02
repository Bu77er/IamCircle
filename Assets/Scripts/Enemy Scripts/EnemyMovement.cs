using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;             
    public float chaseRange = 15f;
    public float moveSpeed = 4f;
    public float health = 100f;

    private bool isDead = false;

    void Update()
    {
        if (isDead || !player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0f; // Lock movement to horizontal
            transform.position += direction * moveSpeed * Time.deltaTime;

            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
