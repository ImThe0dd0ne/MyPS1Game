/*
 * ═══════════════════════════════════════════════════════════════════════
 *              WHY IMPS DON'T MOVE - COMPLETE TROUBLESHOOTING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * PROBLEM: Imps spawn but stand completely still (static models)
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                  FIX #1: AUTOMATIC COMPLETE FIX
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * In Unity menu bar:
 * 
 * Tools → FIX IMP - Scale + Material + Movement (COMPLETE)
 * 
 * This will:
 * ✅ Fix scale (make Imp smaller)
 * ✅ Fix material (add red color)
 * ✅ Fix NavMeshAgent settings
 * 
 * Then continue to Fix #2 below!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *           FIX #2: BAKE NAVMESH (CRITICAL FOR MOVEMENT!)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Imps use NavMeshAgent to move, which REQUIRES a baked NavMesh.
 * 
 * STEP 1: Open Navigation Window
 * --------------------------------
 * 
 * Window → AI → Navigation
 * 
 * (If you don't see it, try: Window → AI → Navigation (Legacy))
 * 
 * 
 * STEP 2: Mark Ground as Walkable
 * ---------------------------------
 * 
 * 1. Select your ground/floor/terrain objects in the Hierarchy
 *    (Usually "Ground", "Floor", "Terrain", "Plane", etc.)
 * 
 * 2. In the Navigation window, click "Object" tab
 * 
 * 3. Check ✅ "Navigation Static"
 * 
 * 4. Click "Apply" at the bottom
 * 
 * 
 * STEP 3: Bake NavMesh
 * ---------------------
 * 
 * 1. In Navigation window, click "Bake" tab
 * 
 * 2. Settings (use these for good results):
 *    - Agent Radius: 0.35
 *    - Agent Height: 1.2
 *    - Max Slope: 45
 *    - Step Height: 0.4
 * 
 * 3. Click "Bake" button at the bottom
 * 
 * 4. Wait for baking to finish (few seconds)
 * 
 * 5. You should see BLUE overlay on walkable areas in Scene view
 * 
 * ✅ If you see blue overlay = NavMesh is ready!
 * ❌ If NO blue overlay = No walkable areas (check Step 2)
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *        FIX #3: CHECK IMP SPAWNS ON NAVMESH (VERY IMPORTANT!)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Imps MUST spawn on the blue NavMesh areas to move!
 * 
 * Check ArenaManager spawn points:
 * 
 * 1. Select ArenaManager in Hierarchy
 * 
 * 2. Look at "Enemy Spawn Points" array
 * 
 * 3. In Scene view, check if spawn points are ON the blue NavMesh
 * 
 * If spawn points are OUTSIDE blue areas:
 * 
 * → Move spawn points to be ON the blue NavMesh
 * → Or expand NavMesh to cover spawn points
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *              FIX #4: CHECK CONSOLE FOR ERRORS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Press Play and look at Console (Ctrl+Shift+C)
 * 
 * Common errors:
 * 
 * "SetDestination can only be called on an active agent"
 * → Agent not on NavMesh when spawned
 * → FIX: Move spawn points to blue NavMesh areas
 * 
 * "Failed to create agent because it is not close to NavMesh"
 * → Agent spawned too far from NavMesh
 * → FIX: Spawn closer to ground, or expand NavMesh
 * 
 * "Player not found"
 * → Player GameObject doesn't have "Player" tag
 * → FIX: Select Player → Inspector → Tag → Player
 * 
 * "NavMesh Agent has no path"
 * → Agent can't reach player
 * → FIX: Make sure player is also on NavMesh
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                  FIX #5: LAYER MASK CHECK
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Make sure layer masks are correct:
 * 
 * 1. Select Imp prefab
 * 
 * 2. Find ImpEnemy script in Inspector
 * 
 * 3. Check "What Is Player" layer mask:
 *    ✅ Should include "Player" layer (layer 6)
 *    ✅ Or "WhatIsPlayer" layer (layer 7)
 * 
 * 4. Check "What Is Ground" layer mask:
 *    ✅ Should include ground layer (layer 8 or Default)
 * 
 * If wrong:
 * → Click layer mask dropdown
 * → Select correct layers
 * → Apply to prefab
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                  FIX #6: ANIMATOR SETTINGS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Check Animator isn't blocking movement:
 * 
 * 1. Select Imp prefab
 * 
 * 2. Find Animator component
 * 
 * 3. Make sure:
 *    ❌ Apply Root Motion = FALSE (must be unchecked!)
 *    ✅ Update Mode = Normal
 *    ✅ Culling Mode = Always Animate
 * 
 * If "Apply Root Motion" is ON, Imp won't respond to NavMeshAgent!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *              DEBUGGING: CHECK IF SCRIPT IS RUNNING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * TEMPORARY DEBUG METHOD:
 * 
 * 1. Open: Assets/My Scripts/ImpEnemy.cs
 * 
 * 2. In Update() method, add at the very top:
 * 
 *    Debug.Log("Imp update - State: " + currentState);
 * 
 * 3. Save and run game
 * 
 * 4. Check Console - should spam "Imp update" messages
 * 
 * If you see messages:
 * ✅ Script is running
 * → Problem is NavMesh or spawn position
 * 
 * If NO messages:
 * ❌ Script not running
 * → Check script is attached to prefab
 * → Check no errors preventing compilation
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                       QUICK CHECKLIST
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Before testing Imp movement, verify:
 * 
 * [ ] Imp prefab has ImpEnemy script attached
 * [ ] Imp prefab has NavMeshAgent component
 * [ ] NavMeshAgent: Apply Root Motion = FALSE
 * [ ] NavMesh is baked (blue overlay in Scene)
 * [ ] Spawn points are ON blue NavMesh areas
 * [ ] Player has "Player" tag
 * [ ] Player layer is in "What Is Player" mask
 * [ ] Imp scale is reasonable (0.4 - 0.5)
 * [ ] No errors in Console when Imp spawns
 * 
 * If ALL checked and still not moving:
 * → Check console for specific error messages
 * → Use debug logging to see what state Imp is in
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    COMMON SCENARIOS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * SCENARIO 1: Imp spawns, stands still, never moves
 * 
 * CAUSE: Not on NavMesh OR NavMesh not baked
 * FIX: Bake NavMesh, move spawn point to NavMesh
 * 
 * 
 * SCENARIO 2: Imp spawns, rotates to face player, doesn't walk
 * 
 * CAUSE: NavMeshAgent can't find path OR Apply Root Motion is ON
 * FIX: Check Animator settings, ensure player on NavMesh
 * 
 * 
 * SCENARIO 3: Imp doesn't detect player at all
 * 
 * CAUSE: Layer mask wrong OR player too far
 * FIX: Check "What Is Player" layer mask includes Player layer
 * 
 * 
 * SCENARIO 4: Imp walks in circles/random direction
 * 
 * CAUSE: Player reference not set OR patrol mode stuck
 * FIX: Player should auto-find with tag, check tag is "Player"
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                         STEP-BY-STEP TEST
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. Run: Tools → FIX IMP - Scale + Material + Movement
 * 
 * 2. Bake NavMesh:
 *    - Window → AI → Navigation
 *    - Mark ground as Navigation Static
 *    - Click Bake
 *    - Verify blue overlay appears
 * 
 * 3. Check spawn points are on blue NavMesh
 * 
 * 4. Press Play
 * 
 * 5. Spawn Imp (press B for arena)
 * 
 * 6. Watch Imp behavior:
 *    - Should patrol (walk around)
 *    - Should chase when you get close
 *    - Should stop and attack at range
 * 
 * 7. If not working, check Console for errors
 * 
 * 8. Use: Tools → Check Imp Status
 *    To verify all settings
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    STILL NOT WORKING?
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * If after all fixes Imp still doesn't move:
 * 
 * 1. Use: Tools → Check Imp Status
 *    → Copy the output
 * 
 * 2. Check Console for ANY error messages
 *    → Copy error messages
 * 
 * 3. Take screenshot of:
 *    → Scene view showing NavMesh (blue overlay)
 *    → Imp prefab Inspector (all components)
 *    → ArenaManager spawn points
 * 
 * 4. Share this info for further debugging
 * 
 * 
 * Most likely cause: NavMesh not baked or spawn point not on NavMesh!
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class IMP_NOT_MOVING_FIX : MonoBehaviour
{
    // Step 1: Tools → FIX IMP - Scale + Material + Movement
    // Step 2: Window → AI → Navigation → Bake NavMesh
    // Step 3: Ensure spawn points are ON the blue NavMesh areas
    // Step 4: Test!
}
