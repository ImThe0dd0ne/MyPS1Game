using UnityEngine;
using UnityEngine.AI;

public class ImpRuntimeDebug : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private ImpEnemy impScript;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        impScript = GetComponent<ImpEnemy>();
        
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log($"║  IMP RUNTIME DEBUG - {gameObject.name}");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        
        Debug.Log($"Position: {transform.position}");
        Debug.Log($"Scale: {transform.localScale}");
        Debug.Log($"Layer: {LayerMask.LayerToName(gameObject.layer)} ({gameObject.layer})");
        
        if (agent != null)
        {
            Debug.Log($"\n<color=cyan>NavMeshAgent Status:</color>");
            Debug.Log($"  isOnNavMesh: {agent.isOnNavMesh} {(agent.isOnNavMesh ? "✅" : "❌ CRITICAL!")}");
            Debug.Log($"  enabled: {agent.enabled}");
            Debug.Log($"  radius: {agent.radius}");
            Debug.Log($"  height: {agent.height}");
            Debug.Log($"  baseOffset: {agent.baseOffset}");
            Debug.Log($"  speed: {agent.speed}");
            Debug.Log($"  stoppingDistance: {agent.stoppingDistance}");
            
            if (!agent.isOnNavMesh)
            {
                Debug.LogError("❌❌❌ IMP IS NOT ON NAVMESH!");
                Debug.LogError("This is why it's not moving!");
                Debug.LogError("Attempting to place on NavMesh...");
                
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                    Debug.Log($"✅ Moved Imp to NavMesh position: {hit.position}");
                }
                else
                {
                    Debug.LogError("❌ Could not find nearby NavMesh! NavMesh is not baked or spawn point is too far from NavMesh!");
                }
            }
        }
        else
        {
            Debug.LogError("❌ NavMeshAgent component MISSING!");
        }
        
        if (animator != null)
        {
            Debug.Log($"\n<color=yellow>Animator Status:</color>");
            Debug.Log($"  Controller: {(animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "NULL ❌")}");
            Debug.Log($"  Apply Root Motion: {animator.applyRootMotion}");
            Debug.Log($"  enabled: {animator.enabled}");
            
            if (animator.runtimeAnimatorController != null)
            {
                Debug.Log($"  Parameters ({animator.parameterCount}):");
                foreach (var param in animator.parameters)
                {
                    Debug.Log($"    - {param.name} ({param.type})");
                }
            }
        }
        else
        {
            Debug.LogError("❌ Animator component MISSING!");
        }
        
        if (impScript != null)
        {
            Debug.Log($"\n<color=green>ImpEnemy Script:</color>");
            Debug.Log($"  agent: {(impScript.agent != null ? "✅" : "❌ NULL")}");
            Debug.Log($"  animator: {(impScript.animator != null ? "✅" : "❌ NULL")}");
            Debug.Log($"  player: {(impScript.player != null ? "✅" : "❌ NULL")}");
            Debug.Log($"  fireballPrefab: {(impScript.fireballPrefab != null ? "✅" : "❌ NULL")}");
            
            Debug.Log($"  whatIsPlayer mask: {impScript.whatIsPlayer.value}");
            Debug.Log($"  whatIsGround mask: {impScript.whatIsGround.value}");
            
            Debug.Log($"  sightRange: {impScript.sightRange}");
            Debug.Log($"  attackRange: {impScript.attackRange}");
        }
        else
        {
            Debug.LogError("❌ ImpEnemy script MISSING!");
        }
        
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Debug.Log($"\n<color=magenta>Renderers ({renderers.Length}):</color>");
        foreach (Renderer r in renderers)
        {
            Debug.Log($"  {r.name}:");
            foreach (Material mat in r.sharedMaterials)
            {
                if (mat != null)
                {
                    Debug.Log($"    - Material: {mat.name}");
                    Texture tex = mat.GetTexture("_BaseMap");
                    if (tex != null)
                    {
                        Debug.Log($"      Texture: {tex.name} ✅");
                    }
                    else
                    {
                        Debug.LogWarning($"      No _BaseMap texture! ⚠️");
                    }
                }
                else
                {
                    Debug.LogError($"    - NULL MATERIAL ❌");
                }
            }
        }
        
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
    }
    
    private float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= 2f && agent != null)
        {
            timer = 0f;
            
            if (!agent.isOnNavMesh)
            {
                Debug.LogError($"⚠️ {gameObject.name} STILL NOT ON NAVMESH after 2 seconds!");
            }
            else if (agent.velocity.magnitude < 0.01f)
            {
                Debug.LogWarning($"⚠️ {gameObject.name} is on NavMesh but NOT MOVING (velocity: {agent.velocity.magnitude})");
                Debug.LogWarning($"   hasPath: {agent.hasPath}, pathPending: {agent.pathPending}, isStopped: {agent.isStopped}");
            }
        }
    }
}
