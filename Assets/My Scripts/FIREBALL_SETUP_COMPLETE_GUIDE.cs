/*
 * ═══════════════════════════════════════════════════════════════════════
 *              FIREBALL & IMP SETUP - COMPLETE GUIDE
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * ✅ GOOD NEWS: Your Imp prefab is 90% correct!
 * ✅ Fireball prefab exists and is assigned!
 * ✅ ImpEnemy script is configured!
 * 
 * ⚠️ NEEDS FIX: Fireball Sphere needs a material
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                  STEP 1: CREATE FIREBALL MATERIAL
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. In Project window: Right-click in Assets/Materials folder
 *    (Or create Materials folder if it doesn't exist)
 * 
 * 2. Create → Material
 * 
 * 3. Name it: "Fireball_Material"
 * 
 * 4. In Inspector, configure:
 * 
 *    ┌────────────────────────────────────────────────┐
 *    │ SHADER SETTINGS                                │
 *    ├────────────────────────────────────────────────┤
 *    │ Shader: Universal Render Pipeline → Lit        │
 *    │                                                 │
 *    │ BASE COLOR                                      │
 *    │ - Base Color: Orange/Red (RGB: 255, 100, 0)   │
 *    │ - Or use color picker for bright orange        │
 *    │                                                 │
 *    │ EMISSION (Makes it glow!)                      │
 *    │ - Scroll down to Emission section              │
 *    │ - Check "Emission" checkbox                    │
 *    │ - Emission Color: Bright Orange (255, 100, 0)  │
 *    │ - Or click HDR color and increase intensity    │
 *    │                                                 │
 *    │ OPTIONAL - For more glow:                      │
 *    │ - Metallic: 0                                  │
 *    │ - Smoothness: 0.5                              │
 *    └────────────────────────────────────────────────┘
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *              STEP 2: ASSIGN MATERIAL TO FIREBALL
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. In Project: Assets/Goblin_Character/Prefab/Fireball
 * 
 * 2. Double-click to open prefab
 * 
 * 3. In Hierarchy (prefab mode), select: Fireball → Sphere
 * 
 * 4. In Inspector, find "Mesh Renderer" component
 * 
 * 5. Expand "Materials" section
 * 
 * 6. Drag "Fireball_Material" into the Element 0 slot
 * 
 * 7. Save prefab (Ctrl+S or File → Save)
 * 
 * 8. Exit prefab mode (click arrow at top left)
 * 
 * RESULT: Fireball should now glow orange/red!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *           STEP 3: VERIFY IMP PREFAB (ALREADY DONE!)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Your Imp prefab already has:
 * ✅ Tag: "Enemy"
 * ✅ NavMeshAgent component
 * ✅ ImpEnemy script
 * ✅ Animator reference
 * ✅ Fireball prefab assigned
 * ✅ Correct layer masks (WhatIsGround, WhatIsPlayer)
 * 
 * ONLY MISSING (Optional):
 * - AudioSource component
 * - Audio clips (attack, hurt, death, aggro sounds)
 * - Death effect prefab
 * - Blood particle effect
 * 
 * These are OPTIONAL - Imp will work without them!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *          STEP 4: ADD OPTIONAL COMPONENTS (IF YOU WANT)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * A) ADD AUDIO (Optional):
 * ------------------------
 * 
 * 1. Select Imp prefab
 * 2. Add Component → Audio → Audio Source
 * 3. In ImpEnemy script, drag AudioSource into "Audio Source" field
 * 4. If you have sound effects, assign them to:
 *    - Attack Sound
 *    - Hurt Sound
 *    - Death Sound
 *    - Aggro Sound
 * 
 * 
 * B) ADD CAPSULE COLLIDER (Recommended):
 * ---------------------------------------
 * 
 * 1. Select Imp prefab
 * 2. Add Component → Physics → Capsule Collider
 * 3. Configure:
 *    - Center: (0, 1, 0)
 *    - Radius: 0.5
 *    - Height: 2
 * 4. This helps with physics interactions
 * 
 * 
 * C) ADD LIGHT TO FIREBALL (Makes it look better):
 * -------------------------------------------------
 * 
 * 1. Open Fireball prefab
 * 2. Select Fireball root
 * 3. Add Component → Rendering → Light
 * 4. Configure:
 *    - Type: Point
 *    - Color: Orange (255, 100, 0)
 *    - Range: 5
 *    - Intensity: 2
 * 5. Save prefab
 * 
 * RESULT: Fireball will light up the environment as it flies!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                   STEP 5: TEST YOUR IMP
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. Make sure ArenaManager has Imp in Enemy Prefabs array
 *    (See QUICK_SETUP_GUIDE.cs Step 6)
 * 
 * 2. Press Play
 * 
 * 3. Press B to start arena
 * 
 * 4. Watch for Imps to spawn
 * 
 * 5. Verify:
 *    ✅ Imp appears (not pink)
 *    ✅ Imp chases you
 *    ✅ Imp stops at range (~12m)
 *    ✅ Imp shoots fireballs
 *    ✅ Fireballs glow orange/red
 *    ✅ Fireballs damage you on hit
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                      TROUBLESHOOTING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * PROBLEM: Fireball still invisible/no material
 * FIX: 
 *   1. Make sure you saved the Fireball prefab after assigning material
 *   2. Check Sphere has MeshRenderer component
 *   3. Make sure material uses URP shader, not Built-in
 * 
 * PROBLEM: Imp doesn't shoot fireballs
 * FIX:
 *   1. Check fireballPrefab is assigned in ImpEnemy script
 *   2. Make sure Fireball prefab has Projectile script
 *   3. Check Console for errors
 * 
 * PROBLEM: Fireballs don't damage player
 * FIX:
 *   1. Make sure Player has PlayerHealth script
 *   2. Make sure Player tag is "Player"
 *   3. Make sure Fireball SphereCollider is "Is Trigger" = true
 * 
 * PROBLEM: Imp appears pink
 * FIX: Use the "Fix Imp Material" tool from earlier
 * 
 * PROBLEM: Imp doesn't move
 * FIX:
 *   1. Make sure NavMesh is baked (see below)
 *   2. Make sure Imp is on NavMesh when spawned
 * 
 * PROBLEM: Fireballs fall to ground
 * FIX:
 *   1. Select Fireball prefab
 *   2. Find Rigidbody component
 *   3. Make sure "Use Gravity" = FALSE
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *              IMPORTANT: NAVMESH FOR IMP TO WALK
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Imp uses NavMeshAgent to move, so you need NavMesh baked:
 * 
 * 1. Window → AI → Navigation (Legacy)
 * 
 * 2. Select your ground/terrain objects
 * 
 * 3. In Navigation window, Object tab:
 *    - Check "Navigation Static"
 * 
 * 4. Switch to Bake tab
 * 
 * 5. Click "Bake" button at bottom
 * 
 * 6. Blue overlay shows walkable areas
 * 
 * If you already did this for goblins, it's already done!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                  FIREBALL MATERIAL VARIATIONS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Want different fireball colors?
 * 
 * RED FIREBALL (Hell theme):
 * - Base Color: (255, 0, 0) Bright Red
 * - Emission: (255, 50, 0) Orange-Red
 * 
 * BLUE FIREBALL (Ice/Magic theme):
 * - Base Color: (0, 100, 255) Blue
 * - Emission: (100, 150, 255) Light Blue
 * 
 * GREEN FIREBALL (Poison theme):
 * - Base Color: (0, 255, 100) Green
 * - Emission: (100, 255, 100) Bright Green
 * 
 * PURPLE FIREBALL (Dark magic):
 * - Base Color: (200, 0, 255) Purple
 * - Emission: (255, 0, 255) Magenta
 * 
 * Just create different materials and assign to different fireballs!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *              CURRENT IMP PREFAB STATUS: ✅ 90% READY!
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * WHAT YOU HAVE:
 * ✅ Imp prefab exists
 * ✅ ImpEnemy script attached and configured
 * ✅ NavMeshAgent configured (Speed: 3.5)
 * ✅ Animator assigned
 * ✅ Tag: "Enemy" set
 * ✅ Layers: WhatIsGround (128), WhatIsPlayer (64) set
 * ✅ Stats configured: 60 HP, 10 damage, 15 XP
 * ✅ Fireball prefab assigned
 * ✅ Fireball has: SphereCollider (trigger), Rigidbody (no gravity), Projectile script
 * 
 * WHAT'S MISSING:
 * ⚠️ Fireball Sphere material (DO STEP 1 & 2 above)
 * ⚠️ AudioSource (optional - Imp works without it)
 * ⚠️ CapsuleCollider (optional but recommended)
 * ⚠️ Fireball SpawnPoint (script auto-creates one if missing)
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                  SIMPLIFIED: MINIMUM TO GET WORKING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * JUST DO THIS:
 * 
 * 1. Create Material (orange, emissive)
 * 2. Assign to Fireball → Sphere
 * 3. Save Fireball prefab
 * 4. Add Imp to ArenaManager Enemy Prefabs array
 * 5. Test!
 * 
 * That's it! Imp will work and shoot fireballs!
 * 
 * Everything else (audio, effects, lights) is optional polish.
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                        QUICK CHECKLIST
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * IMP PREFAB:
 * [✅] Tag: "Enemy"
 * [✅] NavMeshAgent component
 * [✅] ImpEnemy script
 * [✅] Animator assigned
 * [✅] Fireball prefab assigned
 * [✅] Layer masks set
 * [⚠️] AudioSource (optional)
 * [⚠️] CapsuleCollider (recommended)
 * 
 * FIREBALL PREFAB:
 * [✅] SphereCollider (Is Trigger = true)
 * [✅] Rigidbody (Use Gravity = false)
 * [✅] Projectile script
 * [⚠️] Sphere material (NEEDS FIX - do Step 1-2)
 * [⚠️] Light component (optional, makes it pretty)
 * 
 * SCENE SETUP:
 * [✅] NavMesh baked (if you have goblins working)
 * [⚠️] Imp in ArenaManager.enemyPrefabs array
 * [⚠️] Spawn weight in ArenaManager.enemySpawnWeights array
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                         SUMMARY
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * ✅ YOUR IMP SETUP IS ALMOST PERFECT!
 * 
 * Only 1 thing needed: Create and assign fireball material (2 minutes)
 * 
 * Everything else is optional polish that can be added later.
 * 
 * The Imp will shoot fireballs and work correctly once you:
 * 1. Create orange emissive material
 * 2. Assign to Fireball/Sphere
 * 3. Add Imp to ArenaManager
 * 
 * You're doing great! Almost ready to test! 🔥
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class FIREBALL_SETUP_COMPLETE_GUIDE : MonoBehaviour
{
    // Your Imp is 90% ready!
    // Just create fireball material and you're done!
}
