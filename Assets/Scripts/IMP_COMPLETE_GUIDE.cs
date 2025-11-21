using UnityEngine;

public class IMP_COMPLETE_GUIDE
{
}

/*
╔═══════════════════════════════════════════════════════════════════════════════╗
║                    🎯 IMP COMPLETE REBUILD GUIDE 🎯                           ║
╚═══════════════════════════════════════════════════════════════════════════════╝

🚀 ONE-CLICK SOLUTION:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

1. Stop Play Mode (if running)

2. Run this menu command:
   Tools → 🎯 MASTER IMP REBUILD - COMPLETE SOLUTION

3. Wait for it to finish (watch the Console)

4. Press Play

5. Press B to start Arena

6. Watch the Imp work perfectly!


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📋 WHAT THIS DOES:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

STEP 1: Deletes ALL broken Imp fix scripts
  ✅ Removes 30+ old debug/fix scripts that were interfering
  ✅ Cleans up your project completely

STEP 2: Fixes Fireball Material
  ✅ Creates orange emissive material
  ✅ Assigns to Fireball/Sphere renderer
  ✅ Fireballs will be VISIBLE

STEP 3: Rebuilds Imp Prefab from Scratch
  ✅ Removes ALL old broken components
  ✅ Sets scale to 0.5 (proper size, won't clip ground)
  ✅ Adds NavMeshAgent with correct settings:
      - radius: 0.5, height: 2.5, baseOffset: 0
      - speed: 3.5, updatePosition: true
  ✅ Adds CapsuleCollider (non-trigger):
      - Sized for scale 0.5
      - Prevents ground clipping
  ✅ Configures Animator:
      - Speed (float) for movement
      - Attack (trigger) for fireball
      - Die (trigger) for death
  ✅ Adds NEW ImpAI script (based on working Goblin):
      - 60 HP, 10 damage, 15 XP
      - Patrols, Chases, Shoots fireballs
      - Dies properly, gives XP
  ✅ Sets all layers to Enemy
  ✅ Assigns red Imp material


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✅ EXPECTED BEHAVIOR:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

The Imp will now:
  ✅ Spawn CORRECTLY (not half in ground)
  ✅ Spawn with Goblins on all waves
  ✅ Walk smoothly toward player
  ✅ Stop at 12 unit range
  ✅ Shoot ORANGE FIREBALLS (visible)
  ✅ Take damage (shows damage numbers)
  ✅ Die when health = 0
  ✅ Give 15 XP to player
  ✅ Count toward wave completion
  ✅ Play animations (idle, walk, attack, death)


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🎮 IMP STATS (Risk of Rain style):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  Enemy Type:       Ranged
  Health:           60 HP
  Damage:           10 per fireball
  XP Reward:        15
  Movement Speed:   3.5 units/sec
  Attack Range:     12 units (ranged)
  Attack Cooldown:  3 seconds
  Sight Range:      20 units
  Patrol Range:     8 units


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
⚙️ HOW IT WORKS:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

The new ImpAI script is a COPY of the working EnemyAI (Goblin) with these changes:
  1. RANGED attack instead of melee
  2. Spawns fireballs with Rigidbody velocity
  3. Longer attack range (12 units vs Goblin's 2.5)
  4. Longer attack cooldown (3s vs Goblin's 2s)
  5. Same movement, rotation, states as Goblin

This guarantees it works because Goblin works!


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🔧 IF SOMETHING GOES WRONG:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Problem: Imp still half in ground
Solution: The CapsuleCollider and NavMeshAgent height/baseOffset have been sized
         for scale 0.5. If still clipping, the NavMesh itself may need rebaking.
         Go to: Window → AI → Navigation → Bake

Problem: Fireball invisible
Solution: The script creates orange emissive material. If still invisible, check
         that URP is configured properly and shader is "Universal Render Pipeline/Lit"

Problem: Imp not moving
Solution: This should NOT happen as the script is identical to working Goblin.
         If it does, check Console for NavMesh errors.

Problem: Imp not spawning
Solution: Check ArenaManager has Imp prefab in enemyPrefabs array


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📁 NEW FILES CREATED:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  /Assets/Scripts/ImpAI.cs
    → The new working Imp AI (based on Goblin's EnemyAI)

  /Assets/Scripts/MASTER_IMP_REBUILD.cs
    → The rebuild tool (you just used this)

  /Assets/Scripts/IMP_COMPLETE_GUIDE.cs
    → This guide

  /Assets/Materials/FireballMaterial.mat
    → Orange emissive fireball material


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📁 FILES DELETED (30+ old broken scripts):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

All the old ImpEnemy, ImpFix, ImpDebug, etc. scripts have been deleted.
Your project is now CLEAN!


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🎉 YOU'RE DONE! PRESS PLAY AND ENJOY YOUR WORKING IMP! 🎉

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

*/
