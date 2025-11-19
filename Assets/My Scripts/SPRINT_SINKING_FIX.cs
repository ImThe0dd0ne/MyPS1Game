/*
 * ═══════════════════════════════════════════════════════════════════════
 *               SPRINT SINKING FIX - DO THIS NOW!
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * PROBLEM: Character sinks into ground when sprinting
 * 
 * CAUSE: Using root-motion Sprint animation with applyRootMotion enabled
 *        The animation has vertical movement baked in that pushes down!
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                       THE FIX (2 MINUTES)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * OPTION 1: Use InPlace Sprint Animation (RECOMMENDED)
 * -----------------------------------------------------
 * 
 * 1. Open Animator Window (Window → Animation → Animator)
 * 
 * 2. Select your Animator Controller:
 *    Assets/Character/Knight/Animations/KnightAnimator.controller
 * 
 * 3. Find the "Sprint" state (you have it selected already!)
 * 
 * 4. In Inspector, look for "Motion" field
 * 
 * 5. Click the circle icon next to Motion field
 * 
 * 6. Search for: "OneHand_Up_Sprint_InPlace"
 * 
 * 7. Select: OneHand_Up_Sprint_InPlace.anim
 *    (Located in: Assets/DoubleL/Demo/Anim/)
 * 
 * 8. Press Play and test!
 * 
 * ✅ DONE! Sprint should work perfectly now!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * OPTION 2: Disable Root Motion (ALTERNATIVE)
 * --------------------------------------------
 * 
 * If you want to keep the root-motion animation:
 * 
 * 1. Select Player/Knight in Hierarchy
 * 
 * 2. Find Animator component
 * 
 * 3. UNCHECK "Apply Root Motion"
 * 
 * 4. Press Play and test!
 * 
 * ✅ This also works!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * WHY THIS HAPPENED:
 * ------------------
 * 
 * Your Animator has:
 * - Apply Root Motion: TRUE
 * - Sprint Motion: OneHand_Up_Sprint (root motion version)
 * 
 * Root motion animations have positional data baked in.
 * When Apply Root Motion is ON, Unity moves the character 
 * according to the animation's baked movement.
 * 
 * Your Sprint animation has vertical movement that pushes DOWN.
 * This is normal for root-motion anims - they're meant to be 
 * used with ONLY root motion movement (no script movement).
 * 
 * Since you're using SCRIPT-DRIVEN movement (ThirdPersonPlayer.cs),
 * you need EITHER:
 * - InPlace animations (recommended for attacks too!)
 * - OR Apply Root Motion OFF
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * RECOMMENDED SETUP FOR YOUR PROJECT:
 * -----------------------------------
 * 
 * Since you're using script-driven movement, use InPlace versions:
 * 
 * Walk State → OneHand_Up_Walk_InPlace
 * Sprint State → OneHand_Up_Sprint_InPlace
 * Attack1 State → OneHand_Up_Attack_1_InPlace
 * Attack2 State → OneHand_Up_Attack_2_InPlace
 * Attack3 State → OneHand_Up_Attack_3_InPlace
 * 
 * AND keep:
 * Animator → Apply Root Motion: FALSE
 * 
 * This gives you full control via scripts while animations 
 * play correctly in-place!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * QUICK CHECKLIST:
 * ----------------
 * 
 * ☐ Open Animator window
 * ☐ Select Sprint state
 * ☐ Change Motion to: OneHand_Up_Sprint_InPlace
 * ☐ Press Play
 * ☐ Test sprint (Shift key)
 * ☐ Character should NOT sink!
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class SPRINT_SINKING_FIX : MonoBehaviour
{
    // Follow the guide above!
}
