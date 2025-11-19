using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttackWithCombos : MonoBehaviour
{
    [Header("Attack Settings")]
    public int baseDamage = 25;
    public int[] comboDamage = new int[] { 25, 35, 60 };
    public float attackSpeed = 1.2f;
    public float attackCooldown = 0.5f;
    public float attackRange = 2.8f;
    public float attackAngle = 90f;
    public LayerMask enemyLayer;
    
    [Header("Combo System")]
    public bool useComboAnimations = true;
    public float comboWindow = 1.0f;
    public float comboDamageMultiplier = 1.3f;
    public int maxComboCount = 3;
    public string[] comboTriggers = new string[] { "Attack1", "Attack2", "Attack3" };
    
    [Header("Attack Timing Per Combo")]
    public float[] hitDetectionDelays = new float[] { 0.15f, 0.18f, 0.2f };
    public float[] recoveryTimes = new float[] { 0.2f, 0.25f, 0.3f };

    [Header("References")]
    public Transform swordTransform;
    public Transform attackPoint;
    public Animator animator;
    public TrailRenderer swordTrail;

    [Header("Audio")]
    public AudioClip[] whooshSounds;
    public AudioClip[] hitSounds;
    private AudioSource audioSource;

    [Header("Visual Effects")]
    public ParticleSystem bloodSplatter;
    public GameObject impactEffectPrefab;

    [Header("Feedback")]
    public float hitstopDuration = 0.04f;
    public float cameraShakeAmount = 0.15f;
    public float knockbackForce = 4f;
    public bool enableDamageNumbers = true;
    public bool enableScreenFlash = true;
    public bool enableHitSparks = false;

    private bool canAttack = true;
    private int comboCounter = 0;
    private float comboTimer = 0f;

    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (!swordTransform)
        {
            swordTransform = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/PP_Sword_1039");
        }

        if (!attackPoint && swordTransform)
        {
            GameObject attackObj = new GameObject("AttackPoint");
            attackPoint = attackObj.transform;
            attackPoint.SetParent(swordTransform);
            attackPoint.localPosition = new Vector3(0, 0, 1.5f);
        }

        if (swordTrail != null)
            swordTrail.emitting = false;
    }

    private void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                comboCounter = 0;
            }
        }

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        comboCounter++;
        if (comboCounter > maxComboCount)
            comboCounter = 1;

        comboTimer = comboWindow;

        int currentDamage = Mathf.RoundToInt(baseDamage * Mathf.Pow(comboDamageMultiplier, comboCounter - 1));

        if (CombatUI.Instance != null)
        {
            CombatUI.Instance.ShowCombo(comboCounter);
        }

        if (animator)
        {
            if (useComboAnimations && comboCounter <= comboTriggers.Length)
            {
                animator.SetTrigger(comboTriggers[comboCounter - 1]);
            }
            else
            {
                animator.SetTrigger("Attack1");
            }
            animator.speed = attackSpeed;
        }

        PlayWhooshSound();

        if (swordTrail != null)
            swordTrail.emitting = true;

        int comboIndex = Mathf.Clamp(comboCounter - 1, 0, hitDetectionDelays.Length - 1);
        float hitDelay = hitDetectionDelays[comboIndex];
        float recovery = recoveryTimes[comboIndex];

        yield return new WaitForSeconds(hitDelay / attackSpeed);

        DetectAndDamageEnemies(currentDamage);

        yield return new WaitForSeconds(recovery);

        if (swordTrail != null)
            swordTrail.emitting = false;

        if (animator)
            animator.speed = 1f;

        canAttack = true;
    }

    private void DetectAndDamageEnemies(int damage)
    {
        if (!attackPoint) return;

        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        bool hitSomething = false;

        foreach (Collider col in hitColliders)
        {
            Vector3 dirToEnemy = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);

            if (angle > attackAngle) continue;

            hitSomething = true;

            EnemyAI enemy = col.GetComponent<EnemyAI>();
            BossAI boss = col.GetComponent<BossAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, dirToEnemy, knockbackForce * comboCounter);
            }
            else if (boss != null)
            {
                boss.TakeDamage(damage);
            }

            Vector3 hitPosition = col.transform.position + Vector3.up * 1.2f;
            SpawnHitEffects(hitPosition, dirToEnemy);
            
            if (enableDamageNumbers && DamageNumberSpawner.Instance != null)
            {
                DamageNumberSpawner.Instance.SpawnDamageNumber(hitPosition, damage, comboCounter);
            }
            
            if (enableHitSparks)
            {
                HitSpark.Create(hitPosition, -dirToEnemy);
            }
        }

        if (hitSomething)
        {
            ApplyHitFeedback();
        }
    }

    private void SpawnHitEffects(Vector3 position, Vector3 direction)
    {
        if (bloodSplatter != null)
        {
            ParticleSystem blood = Instantiate(bloodSplatter, position, Quaternion.LookRotation(direction));
            Destroy(blood.gameObject, 2f);
        }

        if (impactEffectPrefab != null)
        {
            GameObject impact = Instantiate(impactEffectPrefab, position, Quaternion.identity);
            Destroy(impact, 1f);
        }

        PlayHitSound();
    }

    private void ApplyHitFeedback()
    {
        if (TimeManager.Instance)
            TimeManager.Instance.DoHitstop(hitstopDuration * comboCounter);

        if (CameraShake.Instance)
            CameraShake.Instance.ShakeCamera(cameraShakeAmount * comboCounter, 0.2f);
        
        if (enableScreenFlash && ScreenFlash.Instance)
        {
            float intensity = 0.15f + (comboCounter * 0.05f);
            Color flashColor = new Color(1f, 1f, 1f, intensity);
            ScreenFlash.Instance.Flash(flashColor, 0.08f);
        }
    }

    private void PlayWhooshSound()
    {
        if (whooshSounds != null && whooshSounds.Length > 0 && audioSource)
        {
            AudioClip whoosh = whooshSounds[Random.Range(0, whooshSounds.Length)];
            audioSource.PlayOneShot(whoosh, 0.4f);
        }
    }

    private void PlayHitSound()
    {
        if (hitSounds != null && hitSounds.Length > 0 && audioSource)
        {
            AudioClip hit = hitSounds[Random.Range(0, hitSounds.Length)];
            audioSource.PlayOneShot(hit, 0.7f + (comboCounter * 0.1f));
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * attackRange;
        Vector3 rightBound = Quaternion.Euler(0, attackAngle, 0) * forward;
        Vector3 leftBound = Quaternion.Euler(0, -attackAngle, 0) * forward;

        Gizmos.DrawRay(transform.position, rightBound);
        Gizmos.DrawRay(transform.position, leftBound);
        Gizmos.DrawRay(transform.position, forward);
    }
}
