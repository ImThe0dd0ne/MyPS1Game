/*
 * ═══════════════════════════════════════════════════════════════════════
 *              COMPLETE ANIMATOR CONTROLLER SETUP GUIDE
 *                 (For Modular Combat System)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * This guide will help you set up your Animator Controller to work with
 * the new ModularCombatSystem for fast-paced arena combat!
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                        STEP 1: OPEN ANIMATOR
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. In Project window, navigate to:
 *    Assets/Character/Knight/Animations/
 * 
 * 2. Double-click: KnightAnimator.controller
 * 
 * 3. This opens the Animator window
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                   STEP 2: CREATE ATTACK PARAMETERS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. In Animator window, find the "Parameters" tab (usually top-left)
 * 
 * 2. Click the + button
 * 
 * 3. Select "Trigger"
 * 
 * 4. Name it: Attack1
 * 
 * 5. Repeat to create:
 *    - Attack2 (Trigger)
 *    - Attack3 (Trigger)
 * 
 * Your Parameters should now show:
 * ┌─────────────────┐
 * │ Parameters      │
 * ├─────────────────┤
 * │ ▶ Attack1       │
 * │ ▶ Attack2       │
 * │ ▶ Attack3       │
 * └─────────────────┘
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                   STEP 3: CREATE ATTACK STATES
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * ATTACK 1 STATE:
 * ---------------
 * 1. In Animator window, right-click in empty space
 * 
 * 2. Select: Create State → Empty
 * 
 * 3. Name it: Attack1_State
 * 
 * 4. Click on Attack1_State
 * 
 * 5. In Inspector (right side), find "Motion" field
 * 
 * 6. Click the ⊙ circle button
 * 
 * 7. In search popup, type: OneHand_Up_Attack_1
 * 
 * 8. Select: Assets/DoubleL/Demo/Anim/OneHand_Up_Attack_1.anim
 * 
 * 9. IMPORTANT: Uncheck "Write Defaults" (prevents glitches!)
 * 
 * 
 * ATTACK 2 STATE:
 * ---------------
 * 1. Right-click → Create State → Empty
 * 
 * 2. Name it: Attack2_State
 * 
 * 3. Click on Attack2_State
 * 
 * 4. Set Motion to: OneHand_Up_Attack_2.anim
 *    (Search: OneHand_Up_Attack_2)
 * 
 * 5. Uncheck "Write Defaults"
 * 
 * 
 * ATTACK 3 STATE:
 * ---------------
 * 1. Right-click → Create State → Empty
 * 
 * 2. Name it: Attack3_State
 * 
 * 3. Click on Attack3_State
 * 
 * 4. Set Motion to: OneHand_Up_Attack_3.anim
 *    (Search: OneHand_Up_Attack_3)
 * 
 * 5. Uncheck "Write Defaults"
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *              STEP 4: FIX ROOT MOTION (PREVENTS GLITCHING!)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * This is CRITICAL to fix the standing-still glitch!
 * 
 * FOR EACH ATTACK STATE (Attack1, Attack2, Attack3):
 * 
 * 1. Click the state in Animator
 * 
 * 2. In Inspector, find "Foot IK" checkbox
 * 
 * 3. Check ☑ "Foot IK" (helps with grounding)
 * 
 * 4. You should see these settings:
 *    Motion: OneHand_Up_Attack_X.anim
 *    Speed: 1
 *    Foot IK: ☑
 *    Write Defaults: ☐
 * 
 * 
 * DISABLE ROOT MOTION ON ANIMATOR:
 * 
 * 1. Select your Player GameObject in Hierarchy
 * 
 * 2. Find the Animator component
 * 
 * 3. Uncheck ☐ "Apply Root Motion"
 * 
 * This prevents animations from moving your character!
 * (Your movement script handles position, animations only pose)
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *              STEP 5: CREATE TRANSITIONS FROM IDLE
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * You should have an "Idle" state already. If not, create one.
 * 
 * IDLE → ATTACK1:
 * ---------------
 * 1. Right-click on Idle state
 * 
 * 2. Select "Make Transition"
 * 
 * 3. Drag the arrow to Attack1_State
 * 
 * 4. Click the transition arrow
 * 
 * 5. In Inspector, set:
 *    Has Exit Time: ☐ (UNCHECKED!)
 *    Transition Duration: 0.05
 *    
 * 6. Under "Conditions", click +
 * 
 * 7. Select: Attack1
 * 
 * 
 * IDLE → ATTACK2:
 * ---------------
 * (Usually not needed, but for safety)
 * 
 * 1. Right-click Idle → Make Transition → Attack2_State
 * 
 * 2. Set:
 *    Has Exit Time: ☐
 *    Transition Duration: 0.05
 *    Conditions: + Attack2
 * 
 * 
 * IDLE → ATTACK3:
 * ---------------
 * (Usually not needed, but for safety)
 * 
 * 1. Right-click Idle → Make Transition → Attack3_State
 * 
 * 2. Set:
 *    Has Exit Time: ☐
 *    Transition Duration: 0.05
 *    Conditions: + Attack3
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *              STEP 6: CREATE COMBO CHAIN TRANSITIONS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * This creates the smooth combo flow!
 * 
 * ATTACK1 → ATTACK2 (Combo continues):
 * -------------------------------------
 * 1. Right-click Attack1_State
 * 
 * 2. Make Transition → Attack2_State
 * 
 * 3. Click the arrow, set:
 *    Has Exit Time: ☑ (CHECKED!)
 *    Exit Time: 0.7
 *    Transition Duration: 0.1
 *    
 * 4. Under Conditions, click +
 * 
 * 5. Select: Attack2
 * 
 * This means: "Play 70% of Attack1, then if Attack2 triggered, go to Attack2"
 * 
 * 
 * ATTACK2 → ATTACK3 (Combo continues):
 * -------------------------------------
 * 1. Right-click Attack2_State
 * 
 * 2. Make Transition → Attack3_State
 * 
 * 3. Set:
 *    Has Exit Time: ☑
 *    Exit Time: 0.7
 *    Transition Duration: 0.1
 *    Conditions: + Attack3
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *              STEP 7: CREATE RETURN TO IDLE TRANSITIONS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * These handle when combo ends or is interrupted.
 * 
 * ATTACK1 → IDLE (Timeout):
 * --------------------------
 * 1. Right-click Attack1_State
 * 
 * 2. Make Transition → Idle
 * 
 * 3. Set:
 *    Has Exit Time: ☑
 *    Exit Time: 0.95
 *    Transition Duration: 0.15
 *    Conditions: (NONE - leave empty!)
 * 
 * This means: "If Attack2 isn't triggered, return to Idle at 95%"
 * 
 * 
 * ATTACK2 → IDLE (Timeout):
 * --------------------------
 * 1. Right-click Attack2_State → Transition → Idle
 * 
 * 2. Set:
 *    Has Exit Time: ☑
 *    Exit Time: 0.95
 *    Transition Duration: 0.15
 *    Conditions: (NONE)
 * 
 * 
 * ATTACK3 → IDLE (Always):
 * -------------------------
 * 1. Right-click Attack3_State → Transition → Idle
 * 
 * 2. Set:
 *    Has Exit Time: ☑
 *    Exit Time: 0.9
 *    Transition Duration: 0.2
 *    Conditions: (NONE)
 * 
 * Attack3 is the finisher, so it always returns to Idle!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                   STEP 8: VERIFY YOUR SETUP
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Your Animator should look like this:
 * 
 *                    ┌──────────┐
 *                    │   Idle   │
 *                    └────┬─────┘
 *                         │ Attack1
 *                         ▼
 *                    ┌──────────┐
 *          ┌─────────┤ Attack1  ├─────────┐
 *          │         └────┬─────┘         │
 *          │              │ Attack2       │ (timeout)
 *          │              ▼               │
 *          │         ┌──────────┐         │
 *          │    ┌────┤ Attack2  ├────┐    │
 *          │    │    └────┬─────┘    │    │
 *          │    │         │ Attack3  │    │
 *          │    │         ▼          │    │
 *          │    │    ┌──────────┐    │    │
 *          │    │    │ Attack3  │    │    │
 *          │    │    └────┬─────┘    │    │
 *          │    │ (timeout)│(timeout)│    │
 *          ▼    ▼         ▼          ▼    ▼
 *                    ┌──────────┐
 *                    │   Idle   │
 *                    └──────────┘
 * 
 * 
 * CHECKLIST:
 * 
 * ☐ Parameters created: Attack1, Attack2, Attack3 (all Triggers)
 * ☐ States created: Attack1_State, Attack2_State, Attack3_State
 * ☐ Each state has correct animation assigned
 * ☐ Each state has "Write Defaults" UNCHECKED
 * ☐ Idle → Attack1 (no exit time, Attack1 condition)
 * ☐ Attack1 → Attack2 (exit time 0.7, Attack2 condition)
 * ☐ Attack2 → Attack3 (exit time 0.7, Attack3 condition)
 * ☐ Attack1 → Idle (exit time 0.95, no condition)
 * ☐ Attack2 → Idle (exit time 0.95, no condition)
 * ☐ Attack3 → Idle (exit time 0.9, no condition)
 * ☐ Animator component has "Apply Root Motion" UNCHECKED
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                   STEP 9: SET UP THE SCRIPT
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. Select your Player GameObject in Hierarchy
 * 
 * 2. Find the "PlayerAttack" component
 * 
 * 3. Disable it (uncheck the checkbox) - don't delete, just disable!
 * 
 * 4. Click "Add Component"
 * 
 * 5. Search: ModularCombatSystem
 * 
 * 6. Add it!
 * 
 * 7. Copy references from old PlayerAttack:
 *    - Drag Sword Transform
 *    - Drag Attack Point (if you have one)
 *    - Drag Animator
 *    - Drag Sword Trail
 *    - Set Enemy Layer to "Enemy"
 *    - Drag Audio clips (whoosh and hit sounds)
 *    - Drag Particle effects
 * 
 * 8. ModularCombatSystem settings:
 *    
 *    Combo Chain (expand):
 *    - Size: 3
 *    
 *    Element 0:
 *      Animation Trigger: Attack1
 *      Damage: 25
 *      Hit Detection Delay: 0.15
 *      Recovery Time: 0.2
 *      Attack Range: 2.8
 *      Attack Angle: 90
 *      Knockback Multiplier: 1
 *      Camera Shake Multiplier: 1
 *    
 *    Element 1:
 *      Animation Trigger: Attack2
 *      Damage: 35
 *      Hit Detection Delay: 0.18
 *      Recovery Time: 0.25
 *      Attack Range: 2.8
 *      Attack Angle: 90
 *      Knockback Multiplier: 1.5
 *      Camera Shake Multiplier: 1.3
 *    
 *    Element 2:
 *      Animation Trigger: Attack3
 *      Damage: 50
 *      Hit Detection Delay: 0.2
 *      Recovery Time: 0.3
 *      Attack Range: 3.2
 *      Attack Angle: 120
 *      Knockback Multiplier: 2
 *      Camera Shake Multiplier: 1.6
 *    
 *    Other Settings:
 *    - Combo Window: 0.8
 *    - Attack Speed: 1.2
 *    - Enemy Layer: Enemy
 *    - Use Root Motion: ☐ (UNCHECKED!)
 *    - Allow Input Buffering: ☑
 *    - Input Buffer Time: 0.15
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                       STEP 10: TEST!
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. Press Play
 * 
 * 2. Click Left Mouse Button quickly 3 times
 * 
 * 3. You should see:
 *    - Attack 1 plays
 *    - Attack 2 plays
 *    - Attack 3 plays
 *    - Returns to Idle
 * 
 * 4. Test while moving - should work perfectly!
 * 
 * 5. Test while standing still - NO MORE GLITCHES!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    TROUBLESHOOTING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * PROBLEM: Still glitches when standing still
 * FIX: Make sure "Apply Root Motion" is UNCHECKED on Animator component
 * 
 * PROBLEM: Combo doesn't chain
 * FIX: Check Attack1→Attack2 transition has "Attack2" condition
 *      Check Exit Time is 0.7 with "Has Exit Time" CHECKED
 * 
 * PROBLEM: Can't attack from Idle
 * FIX: Check Idle→Attack1 has "Attack1" condition
 *      Make sure "Has Exit Time" is UNCHECKED
 * 
 * PROBLEM: Stuck in attack animation
 * FIX: Add Attack→Idle transitions with Exit Time 0.95
 * 
 * PROBLEM: Animation looks weird/twisted
 * FIX: Make sure DoubleL .fbx files are set to Humanoid:
 *      Click .fbx → Inspector → Rig → Animation Type: Humanoid
 * 
 * PROBLEM: Character sliding during attack
 * FIX: Use InPlace versions:
 *      OneHand_Up_Attack_1_InPlace.anim (etc.)
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    ADVANCED: ADD MORE COMBOS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Want to add a 4th attack? Easy!
 * 
 * 1. In Animator:
 *    - Add Parameter: Attack4 (Trigger)
 *    - Create State: Attack4_State
 *    - Set Motion: OneHand_Up_Attack_B_1.anim
 *    - Add Transitions:
 *      Attack3 → Attack4 (exit 0.7, Attack4 condition)
 *      Attack4 → Idle (exit 0.9, no condition)
 * 
 * 2. In ModularCombatSystem:
 *    - Combo Chain Size: 4
 *    - Element 3:
 *      Animation Trigger: Attack4
 *      Damage: 70
 *      (etc.)
 * 
 * That's it! Fully modular!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                 FUTURE: ADD SPELLS/PROJECTILES
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * The system is designed to be extended!
 * 
 * For projectiles/spells, you would:
 * 
 * 1. Create a new ComboAttack in the array
 * 2. Set a different input (like Right Mouse Button)
 * 3. In hit detection, spawn a projectile instead
 * 4. Add new Animator states for spell casting
 * 
 * The modular design makes this easy to add later!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                        YOU'RE DONE! 🎮
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Your combat should now:
 * ✅ Work while moving or standing still
 * ✅ Chain 3 attacks smoothly
 * ✅ Feel fast and responsive
 * ✅ Have no animation glitches
 * ✅ Be ready for future expansion
 * 
 * Enjoy your arena combat! 🔥
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class ANIMATOR_SETUP_GUIDE : MonoBehaviour
{
    // Documentation only!
}
