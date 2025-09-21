using UnityEngine;

public class EnemyCube : MonoBehaviour
{
    public int damage = 10;
    public float attackRate = 1f;

    private float lastAttackTime;
    private bool hasAttacked = false;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!hasAttacked || Time.time - lastAttackTime >= attackRate)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    lastAttackTime = Time.time;
                    hasAttacked = true;
                }
            }
        }
    }
}