/*
 * ═══════════════════════════════════════════════════════════════════════
 *           STEP-BY-STEP: FIX SPRINT SINKING (30 SECONDS!)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 
 * STEP 1: Open Animator Window
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                                                                       │
 * │  At the top menu bar, click:                                         │
 * │                                                                       │
 * │  Window → Animation → Animator                                       │
 * │                                                                       │
 * │  (Or press Ctrl+9)                                                   │
 * │                                                                       │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * 
 * STEP 2: Make Sure Animator Controller is Showing
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                                                                       │
 * │  In Project window, navigate to:                                     │
 * │                                                                       │
 * │  Assets → Character → Knight → Animations                            │
 * │                                                                       │
 * │  Double-click: KnightAnimator                                        │
 * │                                                                       │
 * │  You should see states: Idle, Walk, Sprint, Attack1, etc.           │
 * │                                                                       │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * 
 * STEP 3: Click on Sprint State
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                                                                       │
 * │  In the Animator window, you'll see boxes (states).                  │
 * │                                                                       │
 * │  Click on the box labeled "Sprint"                                   │
 * │                                                                       │
 * │  It should highlight (turn blue/selected)                            │
 * │                                                                       │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * 
 * STEP 4: Look at Inspector
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                                                                       │
 * │  In Inspector window, you should see:                                │
 * │                                                                       │
 * │  ┌───────────────────────────────────────┐                          │
 * │  │ Sprint                                 │                          │
 * │  ├───────────────────────────────────────┤                          │
 * │  │ Motion: OneHand_Up_Sprint    ⊙        │ ← THIS LINE!            │
 * │  │ Speed: 1                               │                          │
 * │  │ ...                                    │                          │
 * │  └───────────────────────────────────────┘                          │
 * │                                                                       │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * 
 * STEP 5: Click the Circle Icon
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                                                                       │
 * │  Next to "Motion:", there's a small CIRCLE icon: ⊙                   │
 * │                                                                       │
 * │  Click that circle!                                                  │
 * │                                                                       │
 * │  A "Select AnimationClip" window will pop up.                        │
 * │                                                                       │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * 
 * STEP 6: Search for InPlace Version
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                                                                       │
 * │  In the search box at top of the popup, type:                        │
 * │                                                                       │
 * │  "Sprint_InPlace"                                                    │
 * │                                                                       │
 * │  You should see:                                                     │
 * │  • OneHand_Up_Sprint_InPlace                                         │
 * │                                                                       │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * 
 * STEP 7: Select InPlace Animation
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                                                                       │
 * │  Double-click on:                                                    │
 * │                                                                       │
 * │  OneHand_Up_Sprint_InPlace                                           │
 * │                                                                       │
 * │  The popup will close.                                               │
 * │                                                                       │
 * │  Inspector should now show:                                          │
 * │  Motion: OneHand_Up_Sprint_InPlace                                   │
 * │                                                                       │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * 
 * STEP 8: Save and Test!
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                                                                       │
 * │  Press: Ctrl+S (to save)                                             │
 * │                                                                       │
 * │  Press: Play button ▶                                                │
 * │                                                                       │
 * │  Hold Shift to sprint                                                │
 * │                                                                       │
 * │  ✅ CHARACTER SHOULD NOT SINK!                                       │
 * │                                                                       │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                          WHAT I ALSO DID
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * I updated Movement.cs to automatically disable "Apply Root Motion"
 * in Start():
 * 
 *     animator.applyRootMotion = false;
 * 
 * This ensures the Animator won't try to move the character with
 * animation data - your script handles all movement instead!
 * 
 * This prevents ANY animation from pushing the character into ground.
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                         THAT'S IT!
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * After you change Sprint to use "OneHand_Up_Sprint_InPlace":
 * 
 * ✅ Walking works
 * ✅ Running/Sprinting works
 * ✅ No sinking
 * ✅ No glitches
 * ✅ Back to how it was at the start!
 * 
 * The InPlace animation plays the SAME motion, just without the 
 * position data that was pushing you into the ground.
 * 
 * Your script moves the character, animation just shows the visuals!
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class STEP_BY_STEP_SPRINT_FIX : MonoBehaviour
{
    // READ THE GUIDE ABOVE!
}
