/*
 * ═══════════════════════════════════════════════════════════════════════
 *              IMP ANIMATOR CONTROLLER - SETUP GUIDE
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * PROBLEM: Imp Animator Controller is missing or not assigned
 * 
 * RESULT: Imp won't play animations (idle, walk, attack, death)
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                  AUTOMATIC FIX (RECOMMENDED)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * OPTION 1: Complete Imp Fix (includes animator)
 * ------------------------------------------------
 * 
 * Unity menu: Tools → FIX IMP - Scale + Material + Movement (COMPLETE)
 * 
 * This fixes EVERYTHING including:
 * ✅ Scale
 * ✅ Material
 * ✅ Fireball
 * ✅ Animator Controller ← This!
 * 
 * 
 * OPTION 2: Just Setup Animator Controller
 * ------------------------------------------
 * 
 * Unity menu: Tools → Setup Imp Animator Controller
 * 
 * This will:
 * ✅ Setup animator states (Idle, Move, Attack, Dead, Take Damage)
 * ✅ Setup parameters (Speed, Attack, Die, TakeDamage)
 * ✅ Create transitions between states
 * ✅ Assign controller to Imp prefab
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    MANUAL SETUP (IF AUTOMATIC FAILS)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * STEP 1: Assign Controller to Prefab
 * -------------------------------------
 * 
 * 1. Select: Assets/Prefabs/Imp
 * 
 * 2. Find child with Animator component (usually the model)
 * 
 * 3. In Animator component:
 *    - Controller: Drag "ImpAnimator" from Assets/Imp/Animations/
 *    - ❌ Apply Root Motion: FALSE (very important!)
 *    - Update Mode: Normal
 *    - Culling Mode: Cull Update Transforms
 * 
 * 4. Apply to prefab
 * 
 * 
 * STEP 2: Verify Animator Controller Has States
 * -----------------------------------------------
 * 
 * 1. Open: Assets/Imp/Animations/ImpAnimator
 * 
 * 2. In Animator window, you should see states:
 *    - Idle
 *    - Move
 *    - Attack
 *    - Dead
 *    - Take Damage (optional)
 * 
 * 3. Check Parameters panel (top left):
 *    - Speed (Float)
 *    - Attack (Trigger)
 *    - Die (Trigger)
 *    - TakeDamage (Trigger) - optional
 * 
 * 4. If missing, use: Tools → Setup Imp Animator Controller
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                  ANIMATOR CONTROLLER STRUCTURE
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * The ImpEnemy script controls animations using these parameters:
 * 
 * SPEED (Float):
 * - Set continuously in Update()
 * - Value 0 → Idle animation
 * - Value > 0.1 → Move/Walk animation
 * - Calculated from: agent.velocity.magnitude / agent.speed
 * 
 * ATTACK (Trigger):
 * - Set when attacking
 * - Triggers: animator.SetTrigger("Attack")
 * - Plays attack animation
 * - Auto-returns to Idle when done
 * 
 * DIE (Trigger):
 * - Set when health <= 0
 * - Triggers: animator.SetTrigger("Die")
 * - Plays death animation
 * - No return (stays in dead state)
 * 
 * TAKEDAMAGE (Trigger) - Optional:
 * - Could be used for hit reactions
 * - Not currently used by ImpEnemy script
 * - Can be added later
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    STATE MACHINE DIAGRAM
 * ═══════════════════════════════════════════════════════════════════════
 * 
 *                    ┌─────────┐
 *                    │  Entry  │
 *                    └────┬────┘
 *                         │
 *                         ▼
 *                    ┌─────────┐
 *              ┌────▶│  Idle   │◀────┐
 *              │     └────┬────┘     │
 *              │          │          │
 *      Speed<0.1   Speed>0.1   hasExitTime
 *              │          │          │
 *              │          ▼          │
 *              │     ┌─────────┐    │
 *              └─────│  Move   │────┘
 *                    └────┬────┘
 *                         │
 *                   Attack trigger
 *                         │
 *                         ▼
 *                    ┌─────────┐
 *                    │ Attack  │
 *                    └────┬────┘
 *                         │
 *                    hasExitTime
 *                         │
 *                         └──────▶ Back to Idle
 * 
 *                 Any State ──Die trigger──▶ ┌──────┐
 *                                             │ Dead │
 *                                             └──────┘
 *                                             (final state)
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    AVAILABLE ANIMATIONS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Location: Assets/Imp/Animations/
 * 
 * Idle.anim              → Standing idle pose
 * Move.anim              → Walking/running forward
 * Move2.forvard.anim     → Alternative walk (can use either)
 * Attack1.anim           → Attack animation
 * Attack2.anim           → Alternative attack (can use for variety)
 * Dead.anim              → Death animation
 * Take Damage.anim       → Hit reaction
 * Take Damage2.anim      → Alternative hit reaction
 * 
 * The automatic setup uses:
 * - Idle.anim for Idle state
 * - Move.anim for Move state
 * - Attack1.anim for Attack state
 * - Dead.anim for Dead state
 * 
 * You can swap these in the Animator Controller if you prefer different ones!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    VERIFY ANIMATOR IS WORKING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * STEP 1: Check Assignment
 * -------------------------
 * 
 * Unity menu: Tools → Check Imp Animator
 * 
 * Should show:
 * ✅ Animator component: Found
 * ✅ Controller: Assets/Imp/Animations/ImpAnimator.controller
 * ✅ Apply Root Motion: False
 * 
 * 
 * STEP 2: Test in Play Mode
 * ---------------------------
 * 
 * 1. Press Play
 * 
 * 2. Spawn an Imp (press B)
 * 
 * 3. Watch animations:
 *    ✅ Should play Idle when standing
 *    ✅ Should play Move when walking
 *    ✅ Should play Attack when attacking
 *    ✅ Should play Dead when killed
 * 
 * 
 * STEP 3: Debug Animator in Play Mode
 * -------------------------------------
 * 
 * 1. With game running, select an Imp in Hierarchy
 * 
 * 2. Open Animator window (Window → Animation → Animator)
 * 
 * 3. Watch states light up as Imp behaves:
 *    - Idle state should be active when still
 *    - Move state should activate when walking
 *    - Attack state when attacking
 * 
 * 4. Check parameters values:
 *    - Speed should change 0-1 based on movement
 *    - Triggers should flash when activated
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                       TROUBLESHOOTING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * PROBLEM: Imp is T-posing (stuck in default pose)
 * 
 * CAUSE: No animator controller assigned
 * FIX: Run "Tools → Setup Imp Animator Controller"
 * 
 * 
 * PROBLEM: Imp slides around (moves without walk animation)
 * 
 * CAUSE 1: Apply Root Motion is ON
 * FIX: Turn OFF "Apply Root Motion" in Animator component
 * 
 * CAUSE 2: Speed parameter not updating
 * FIX: Check ImpEnemy script UpdateAnimation() is being called
 * 
 * 
 * PROBLEM: Imp plays Idle but never Walk
 * 
 * CAUSE: Speed parameter not reaching > 0.1
 * FIX: Check Imp is actually moving (NavMesh baked?)
 * 
 * 
 * PROBLEM: Walk animation plays but Imp doesn't move position
 * 
 * CAUSE: Apply Root Motion is ON
 * FIX: Turn OFF "Apply Root Motion" - NavMeshAgent controls position!
 * 
 * 
 * PROBLEM: Attack animation doesn't play
 * 
 * CAUSE: Attack trigger not being set
 * FIX: 
 *   1. Check ImpEnemy script AttackRoutine() calls animator.SetTrigger("Attack")
 *   2. Check Animator has "Attack" parameter (case-sensitive!)
 *   3. Check transition from Any State to Attack state exists
 * 
 * 
 * PROBLEM: Death animation doesn't play
 * 
 * CAUSE: Die trigger not set or transition missing
 * FIX:
 *   1. Check ImpEnemy Die() calls animator.SetTrigger("Die")
 *   2. Check Animator has "Die" parameter
 *   3. Check Any State → Dead transition exists
 * 
 * 
 * PROBLEM: Animations are very fast/slow
 * 
 * CAUSE: Animation clip speed settings
 * FIX:
 *   1. Select animation clip (e.g., Move.anim)
 *   2. In Inspector, adjust Speed multiplier
 *   3. Typical values: 0.5-2.0
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    CUSTOMIZING ANIMATIONS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * SWAP AN ANIMATION:
 * 
 * 1. Open: Assets/Imp/Animations/ImpAnimator
 * 
 * 2. Click a state (e.g., "Move")
 * 
 * 3. In Inspector, find "Motion" field
 * 
 * 4. Drag different animation clip (e.g., Move2.forvard)
 * 
 * 5. Test in Play mode
 * 
 * 
 * ADD RANDOM ATTACKS:
 * 
 * Currently uses Attack1 only. To add variety:
 * 
 * 1. Create a Blend Tree for Attack state
 * 
 * 2. Add Attack1 and Attack2 clips
 * 
 * 3. Use Random blend type
 * 
 * 4. Both attacks will randomly play!
 * 
 * 
 * ADJUST TRANSITION SPEEDS:
 * 
 * Make animations blend faster/slower:
 * 
 * 1. Click transition arrow between states
 * 
 * 2. In Inspector, adjust "Transition Duration"
 *    - 0 = Instant snap
 *    - 0.1-0.2 = Quick blend (recommended)
 *    - 0.5+ = Slow smooth blend
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                         QUICK CHECKLIST
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * [ ] Animator Controller exists at Assets/Imp/Animations/ImpAnimator
 * [ ] Controller has states: Idle, Move, Attack, Dead
 * [ ] Controller has parameters: Speed, Attack, Die
 * [ ] Imp prefab has Animator component
 * [ ] Animator component has Controller assigned
 * [ ] Apply Root Motion = FALSE (critical!)
 * [ ] Animations play in Play mode
 * 
 * If all checked, animations should work perfectly!
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class IMP_ANIMATOR_SETUP_GUIDE : MonoBehaviour
{
    // Automatic: Tools → FIX IMP - Scale + Material + Movement (COMPLETE)
    // Or just: Tools → Setup Imp Animator Controller
    // Then verify: Tools → Check Imp Animator
}
