/*
 * ═══════════════════════════════════════════════════════════════════════
 *               ANIMATOR CONTROLLER - AAA SETTINGS CHECKLIST
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Open: Assets/Character/Knight/Animations/KnightAnimator.controller
 * 
 * 
 * FOR EACH TRANSITION, SET THESE VALUES:
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │                    MOVEMENT TRANSITIONS                         │
 * └─────────────────────────────────────────────────────────────────┘
 * 
 * Idle → Walk
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.05
 * ├─ Interruption Source:     Current State
 * └─ Condition: Speed > 0.1
 * 
 * Walk → Idle
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.05
 * ├─ Interruption Source:     Current State
 * └─ Condition: Speed < 0.1
 * 
 * Walk → Sprint
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.08
 * ├─ Interruption Source:     Current State
 * └─ Condition: Sprint = true
 * 
 * Sprint → Walk
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.08
 * ├─ Interruption Source:     Current State
 * └─ Condition: Sprint = false
 * 
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │                     ATTACK TRANSITIONS                          │
 * └─────────────────────────────────────────────────────────────────┘
 * 
 * Any State → Attack1
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.05
 * ├─ Interruption Source:     Current State
 * ├─ Ordered Interruption:    ☑ ON
 * └─ Condition: Attack1 trigger
 * 
 * Any State → Attack2
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.05
 * ├─ Interruption Source:     Current State
 * ├─ Ordered Interruption:    ☑ ON
 * └─ Condition: Attack2 trigger
 * 
 * Any State → Attack3
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.05
 * ├─ Interruption Source:     Current State
 * ├─ Ordered Interruption:    ☑ ON
 * └─ Condition: Attack3 trigger
 * 
 * Attack1 → Idle
 * ├─ Has Exit Time:           ☑ ON (exception!)
 * ├─ Exit Time:               0.9
 * ├─ Transition Duration:     0.1
 * └─ Interruption Source:     Current State
 * 
 * Attack2 → Idle
 * ├─ Has Exit Time:           ☑ ON (exception!)
 * ├─ Exit Time:               0.9
 * ├─ Transition Duration:     0.1
 * └─ Interruption Source:     Current State
 * 
 * Attack3 → Idle
 * ├─ Has Exit Time:           ☑ ON (exception!)
 * ├─ Exit Time:               0.9
 * ├─ Transition Duration:     0.1
 * └─ Interruption Source:     Current State
 * 
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │                      JUMP TRANSITIONS                           │
 * └─────────────────────────────────────────────────────────────────┘
 * 
 * Any State → Jump
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.05
 * ├─ Interruption Source:     Current State
 * └─ Condition: Jump trigger
 * 
 * Jump → Idle/Walk
 * ├─ Has Exit Time:           ☑ ON
 * ├─ Exit Time:               0.85
 * ├─ Transition Duration:     0.1
 * └─ Condition: IsGrounded = true
 * 
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │                     SLIDE TRANSITIONS                           │
 * └─────────────────────────────────────────────────────────────────┘
 * 
 * Any State → Slide
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.08
 * ├─ Interruption Source:     Current State
 * └─ Condition: IsSliding = true
 * 
 * Slide → Idle/Walk
 * ├─ Has Exit Time:           ☐ OFF
 * ├─ Transition Duration:     0.1
 * ├─ Interruption Source:     Next State
 * └─ Condition: IsSliding = false
 * 
 * 
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 *                           QUICK TIPS
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * 
 * 1. Most transitions should have "Has Exit Time" OFF
 *    - Only use it for attack → idle and jump → idle
 *    - This allows instant interruption = responsive feel
 * 
 * 2. Keep transition durations SHORT (0.05 - 0.1)
 *    - Default Unity is often 0.25 (too slow!)
 *    - 0.05 = snappy, 0.1 = smooth, 0.15+ = sluggish
 * 
 * 3. Set Interruption Source to "Current State"
 *    - Allows new animations to override old ones
 *    - Critical for responsive combat
 * 
 * 4. Enable "Ordered Interruption" for attacks
 *    - Prevents animation glitches
 *    - Ensures combo flows properly
 * 
 * 5. Test frequently!
 *    - Change one transition at a time
 *    - Press Play and test movement/combat
 *    - Adjust durations to taste
 * 
 * 
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 *                       BEFORE vs AFTER
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * 
 * BEFORE (Default Unity):
 * ❌ Has Exit Time: ON everywhere
 * ❌ Transition Duration: 0.25s
 * ❌ Visible pauses between animations
 * ❌ Input feels delayed
 * ❌ Can't interrupt animations
 * 
 * AFTER (AAA Settings):
 * ✅ Has Exit Time: OFF (except attack/jump → idle)
 * ✅ Transition Duration: 0.05 - 0.1s
 * ✅ Seamless animation blending
 * ✅ Instant input response
 * ✅ Can interrupt/queue actions
 * 
 * 
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 *                    HOW TO OPEN ANIMATOR
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * 
 * 1. Project window
 * 2. Navigate to: Assets/Character/Knight/Animations/
 * 3. Double-click: KnightAnimator.controller
 * 4. Animator window opens
 * 5. Click any transition arrow between states
 * 6. Inspector shows transition settings
 * 7. Adjust values as shown above
 * 8. Press Ctrl+S to save
 * 9. Press Play to test
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class ANIMATOR_AAA_CHECKLIST : MonoBehaviour
{
    // Follow the checklist above to update your Animator!
}
