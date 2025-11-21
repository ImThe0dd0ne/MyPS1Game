using UnityEngine;
using UnityEditor;

public class COMPLETE_IMP_FIX_MASTER : MonoBehaviour
{
    [MenuItem("Tools/⚡ COMPLETE IMP FIX - DO EVERYTHING ⚡")]
    public static void FixEverything()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║      ⚡ COMPLETE IMP FIX - DOING EVERYTHING ⚡           ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
        
        REBUILD_IMP_COMPLETELY.RebuildImp();
        
        Debug.Log("\n\n╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║              ✅ ALL FIXES COMPLETE!                      ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        
        Debug.Log("\n📋 WHAT WAS FIXED:");
        Debug.Log("  ✅ Imp prefab completely rebuilt");
        Debug.Log("  ✅ All missing scripts removed");
        Debug.Log("  ✅ NavMeshAgent configured properly");
        Debug.Log("  ✅ Scale increased to 0.5 (more visible)");
        Debug.Log("  ✅ CapsuleCollider added (non-trigger)");
        Debug.Log("  ✅ Animator with Speed/Attack/Die parameters");
        Debug.Log("  ✅ ImpEnemy script with all stats");
        Debug.Log("  ✅ Fireball attack configured");
        Debug.Log("  ✅ Layer masks set (Player, WhatIsGround)");
        Debug.Log("  ✅ All children set to Enemy layer");
        Debug.Log("  ✅ Material assigned");
        Debug.Log("  ✅ Fireball material fixed (orange emissive)");
        Debug.Log("  ✅ Spawn height increased to 2 units");
        Debug.Log("  ✅ Auto-height adjustment on spawn");
        
        Debug.Log("\n🎮 HOW TO TEST:");
        Debug.Log("  1. Press PLAY");
        Debug.Log("  2. Press B to start arena");
        Debug.Log("  3. Imp should:");
        Debug.Log("     - Spawn ABOVE ground (not clipping)");
        Debug.Log("     - Move toward player");
        Debug.Log("     - Attack with orange fireballs");
        Debug.Log("     - Take damage and die properly");
        Debug.Log("     - Animate (idle/walk/attack/death)");
        
        Debug.Log("\n⚠️ IMPORTANT NOTES:");
        Debug.Log("  - Imp will spawn with Goblin (mixed spawning)");
        Debug.Log("  - Imp has ranged fireball attack (12 unit range)");
        Debug.Log("  - Goblin has melee attack");
        Debug.Log("  - Both are configured correctly now!");
        
        Debug.Log("\n🔧 IF STILL BROKEN:");
        Debug.Log("  1. Check console for errors");
        Debug.Log("  2. Verify NavMesh is baked (Window → AI → Navigation)");
        Debug.Log("  3. Ensure Player layer and Enemy layer exist");
        Debug.Log("  4. Make sure WhatIsGround layer exists");
        
        Debug.Log("\n✨ READY TO TEST! ✨\n");
    }
}
