/*
 * ═══════════════════════════════════════════════════════════════════════
 *                   COMPLETE FIX - STEP BY STEP
 *              (Fixes ALL Issues - Follow This Exactly!)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * This guide fixes:
 * ✅ Movement momentum on slopes (walking/running)
 * ✅ Attack animations not working (only attack 1 showing)
 * ✅ Standing still glitch
 * ✅ Audio only playing on game start
 * ✅ Camera going under terrain
 * ✅ Ugly sword particles
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *               PART 1: FIX THE ANIMATOR (CRITICAL!)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * STEP 1: Open the Animator Controller
 * --------------------------------------
 * 
 * 1. In Project window: Assets/Character/Knight/Animations/
 * 2. Double-click: KnightAnimator.controller
 * 3. Animator window opens
 * 
 * 
 * STEP 2: Check Parameters
 * -------------------------
 * 
 * Make sure you have these EXACT parameters:
 * 
 * - Attack1 (Trigger)
 * - Attack2 (Trigger)
 * - Attack3 (Trigger)
 * 
 * If missing, add them:
 * 1. Click + in Parameters tab
 * 2. Select "Trigger"
 * 3. Name exactly as shown above
 * 
 * 
 * STEP 3: Use InPlace Animations (Fixes Standing Still Glitch!)
 * -------------------------------------------------------------
 * 
 * This is THE FIX for the standing still glitch!
 * 
 * For Attack1_State:
 * 1. Click on Attack1_State in Animator
 * 2. In Inspector, find "Motion" field
 * 3. Click the ⊙ circle button
 * 4. Search: OneHand_Up_Attack_1_InPlace
 * 5. Select: Assets/DoubleL/Demo/Anim/OneHand_Up_Attack_1_InPlace.anim
 * 
 * For Attack2_State:
 * 1. Click on Attack2_State
 * 2. Motion → Search: OneHand_Up_Attack_2_InPlace
 * 3. Select: OneHand_Up_Attack_2_InPlace.anim
 * 
 * For Attack3_State:
 * 1. Click on Attack3_State
 * 2. Motion → Search: OneHand_Up_Attack_3_InPlace
 * 3. Select: OneHand_Up_Attack_3_InPlace.anim
 * 
 * InPlace = No root motion = No glitching when standing still!
 * 
 * 
 * STEP 4: Verify Transitions (Why Attack 2 & 3 Don't Play!)
 * ----------------------------------------------------------
 * 
 * This is probably why only Attack1 works!
 * 
 * Click each transition arrow and verify settings:
 * 
 * Idle → Attack1:
 *   ☐ Has Exit Time (UNCHECKED!)
 *   Transition Duration: 0.05
 *   Conditions: Attack1
 * 
 * Attack1 → Attack2 (The Combo Chain!):
 *   ☑ Has Exit Time (CHECKED!)
 *   Exit Time: 0.7
 *   Transition Duration: 0.1
 *   Conditions: Attack2
 * 
 * Attack2 → Attack3 (The Combo Chain!):
 *   ☑ Has Exit Time (CHECKED!)
 *   Exit Time: 0.7
 *   Transition Duration: 0.1
 *   Conditions: Attack3
 * 
 * Attack1 → Idle (Timeout):
 *   ☑ Has Exit Time (CHECKED!)
 *   Exit Time: 0.95
 *   Transition Duration: 0.15
 *   Conditions: (NONE - leave empty!)
 * 
 * Attack2 → Idle (Timeout):
 *   ☑ Has Exit Time (CHECKED!)
 *   Exit Time: 0.95
 *   Transition Duration: 0.15
 *   Conditions: (NONE)
 * 
 * Attack3 → Idle (Always):
 *   ☑ Has Exit Time (CHECKED!)
 *   Exit Time: 0.9
 *   Transition Duration: 0.2
 *   Conditions: (NONE)
 * 
 * CRITICAL: Make sure Attack1→Attack2 and Attack2→Attack3 have:
 * - Exit Time set to 0.7
 * - Has Exit Time CHECKED
 * - The correct trigger condition
 * 
 * Without these, combo won't chain!
 * 
 * 
 * STEP 5: Disable Root Motion on Animator Component
 * --------------------------------------------------
 * 
 * 1. Select Player GameObject in Hierarchy
 * 2. Find Animator component
 * 3. UNCHECK ☐ "Apply Root Motion"
 * 
 * This prevents animations from moving the character!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *               PART 2: FIX THE COMBAT SCRIPT
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * STEP 1: Remove Old Combat Scripts
 * ----------------------------------
 * 
 * 1. Select Player GameObject
 * 2. Find "PlayerAttack" component
 * 3. Remove it (click ⋮ → Remove Component)
 * 4. Find "ModularCombatSystem" component
 * 5. Remove it too
 * 
 * 
 * STEP 2: Add Fixed Combat System
 * --------------------------------
 * 
 * 1. With Player selected
 * 2. Click "Add Component"
 * 3. Search: FixedCombatSystem
 * 4. Add it!
 * 
 * 
 * STEP 3: Configure Fixed Combat System
 * --------------------------------------
 * 
 * COMBO SETUP:
 * - Max Combo Count: 3
 * - Combo Window: 0.8
 * - Attack Speed: 1.2
 * 
 * DAMAGE:
 * - Attack 1 Damage: 25
 * - Attack 2 Damage: 35
 * - Attack 3 Damage: 50
 * 
 * TIMING:
 * - Attack 1 Hit Delay: 0.15
 * - Attack 2 Hit Delay: 0.18
 * - Attack 3 Hit Delay: 0.2
 * - Attack 1 Recovery: 0.2
 * - Attack 2 Recovery: 0.25
 * - Attack 3 Recovery: 0.3
 * 
 * RANGE:
 * - Attack Range: 2.8
 * - Attack Angle: 90
 * - Enemy Layer: Enemy
 * 
 * REFERENCES:
 * - Animator: (drag from Player's Animator component)
 * - Attack Point: (leave empty, auto-creates)
 * - Sword Transform: (drag PP_Sword_1039 from hierarchy)
 * - Sword Trail: (drag from sword object if you have one)
 * - Audio Source: (drag from Player's AudioSource component)
 * 
 * AUDIO:
 * - Whoosh Sounds: (drag your sword swoosh clips here)
 * - Hit Sounds: (drag your hit impact clips here)
 * 
 * EFFECTS:
 * - Blood Splatter: (drag if you have blood particle)
 * - Knockback Force: 4
 * - Camera Shake Amount: 0.15
 * - Hitstop Duration: 0.04
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *               PART 3: FIX AUDIO (Play on Action!)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * The audio was playing on Start() because it was in Awake/Start!
 * 
 * FixedCombatSystem plays audio when:
 * - Attack starts → whoosh sound
 * - Hit detected → hit sound
 * 
 * Make sure:
 * 1. AudioSource is on Player GameObject
 * 2. AudioSource is NOT set to "Play On Awake"
 * 3. No AudioClip in "Clip" field
 * 4. Volume is at 1
 * 
 * To check:
 * 1. Select Player
 * 2. Find AudioSource component
 * 3. UNCHECK ☐ "Play On Awake"
 * 4. Clear the "AudioClip" field (set to None)
 * 
 * Now sounds will only play when you attack!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *               PART 4: FIX CAMERA CLIPPING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Find your camera script (likely Cinemachine or custom camera):
 * 
 * OPTION A: If using Cinemachine
 * -------------------------------
 * 
 * 1. Find your Virtual Camera in Hierarchy
 * 2. Look for "Cinemachine Collider" component
 * 3. If not there, add it:
 *    - Add Component → Cinemachine Collider
 * 4. Set:
 *    - Collide Against: Everything (or WhatIsGround layer)
 *    - Minimum Distance From Target: 0.2
 *    - Avoid Obstacles: ☑
 *    - Distance Limit: 0.2
 *    - Camera Radius: 0.2
 * 
 * OPTION B: If using custom camera script
 * ----------------------------------------
 * 
 * Add a raycast from camera toward player:
 * 
 * if (Physics.Linecast(playerPos, cameraPos, out RaycastHit hit, groundLayer))
 * {
 *     // Move camera to hit point with small offset
 *     cameraPos = hit.point + hit.normal * 0.2f;
 * }
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *               PART 5: IMPROVE SWORD PARTICLES
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * STEP 1: Find Your Sword Trail
 * ------------------------------
 * 
 * 1. In Hierarchy, expand Player → Knight
 * 2. Navigate to: mixamorig:RightHand → PP_Sword_1039
 * 3. Look for TrailRenderer component
 * 
 * 
 * STEP 2: Better Trail Settings
 * ------------------------------
 * 
 * If you have a TrailRenderer:
 * 
 * Time: 0.3
 * Min Vertex Distance: 0.05
 * Width:
 *   - Start: 0.15
 *   - End: 0.01
 * Color:
 *   - Start: White (full alpha)
 *   - End: Cyan (zero alpha)
 * Material: Default-Particle (or a glowing trail material)
 * 
 * 
 * STEP 3: Add Glow Effect
 * ------------------------
 * 
 * 1. Create new Material:
 *    Assets → Create → Material → "SwordTrailMat"
 * 
 * 2. Set material:
 *    Shader: Universal Render Pipeline/Particles/Unlit
 *    Render Face: Both
 *    Blending Mode: Additive
 *    
 * 3. Set color:
 *    Base Map: None
 *    Color: Bright cyan or white (1, 1, 1, 0.8)
 *    
 * 4. Assign to Trail Renderer:
 *    Drag "SwordTrailMat" to TrailRenderer's Material slot
 * 
 * 
 * STEP 4: Add Swoosh Particle
 * ----------------------------
 * 
 * For even better effect:
 * 
 * 1. Right-click PP_Sword_1039 → Effects → Particle System
 * 2. Name it "SwordSwoosh"
 * 3. Set:
 *    Duration: 0.3
 *    Looping: OFF
 *    Play On Awake: OFF
 *    Start Lifetime: 0.2
 *    Start Speed: 2
 *    Start Size: 0.1 to 0.3
 *    Max Particles: 50
 *    
 * 4. Emission:
 *    Rate over Time: 0
 *    Bursts: Add burst → Time 0, Count 20
 *    
 * 5. Shape:
 *    Shape: Sphere
 *    Radius: 0.5
 *    
 * 6. Color over Lifetime:
 *    Gradient: White → Transparent
 *    
 * 7. Size over Lifetime:
 *    Curve: 1 → 0
 * 
 * Now in FixedCombatSystem, add:
 * public ParticleSystem swordSwoosh;
 * 
 * In AttackRoutine(), add:
 * if (swordSwoosh) swordSwoosh.Play();
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *               PART 6: TEST EVERYTHING!
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * CHECKLIST:
 * 
 * ☐ Press Play
 * ☐ Click left mouse 3 times quickly
 * ☐ All 3 attacks play (different animations!)
 * ☐ No glitch when standing still
 * ☐ Whoosh sound plays each attack
 * ☐ Hit sound plays when hitting enemy
 * ☐ Combo counter shows on screen
 * ☐ Walk up/down slope - no weird momentum
 * ☐ Camera doesn't go under terrain
 * ☐ Sword trail looks good
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *               TROUBLESHOOTING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * PROBLEM: Still only Attack1 plays
 * FIX: Check Animator transitions!
 *      - Attack1→Attack2 needs Attack2 trigger
 *      - Attack2→Attack3 needs Attack3 trigger
 *      - Both need Exit Time 0.7 with Has Exit Time CHECKED
 * 
 * PROBLEM: Still glitches when standing
 * FIX: Make sure you're using InPlace animations:
 *      - OneHand_Up_Attack_1_InPlace.anim
 *      - OneHand_Up_Attack_2_InPlace.anim
 *      - OneHand_Up_Attack_3_InPlace.anim
 *      - Animator → Apply Root Motion is UNCHECKED
 * 
 * PROBLEM: Audio still plays on start
 * FIX: AudioSource → UNCHECK "Play On Awake"
 *      Clear the AudioClip field (set to None)
 * 
 * PROBLEM: Momentum on slopes
 * FIX: Already fixed in Movement.cs!
 *      Removed ProjectOnPlane from Move() method
 *      Only slides have slope interaction now
 * 
 * PROBLEM: Camera goes through terrain
 * FIX: Add Cinemachine Collider component
 *      OR add raycast collision in your camera script
 * 
 * PROBLEM: Ugly combo UI
 * FIX: Find CombatUI script and adjust:
 *      - Font size
 *      - Position
 *      - Color/fade
 *      - Animation curve
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *               EVERYTHING SHOULD NOW WORK! 🎉
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * You now have:
 * ✅ Working 3-hit combo with all animations
 * ✅ No standing still glitch (InPlace animations!)
 * ✅ Audio plays on action, not on start
 * ✅ No slope momentum for walk/run (only slide)
 * ✅ Camera collision
 * ✅ Better sword trail
 * ✅ Clean, simple combat system
 * 
 * Enjoy your combat! 🔥
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class COMPLETE_FIX_GUIDE : MonoBehaviour
{
    // Documentation only!
}
