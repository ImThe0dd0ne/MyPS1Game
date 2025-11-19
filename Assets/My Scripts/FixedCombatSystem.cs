using UnityEngine;
using System.Collections;

public class FixedCombatSystem : MonoBehaviour
{
    [Header("=== COMBO SETUP ===")]
    public int maxComboCount = 3;
    public float comboWindow = 0.8f;
    public float attackSpeed = 1.2f;
    
    [Header("=== DAMAGE ===")]
    public float attack1Damage = 25f;
    public float attack2Damage = 35f;
    public float attack3Damage = 50f;
    
    [Header("=== TIMING ===")]
    public float attack1HitDelay = 0.15f;
    public float attack2HitDelay = 0.18f;
    public float attack3HitDelay = 0.2f;
    
    public float attack1Recovery = 0.2f;
    public float attack2Recovery = 0.25f;
    public float attack3Recovery = 0.3f;
    
    [Header("=== RANGE ===")]
    public float attackRange = 2.8f;
    public float attackAngle = 90f;
    public LayerMask enemyLayer;
    
    [Header("=== REFERENCES ===")]
    public Animator animator;
    public Transform attackPoint;
    public Transform swordTransform;
    public TrailRenderer swordTrail;
    public AudioSource audioSource;
    
    [Header("=== AUDIO ===")]
    public AudioClip[] whooshSounds;
    public AudioClip[] hitSounds;
    
    [Header("=== EFFECTS ===")]
    public ParticleSystem swordSlashEffect;
    public ParticleSystem bloodSplatter;
    public float knockbackForce = 4f;
    public float cameraShakeAmount = 0.15f;
    public float hitstopDuration = 0.04f;
    
    private int currentCombo = 0;
    private float comboTimer = 0f;
    private bool isAttacking = false;
    private bool attackQueued = false;
    
    void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        
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
            
        if (animator)
            animator.applyRootMotion = false;
    }
    
    void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                currentCombo = 0;
                if (CombatUI.Instance) CombatUI.Instance.ResetCombo();
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                PerformAttack();
            }
            else
            {
                attackQueued = true;
            }
        }
    }
    
    void PerformAttack()
    {
        currentCombo++;
        if (currentCombo > maxComboCount)
            currentCombo = 1;
        
        comboTimer = comboWindow;
        
        if (CombatUI.Instance)
            CombatUI.Instance.ShowCombo(currentCombo);
        
        StartCoroutine(AttackRoutine(currentCombo));
    }
    
    IEnumerator AttackRoutine(int comboNumber)
    {
        isAttacking = true;
        attackQueued = false;
        
        string trigger = "Attack" + comboNumber;
        float hitDelay = 0.15f;
        float recovery = 0.2f;
        float damage = attack1Damage;
        
        switch (comboNumber)
        {
            case 1:
                hitDelay = attack1HitDelay;
                recovery = attack1Recovery;
                damage = attack1Damage;
                break;
            case 2:
                hitDelay = attack2HitDelay;
                recovery = attack2Recovery;
                damage = attack2Damage;
                break;
            case 3:
                hitDelay = attack3HitDelay;
                recovery = attack3Recovery;
                damage = attack3Damage;
                break;
        }
        
        if (animator)
        {
            animator.SetTrigger(trigger);
            animator.speed = attackSpeed;
        }
        
        PlayWhooshSound();
        
        if (swordTrail)
            swordTrail.emitting = true;
        
        if (swordSlashEffect)
            swordSlashEffect.Play();
        
        yield return new WaitForSeconds(hitDelay / attackSpeed);
        
        DetectHit(Mathf.RoundToInt(damage), comboNumber);
        
        yield return new WaitForSeconds(recovery / attackSpeed);
        
        if (swordTrail)
            swordTrail.emitting = false;
        
        if (animator)
            animator.speed = 1f;
        
        isAttacking = false;
        
        if (attackQueued)
        {
            PerformAttack();
        }
    }
    
    void DetectHit(int damage, int comboNumber)
    {
        if (!attackPoint) return;
        
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        
        bool hitSomething = false;
        
        foreach (Collider col in hits)
        {
            Vector3 dirToEnemy = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);
            
            if (angle > attackAngle) continue;
            
            hitSomething = true;
            
            EnemyAI enemy = col.GetComponent<EnemyAI>();
            BossAI boss = col.GetComponent<BossAI>();
            
            if (enemy != null)
            {
                float knockback = knockbackForce * comboNumber;
                enemy.TakeDamage(damage, dirToEnemy, knockback);
            }
            else if (boss != null)
            {
                boss.TakeDamage(damage);
            }
            
            Vector3 hitPos = col.transform.position + Vector3.up * 1.2f;
            
            if (bloodSplatter)
            {
                ParticleSystem blood = Instantiate(bloodSplatter, hitPos, Quaternion.LookRotation(dirToEnemy));
                Destroy(blood.gameObject, 2f);
            }
            
            if (DamageNumberSpawner.Instance)
            {
                DamageNumberSpawner.Instance.SpawnDamageNumber(hitPos, damage, comboNumber);
            }
            
            PlayHitSound(comboNumber);
        }
        
        if (hitSomething)
        {
            if (TimeManager.Instance)
                TimeManager.Instance.DoHitstop(hitstopDuration * comboNumber);
            
            if (CameraShake.Instance)
                CameraShake.Instance.ShakeCamera(cameraShakeAmount * comboNumber, 0.2f);
            
            if (ScreenFlash.Instance)
            {
                float intensity = 0.15f + (comboNumber * 0.05f);
                ScreenFlash.Instance.Flash(new Color(1f, 1f, 1f, intensity), 0.08f);
            }
        }
    }
    
    void PlayWhooshSound()
    {
        if (whooshSounds != null && whooshSounds.Length > 0 && audioSource)
        {
            AudioClip clip = whooshSounds[Random.Range(0, whooshSounds.Length)];
            audioSource.PlayOneShot(clip, 0.4f);
        }
    }
    
    void PlayHitSound(int comboNumber)
    {
        if (hitSounds != null && hitSounds.Length > 0 && audioSource)
        {
            AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
            float volume = 0.7f + (comboNumber * 0.1f);
            audioSource.PlayOneShot(clip, volume);
        }
    }
    
    public void ForceReset()
    {
        StopAllCoroutines();
        isAttacking = false;
        attackQueued = false;
        currentCombo = 0;
        comboTimer = 0f;
        
        if (swordTrail)
            swordTrail.emitting = false;
        
        if (animator)
            animator.speed = 1f;
        
        if (CombatUI.Instance)
            CombatUI.Instance.ResetCombo();
    }
    
    public bool IsAttacking()
    {
        return isAttacking;
    }
    
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * attackRange;
        Vector3 rightBound = Quaternion.Euler(0, attackAngle, 0) * forward;
        Vector3 leftBound = Quaternion.Euler(0, -attackAngle, 0) * forward;
        
        Gizmos.DrawRay(transform.position, forward);
        Gizmos.DrawRay(transform.position, rightBound);
        Gizmos.DrawRay(transform.position, leftBound);
    }
}
