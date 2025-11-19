using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ComboAttack
{
    public string animationTrigger = "Attack1";
    public float damage = 25f;
    public float hitDetectionDelay = 0.15f;
    public float recoveryTime = 0.2f;
    public float attackRange = 2.8f;
    public float attackAngle = 90f;
    public float knockbackMultiplier = 1f;
    public float cameraShakeMultiplier = 1f;
    public AudioClip[] customWhooshSounds;
}

public class ModularCombatSystem : MonoBehaviour
{
    [Header("=== COMBO SYSTEM ===")]
    [Tooltip("All attacks in the combo chain")]
    public ComboAttack[] comboChain = new ComboAttack[3];
    
    [Tooltip("Time window to continue combo")]
    public float comboWindow = 0.8f;
    
    [Tooltip("Animation speed multiplier")]
    public float attackSpeed = 1.2f;
    
    [Tooltip("Enemies layer mask")]
    public LayerMask enemyLayer;

    [Header("=== MODULAR SETTINGS ===")]
    [Tooltip("Allow attacking while moving")]
    public bool canAttackWhileMoving = true;
    
    [Tooltip("Root motion handling for animations")]
    public bool useRootMotion = false;
    
    [Tooltip("Queue next attack input")]
    public bool allowInputBuffering = true;
    
    [Tooltip("Input buffer window in seconds")]
    public float inputBufferTime = 0.15f;

    [Header("=== REFERENCES ===")]
    public Transform swordTransform;
    public Transform attackPoint;
    public Animator animator;
    public TrailRenderer swordTrail;
    public CharacterController characterController;
    public ThirdPersonPlayer movementController;

    [Header("=== AUDIO ===")]
    public AudioClip[] defaultWhooshSounds;
    public AudioClip[] hitSounds;
    private AudioSource audioSource;

    [Header("=== VISUAL EFFECTS ===")]
    public ParticleSystem bloodSplatter;
    public GameObject impactEffectPrefab;

    [Header("=== FEEDBACK ===")]
    public float baseHitstopDuration = 0.04f;
    public float baseCameraShake = 0.15f;
    public float baseKnockbackForce = 4f;
    public bool enableDamageNumbers = true;
    public bool enableScreenFlash = true;
    public bool enableHitSparks = false;

    private int currentComboIndex = 0;
    private float comboTimer = 0f;
    private bool isAttacking = false;
    private bool attackQueued = false;
    private float inputBufferTimer = 0f;
    private Vector3 attackMoveDirection;

    void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (!characterController) characterController = GetComponent<CharacterController>();
        if (!movementController) movementController = GetComponent<ThirdPersonPlayer>();

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

        InitializeDefaultCombo();
        
        if (animator && !useRootMotion)
        {
            animator.applyRootMotion = false;
        }
    }

    void InitializeDefaultCombo()
    {
        if (comboChain.Length == 0)
        {
            comboChain = new ComboAttack[3];
        }

        for (int i = 0; i < comboChain.Length; i++)
        {
            if (comboChain[i] == null)
            {
                comboChain[i] = new ComboAttack();
                comboChain[i].animationTrigger = "Attack" + (i + 1);
                comboChain[i].damage = 25f * (1f + i * 0.4f);
                comboChain[i].hitDetectionDelay = 0.15f + (i * 0.03f);
                comboChain[i].recoveryTime = 0.2f + (i * 0.05f);
                comboChain[i].attackRange = 2.8f;
                comboChain[i].attackAngle = 90f;
                comboChain[i].knockbackMultiplier = 1f + (i * 0.5f);
                comboChain[i].cameraShakeMultiplier = 1f + (i * 0.3f);
            }
        }
    }

    void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }

        if (inputBufferTimer > 0f)
        {
            inputBufferTimer -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                ExecuteAttack();
            }
            else if (allowInputBuffering)
            {
                attackQueued = true;
                inputBufferTimer = inputBufferTime;
            }
        }
    }

    void ExecuteAttack()
    {
        if (currentComboIndex >= comboChain.Length)
        {
            currentComboIndex = 0;
        }

        comboTimer = comboWindow;
        attackMoveDirection = transform.forward;

        StartCoroutine(PerformAttack(comboChain[currentComboIndex], currentComboIndex));
        
        currentComboIndex++;
        if (currentComboIndex >= comboChain.Length)
        {
            currentComboIndex = 0;
        }
    }

    IEnumerator PerformAttack(ComboAttack attack, int comboNumber)
    {
        isAttacking = true;
        attackQueued = false;

        if (CombatUI.Instance != null)
        {
            CombatUI.Instance.ShowCombo(comboNumber + 1);
        }

        if (animator)
        {
            animator.SetTrigger(attack.animationTrigger);
            animator.speed = attackSpeed;
        }

        PlayWhooshSound(attack);

        if (swordTrail != null)
            swordTrail.emitting = true;

        float adjustedHitDelay = attack.hitDetectionDelay / attackSpeed;
        yield return new WaitForSeconds(adjustedHitDelay);

        PerformHitDetection(attack, comboNumber + 1);

        float adjustedRecovery = attack.recoveryTime / attackSpeed;
        yield return new WaitForSeconds(adjustedRecovery);

        if (swordTrail != null)
            swordTrail.emitting = false;

        if (animator)
            animator.speed = 1f;

        isAttacking = false;

        if (attackQueued && inputBufferTimer > 0f)
        {
            ExecuteAttack();
        }
    }

    void PerformHitDetection(ComboAttack attack, int comboNumber)
    {
        if (!attackPoint) return;

        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attack.attackRange, enemyLayer);

        bool hitSomething = false;
        List<Collider> alreadyHit = new List<Collider>();

        foreach (Collider col in hitColliders)
        {
            if (alreadyHit.Contains(col)) continue;

            Vector3 dirToEnemy = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);

            if (angle > attack.attackAngle) continue;

            alreadyHit.Add(col);
            hitSomething = true;

            int damage = Mathf.RoundToInt(attack.damage);
            float knockback = baseKnockbackForce * attack.knockbackMultiplier;

            EnemyAI enemy = col.GetComponent<EnemyAI>();
            BossAI boss = col.GetComponent<BossAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, dirToEnemy, knockback);
            }
            else if (boss != null)
            {
                boss.TakeDamage(damage);
            }

            Vector3 hitPosition = col.transform.position + Vector3.up * 1.2f;
            SpawnHitEffects(hitPosition, dirToEnemy, attack, comboNumber);
        }

        if (hitSomething)
        {
            ApplyHitFeedback(attack, comboNumber);
        }
    }

    void SpawnHitEffects(Vector3 position, Vector3 direction, ComboAttack attack, int comboNumber)
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

        if (enableDamageNumbers && DamageNumberSpawner.Instance != null)
        {
            int damage = Mathf.RoundToInt(attack.damage);
            DamageNumberSpawner.Instance.SpawnDamageNumber(position, damage, comboNumber);
        }

        if (enableHitSparks)
        {
            HitSpark.Create(position, -direction);
        }

        PlayHitSound(comboNumber);
    }

    void ApplyHitFeedback(ComboAttack attack, int comboNumber)
    {
        if (TimeManager.Instance)
        {
            float hitstop = baseHitstopDuration * attack.cameraShakeMultiplier;
            TimeManager.Instance.DoHitstop(hitstop);
        }

        if (CameraShake.Instance)
        {
            float shake = baseCameraShake * attack.cameraShakeMultiplier;
            CameraShake.Instance.ShakeCamera(shake, 0.2f);
        }

        if (enableScreenFlash && ScreenFlash.Instance)
        {
            float intensity = 0.15f + (comboNumber * 0.05f);
            Color flashColor = new Color(1f, 1f, 1f, intensity);
            ScreenFlash.Instance.Flash(flashColor, 0.08f);
        }
    }

    void PlayWhooshSound(ComboAttack attack)
    {
        if (!audioSource) return;

        AudioClip[] soundsToUse = attack.customWhooshSounds != null && attack.customWhooshSounds.Length > 0 
            ? attack.customWhooshSounds 
            : defaultWhooshSounds;

        if (soundsToUse != null && soundsToUse.Length > 0)
        {
            AudioClip whoosh = soundsToUse[Random.Range(0, soundsToUse.Length)];
            audioSource.PlayOneShot(whoosh, 0.4f);
        }
    }

    void PlayHitSound(int comboNumber)
    {
        if (hitSounds != null && hitSounds.Length > 0 && audioSource)
        {
            AudioClip hit = hitSounds[Random.Range(0, hitSounds.Length)];
            float volume = 0.7f + (comboNumber * 0.1f);
            audioSource.PlayOneShot(hit, volume);
        }
    }

    void ResetCombo()
    {
        currentComboIndex = 0;
        
        if (CombatUI.Instance != null)
        {
            CombatUI.Instance.ResetCombo();
        }
    }

    public void ForceResetCombo()
    {
        ResetCombo();
        isAttacking = false;
        attackQueued = false;
        comboTimer = 0f;
        
        if (swordTrail != null)
            swordTrail.emitting = false;
        
        if (animator)
            animator.speed = 1f;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public int GetCurrentComboIndex()
    {
        return currentComboIndex;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        for (int i = 0; i < comboChain.Length; i++)
        {
            if (comboChain[i] == null) continue;

            Gizmos.color = new Color(1f, 0f, 0f, 0.3f + (i * 0.2f));
            Gizmos.DrawWireSphere(attackPoint.position, comboChain[i].attackRange);
        }

        if (comboChain.Length > 0 && comboChain[0] != null)
        {
            Gizmos.color = Color.yellow;
            float range = comboChain[0].attackRange;
            float angle = comboChain[0].attackAngle;
            
            Vector3 forward = transform.forward * range;
            Vector3 rightBound = Quaternion.Euler(0, angle, 0) * forward;
            Vector3 leftBound = Quaternion.Euler(0, -angle, 0) * forward;

            Gizmos.DrawRay(transform.position, rightBound);
            Gizmos.DrawRay(transform.position, leftBound);
            Gizmos.DrawRay(transform.position, forward);
        }
    }
}
