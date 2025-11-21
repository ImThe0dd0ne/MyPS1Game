/*
 * ═══════════════════════════════════════════════════════════════════════
 *                   FIX PINK MATERIALS - QUICK GUIDE
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * PROBLEM: Imp (and possibly other models) show as bright pink
 * 
 * CAUSE: Materials are using Built-in shaders, but your project uses URP
 *        (Universal Render Pipeline)
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    AUTOMATIC FIX (EASIEST)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * 1. In Unity menu bar, click: Tools → Fix Imp Material (URP)
 * 
 * 2. Check Console - should say "✅ Imp Material fixed!"
 * 
 * 3. Check Imp model - should no longer be pink!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    MANUAL FIX (IF AUTOMATIC FAILS)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * METHOD 1 - QUICK FIX:
 * ---------------------
 * 
 * 1. Select: Assets/Imp/Materials/Imp Red Material
 * 
 * 2. In Inspector, find "Shader" dropdown (at the top)
 * 
 * 3. Click the dropdown
 * 
 * 4. Choose: Universal Render Pipeline → Lit
 * 
 * 5. Done! Material should now show correctly
 * 
 * 
 * METHOD 2 - UPGRADE ALL MATERIALS AT ONCE:
 * ------------------------------------------
 * 
 * If you have MANY pink materials:
 * 
 * 1. Unity menu: Edit → Render Pipeline → Universal Render Pipeline
 * 
 * 2. Click: Upgrade Project Materials to URP Materials
 * 
 * 3. Click "Proceed" in the dialog
 * 
 * 4. Wait for upgrade to complete
 * 
 * 5. All materials should now be fixed!
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    FOR SPIDER (IF ALSO PINK)
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * If your Spider model is also pink, do the same:
 * 
 * 1. Find Spider materials in Assets/Spider/Materials/ (or similar)
 * 
 * 2. Select each pink material
 * 
 * 3. Change Shader to: Universal Render Pipeline → Lit
 * 
 * 4. Repeat for all Spider materials
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    UNDERSTANDING THE ISSUE
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * WHY PINK?
 * 
 * Unity shows pink when a material's shader is incompatible with your
 * render pipeline. Your project uses URP, but the Imp asset came with
 * Built-in Pipeline shaders.
 * 
 * SHADER TYPES:
 * 
 * ❌ Built-in: "Standard", "Standard (Specular Setup)"
 *    → Shows PINK in URP projects
 * 
 * ✅ URP: "Universal Render Pipeline/Lit"
 *    → Works correctly in URP projects
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    COMMON SHADER CONVERSIONS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Built-in Shader               →  URP Equivalent
 * --------------------------------  ---------------------------------
 * Standard                      →  Universal Render Pipeline/Lit
 * Standard (Specular Setup)     →  Universal Render Pipeline/Lit
 * Unlit/Color                   →  Universal Render Pipeline/Unlit
 * Unlit/Texture                 →  Universal Render Pipeline/Unlit
 * Particles/Standard Unlit      →  Universal Render Pipeline/Particles/Unlit
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    ADVANCED: URP SHADER OPTIONS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * Universal Render Pipeline/Lit
 * - Best for most 3D models
 * - Supports lighting, shadows, normal maps
 * - PBR (Physically Based Rendering)
 * 
 * Universal Render Pipeline/Simple Lit
 * - Lighter version of Lit
 * - Good for mobile/performance
 * - Less features but faster
 * 
 * Universal Render Pipeline/Unlit
 * - No lighting calculations
 * - Good for UI, VFX, glowing objects
 * - Fastest performance
 * 
 * Universal Render Pipeline/Baked Lit
 * - For objects with baked lighting only
 * - No real-time lights
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    TROUBLESHOOTING
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * PROBLEM: "Tools → Fix Imp Material" menu doesn't appear
 * FIX: Wait a few seconds for script to compile, then check again
 * 
 * PROBLEM: Material still pink after changing shader
 * FIX: Try these:
 *   1. Check if textures are assigned (BaseMap, Normal Map)
 *   2. Try "Universal Render Pipeline/Simple Lit" instead
 *   3. Create a NEW material and copy texture assignments
 * 
 * PROBLEM: Model is now gray/white instead of colored
 * FIX: 
 *   1. Select material
 *   2. Set "Base Color" to desired color (red for Imp)
 *   3. Or assign the albedo/diffuse texture to "Base Map"
 * 
 * PROBLEM: All my materials turned pink after importing asset
 * FIX: Use Method 2 above to upgrade all project materials at once
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    PREVENTION FOR FUTURE ASSETS
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * When importing NEW 3D models/assets from the Asset Store or elsewhere:
 * 
 * 1. Import the asset normally
 * 
 * 2. If materials are pink:
 *    - Edit → Render Pipeline → URP
 *    - Upgrade Project Materials to URP Materials
 * 
 * 3. Or manually change each material's shader to URP/Lit
 * 
 * 4. For consistent results:
 *    - Look for "URP compatible" assets on Asset Store
 *    - Or create your own materials with URP shaders
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 *                    QUICK REFERENCE
 * ═══════════════════════════════════════════════════════════════════════
 * 
 * ✅ TO FIX IMP NOW:
 * 
 * Option A (Auto):
 *   Tools → Fix Imp Material (URP)
 * 
 * Option B (Manual):
 *   1. Select: Imp Red Material
 *   2. Shader → Universal Render Pipeline → Lit
 * 
 * Option C (All materials):
 *   Edit → Render Pipeline → URP → Upgrade Project Materials
 * 
 * 
 * ═══════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class FIX_PINK_MATERIALS_GUIDE : MonoBehaviour
{
    // Use Tools → Fix Imp Material (URP) to auto-fix!
    // Or follow the manual steps above.
}
