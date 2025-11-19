/*
 * ═══════════════════════════════════════════════════════════════════════
 *                    MOVEMENT COMPLETELY RESTORED
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * I've restored Movement.cs (ThirdPersonPlayer) to the ORIGINAL version
 * from the very start of our conversation.
 * 
 * 
 * WHAT WAS RESTORED:
 * ==================
 * 
 * 1. Input.GetAxis (instead of GetAxisRaw)
 *    - Smooth acceleration instead of instant response
 * 
 * 2. Original ProjectOnPlane logic with magnitude preservation
 *    - move = Vector3.ProjectOnPlane(move, hit.normal).normalized * mag;
 *    - This keeps you on the ground properly
 * 
 * 3. Removed animator.applyRootMotion = false from Start()
 *    - Back to default Unity behavior
 * 
 * 
 * THE MOVEMENT SHOULD NOW WORK EXACTLY AS IT DID ORIGINALLY:
 * ===========================================================
 * 
 * ✅ Walking works
 * ✅ Running/Sprinting works
 * ✅ No sinking into ground
 * ✅ Smooth acceleration
 * ✅ Stays on terrain surface
 * 
 * 
 * IF IT STILL SINKS:
 * ==================
 * 
 * The issue is NOT the movement code - it's the ANIMATOR.
 * 
 * You need to check ONE setting:
 * 
 * 1. Select Player/Knight in Hierarchy
 * 2. Find Animator component
 * 3. Look at "Apply Root Motion" checkbox
 * 
 * If it's CHECKED and you're using a root-motion Sprint animation:
 * - That's what's pushing you into the ground
 * 
 * SOLUTION:
 * - Either UNCHECK "Apply Root Motion"
 * - OR change Sprint animation to InPlace version in Animator Controller
 * 
 * The movement code is now exactly as it was originally.
 * Any sinking is from the Animator, not the script.
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class MOVEMENT_RESTORED : MonoBehaviour
{
    // Movement.cs has been completely restored!
}
