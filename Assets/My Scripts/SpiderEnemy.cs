using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SpiderEnemy : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Stats")]
    public float maxHealth = 80f;
    public float health = 80f;
    public int attackDamage = 12;
    public int xpReward = 18;

    [Header("Movement - Fast & Aggressive")]
    public float walkPointRange = 10f;
    public float moveSpeedMultiplier = 1.3f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Combat - Poison Melee")]
    public float sightRange = 18f;
    public float attackRange = 2.2f;
    public float timeBetweenAttacks = 1.8f;
    public float attackWindupTime = 0.4f;
    public int poisonDamage = 3;
    public float poisonDuration = 5f;
    private bool alreadyAttacked;
    private bool isStunned = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip aggroSound;

    [Header("VFX")]
    public GameObject deathEffectPrefab;
    public ParticleSystem bloodEffect;
    public ParticleSystem poisonEffect;

    private enum State { Patrol, Chase, Attack, Stunned, Dead }
    private State currentState = State.Patrol;
    private HitEffect hitEffect;
    private bool hasAggroed = false;
    private EnemyHealthBar healthBar;

    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();
        if (!player)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) player = playerObj.transform;
        }

        hitEffect = gameObject.AddComponent<HitEffect>();
        healthBar = gameObject.AddComponent<EnemyHealthBar>();

        agent.updateRotation = false;
        agent.stoppingDistance = attackRange * 0.7f;
        agent.speed *= moveSpeedMultiplier;

        if (animator) animator.applyRootMotion = false;

        health = maxHealth;
    }

    private void Update()
    {
        if (currentState == State.Dead || !player || !agent.isOnNavMesh) return;

        HubZone hubZone = FindFirstObjectByType<HubZone>();
        if (hubZone != null && hubZone.IsPlayerInHub())
        {
            agent.isStopped = true;
            if (animator) animator.SetFloat("Speed", 0f);
            return;
        }

        if (isStunned) return;

        bool playerInSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        bool playerInAttack = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSight && !playerInAttack) SwitchState(State.Patrol);
        else if (playerInSight && !playerInAttack) SwitchState(State.Chase);
        else if (playerInAttack) SwitchState(State.Attack);

        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
            case State.Attack: Attack(); break;
        }

        UpdateAnimation();
        SmoothRotate();
    }

    private void SwitchState(State newState)
    {
        if (currentState == newState) return;

        if (newState == State.Chase && !hasAggroed)
        {
            hasAggroed = true;
            PlaySound(aggroSound);
        }

        currentState = newState;
    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.isStopped = false;
            agent.SetDestination(walkPoint);

            if (Vector3.Distance(transform.position, walkPoint) < 1f)
                walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = transform.position + new Vector3(
                Random.Range(-walkPointRange, walkPointRange),
                0,
                Random.Range(-walkPointRange, walkPointRange)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                walkPoint = hit.position;
                walkPointSet = true;
                return;
            }
        }
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        agent.isStopped = true;

        if (!alreadyAttacked)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        alreadyAttacked = true;

        if (animator) animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackWindupTime);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange * 1.2f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                ApplyPoison(playerHealth);
                PlaySound(attackSound);
            }
        }

        yield return new WaitForSeconds(timeBetweenAttacks - attackWindupTime);
        alreadyAttacked = false;
    }

    private void ApplyPoison(PlayerHealth playerHealth)
    {
        if (playerHealth == null) return;

        playerHealth.StartCoroutine(PoisonDamageOverTime(playerHealth));

        if (poisonEffect != null)
        {
            ParticleSystem poison = Instantiate(poisonEffect, player.position + Vector3.up * 1f, Quaternion.identity);
            poison.transform.SetParent(player);
            Destroy(poison.gameObject, poisonDuration);
        }
    }

    private IEnumerator PoisonDamageOverTime(PlayerHealth playerHealth)
    {
        float elapsed = 0f;
        float tickRate = 1f;

        while (elapsed < poisonDuration)
        {
            yield return new WaitForSeconds(tickRate);
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(poisonDamage);
            }
            elapsed += tickRate;
        }
    }

    public void TakeDamage(int damage, Vector3 hitDirection, float knockbackForce)
    {
        if (currentState == State.Dead) return;

        health -= damage;

        if (healthBar != null)
        {
            healthBar.UpdateHealth(health, maxHealth);
        }

        if (hitEffect != null)
            hitEffect.FlashDamage(Color.red, 0.1f);

        PlaySound(hurtSound);

        if (bloodEffect != null)
        {
            ParticleSystem blood = Instantiate(bloodEffect, transform.position + Vector3.up * 1f, Quaternion.identity);
            Destroy(blood.gameObject, 2f);
        }

        if (health > 0)
        {
            StartCoroutine(ApplyKnockback(hitDirection, knockbackForce));
        }
        else
        {
            Die();
        }
    }

    private IEnumerator ApplyKnockback(Vector3 direction, float force)
    {
        isStunned = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            float curve = 1f - (elapsed / duration);
            Vector3 knockback = direction * force * curve;
            agent.Move(knockback * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        isStunned = false;
    }

    private void Die()
    {
        currentState = State.Dead;
        isStunned = true;

        if (animator)
        {
            animator.SetTrigger("Die");
        }

        PlaySound(deathSound);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GainXP(xpReward);
        }

        ArenaManager arenaManager = FindFirstObjectByType<ArenaManager>();
        if (arenaManager != null)
        {
            arenaManager.OnEnemyKilled();
        }

        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Destroy(effect, 3f);
        }

        Destroy(gameObject, 2f);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void UpdateAnimation()
    {
        if (!animator) return;

        float speed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", Mathf.Clamp01(speed));
    }

    private void SmoothRotate()
    {
        if (currentState == State.Attack || isStunned) return;
        if (agent.velocity.sqrMagnitude < 0.1f) return;

        Vector3 direction = agent.velocity.normalized;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 12f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (walkPointSet)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(walkPoint, 0.4f);
        }
    }
}
