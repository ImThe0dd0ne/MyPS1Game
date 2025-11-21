/*
 * ═══════════════════════════════════════════════════════════════════════
 *              COMBAT TIMING VALUES - QUICK REFERENCE
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Current Settings: FAST-PACED ACTION (DMC/RoR2 style)
 * 
 * 
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                      ATTACK TIMINGS                                 │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * Attack Speed Multiplier:     1.5x
 * 
 * Attack 1:
 * ├─ Hit Delay:        0.08s  (time to damage)
 * ├─ Recovery:         0.10s  (lockout after)
 * ├─ Total Duration:   0.18s
 * └─ Attacks/second:   ~5.5
 * 
 * Attack 2:
 * ├─ Hit Delay:        0.10s
 * ├─ Recovery:         0.12s
 * ├─ Total Duration:   0.22s
 * └─ Attacks/second:   ~4.5
 * 
 * Attack 3:
 * ├─ Hit Delay:        0.12s
 * ├─ Recovery:         0.15s
 * ├─ Total Duration:   0.27s
 * └─ Attacks/second:   ~3.7
 * 
 * Full 3-Hit Combo:    ~0.67s total
 * 
 * 
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                    MOVEMENT SETTINGS                                │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * Walk Speed:          6.5 m/s
 * Sprint Speed:        13 m/s
 * Rotation Speed:      18 (degrees/frame at 60fps)
 * Input Smooth:        0.08s
 * Animation Damp:      0.05s
 * 
 * Movement During Attacks:  ✅ ENABLED (always)
 * Rotation During Attacks:  ✅ ENABLED (always)
 * Sprint During Attacks:    ✅ ENABLED (always)
 * 
 * 
 * ┌─────────────────────────────────────────────────────────────────────┐
 * │                   PRESET CONFIGURATIONS                             │
 * └─────────────────────────────────────────────────────────────────────┘
 * 
 * ULTRA FAST (Pure Arcade):
 * -------------------------
 * Attack Speed:        2.0x
 * Hit Delays:          0.04s, 0.06s, 0.08s
 * Recovery:            0.05s, 0.06s, 0.08s
 * Result: Blazing fast, button-masher style
 * 
 * 
 * CURRENT (Fast Action):
 * ----------------------
 * Attack Speed:        1.5x
 * Hit Delays:          0.08s, 0.10s, 0.12s
 * Recovery:            0.10s, 0.12s, 0.15s
 * Result: Fast-paced, DMC/RoR2 style ✅ ACTIVE
 * 
 * 
 * BALANCED (Still Fluid):
 * -----------------------
 * Attack Speed:        1.3x
 * Hit Delays:          0.12s, 0.14s, 0.16s
 * Recovery:            0.15s, 0.18s, 0.22s
 * Result: Responsive but strategic
 * 
 * 
 * DELIBERATE (Souls-like):
 * ------------------------
 * Attack Speed:        1.0x
 * Hit Delays:          0.20s, 0.25s, 0.30s
 * Recovery:            0.30s, 0.40s, 0.50s
 * Result: Heavy, committed attacks
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                       HOW TO SWITCH PRESETS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. Select Player/Knight in Hierarchy
 * 2. Find FixedCombatSystem component in Inspector
 * 3. Change values under "Timing" section
 * 4. Press Play to test immediately
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                      PERFORMANCE METRICS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Time to First Hit:       0.08s  (instant feedback!)
 * Combo Chain Speed:       ~0.67s (3 full attacks)
 * Attacks per Second:      ~4-5   (rapid combat)
 * Input to Damage:         0.08s  (feels instant)
 * 
 * Movement Responsiveness: Instant (no locks)
 * Turn Rate:              18/frame (fast)
 * Sprint Accessibility:   Always (even during attacks)
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class COMBAT_VALUES_QUICK_REFERENCE : MonoBehaviour
{
    // Fast-Paced Action preset is active!
    // See values above for tuning reference.
}
