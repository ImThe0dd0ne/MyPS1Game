/*
 * ═══════════════════════════════════════════════════════════════════════
 *                         QUICK START GUIDE
 *                    (Get Combat Working in 10 Minutes!)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Follow these steps EXACTLY and you'll have working combo combat!
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                          STEP 1: ANIMATOR
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. Open: Assets/Character/Knight/Animations/KnightAnimator.controller
 * 
 * 2. Add 3 Trigger Parameters:
 *    Parameters tab → + → Trigger
 *    Names: Attack1, Attack2, Attack3
 * 
 * 3. Create 3 Attack States:
 *    Right-click → Create State → Empty
 *    
 *    State 1: Attack1_State
 *      Motion: OneHand_Up_Attack_1.anim
 *      Write Defaults: ☐ (unchecked)
 *    
 *    State 2: Attack2_State  
 *      Motion: OneHand_Up_Attack_2.anim
 *      Write Defaults: ☐
 *    
 *    State 3: Attack3_State
 *      Motion: OneHand_Up_Attack_3.anim
 *      Write Defaults: ☐
 * 
 * 4. Create Transitions (The Important Part!):
 *    
 *    Idle → Attack1:
 *      Has Exit Time: ☐ NO
 *      Condition: Attack1
 *      Duration: 0.05
 *    
 *    Attack1 → Attack2:
 *      Has Exit Time: ☑ YES
 *      Exit Time: 0.7
 *      Condition: Attack2
 *      Duration: 0.1
 *    
 *    Attack2 → Attack3:
 *      Has Exit Time: ☑ YES
 *      Exit Time: 0.7
 *      Condition: Attack3
 *      Duration: 0.1
 *    
 *    Attack1 → Idle (timeout):
 *      Has Exit Time: ☑ YES
 *      Exit Time: 0.95
 *      Condition: (none)
 *      Duration: 0.15
 *    
 *    Attack2 → Idle (timeout):
 *      Has Exit Time: ☑ YES
 *      Exit Time: 0.95
 *      Condition: (none)
 *      Duration: 0.15
 *    
 *    Attack3 → Idle (always):
 *      Has Exit Time: ☑ YES
 *      Exit Time: 0.9
 *      Condition: (none)
 *      Duration: 0.2
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                     STEP 2: FIX ROOT MOTION
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * This fixes the glitching when standing still!
 * 
 * 1. Select your Player GameObject
 * 
 * 2. Find Animator component
 * 
 * 3. UNCHECK ☐ "Apply Root Motion"
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                      STEP 3: ADD THE SCRIPT
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. Select Player GameObject
 * 
 * 2. DISABLE old PlayerAttack component (uncheck it)
 * 
 * 3. Add Component → ModularCombatSystem
 * 
 * 4. Expand "Combo Chain"
 *    Set Size: 3
 * 
 * 5. Set these values:
 *    
 *    Element 0:
 *      Animation Trigger: Attack1
 *      Damage: 25
 *      Hit Detection Delay: 0.15
 *      Recovery Time: 0.2
 *      Attack Range: 2.8
 *      Attack Angle: 90
 *    
 *    Element 1:
 *      Animation Trigger: Attack2
 *      Damage: 35
 *      Hit Detection Delay: 0.18
 *      Recovery Time: 0.25
 *      Attack Range: 2.8
 *      Attack Angle: 90
 *    
 *    Element 2:
 *      Animation Trigger: Attack3
 *      Damage: 50
 *      Hit Detection Delay: 0.2
 *      Recovery Time: 0.3
 *      Attack Range: 3.2
 *      Attack Angle: 120
 * 
 * 6. Copy references from old PlayerAttack:
 *    - Sword Transform
 *    - Animator
 *    - Sword Trail
 *    - Audio clips
 *    - Particle effects
 *    - Enemy Layer: Enemy
 * 
 * 7. Other settings:
 *    - Combo Window: 0.8
 *    - Attack Speed: 1.2
 *    - Use Root Motion: ☐ (unchecked)
 *    - Allow Input Buffering: ☑ (checked)
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                        STEP 4: TEST!
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. Press Play
 * 
 * 2. Click left mouse button 3 times quickly
 * 
 * 3. Watch the 3-hit combo!
 * 
 * 4. Try while standing still - should work perfectly!
 * 
 * 5. Try while moving - should also work!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                      TROUBLESHOOTING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Q: Still glitches when standing?
 * A: Check "Apply Root Motion" is UNCHECKED on Animator component
 * 
 * Q: Combo doesn't chain?
 * A: Check transitions have correct triggers and Exit Times
 * 
 * Q: Can't attack at all?
 * A: Check Idle→Attack1 has Attack1 trigger and NO exit time
 * 
 * Q: Character slides during attack?
 * A: Use InPlace animations instead:
 *    OneHand_Up_Attack_1_InPlace.anim
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    NEED MORE DETAILS?
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * See: ANIMATOR_SETUP_GUIDE.cs for complete step-by-step instructions!
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class QUICK_START : MonoBehaviour
{
    // Documentation only!
}
