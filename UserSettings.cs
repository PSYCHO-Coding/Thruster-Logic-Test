using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using VRage.ObjectBuilders;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.Game.ModAPI;
using VRage;
using System.Runtime.CompilerServices;

using PSYCHO.ThrusterVisualHandlerData;

namespace PSYCHO.ThrusterVisualHandlerUserSettings
{
    public class UserData
    {
        // DO NOT EDIT!!!
        ThrusterDataHandler ThrusterDataInstance => ThrusterDataHandler.ThrusterDataInstance;

        // ========================
        // USER CHANGABLE VARIABLES
        // ========================

        // ADD THRUSTERS HERE.
        public readonly HashSet<string> ThrusterSubtypeIDs = new HashSet<string>()
        {
            "SuperThruster_Small",
            "LargeBlockSmallThrust",
            "LargeBlockLargeThrust"
        };
        
        public void ConstructThrusterData()
        {
            // ==============================================================
            // COPY AND EDIT THIS PER THRUSTER / THRUSTER + EMISSIVE MATERIAL
            // ==============================================================

            // ONLY EDIT VALUES THAT ARE PROPERLY ANOTATED WITH '// EDIT'

            // ============================================================== COPY BLOCK START
            // DO NOT EDIT
            ThrusterDataInstance.thrustData = new ThrusterDataHandler.ThrusterData();

            // EDIT THRUSTER
            ThrusterDataInstance.MySubtypeID = "SuperThruster_Small";
            // EDIT MATERIAL
            ThrusterDataInstance.thrustData.EmissiveMaterialName = "Emissive";
            // EDIT STATIC
            ThrusterDataInstance.thrustData.OnColor = new Color(0, 20, 255, 255);
            ThrusterDataInstance.thrustData.OffColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonWorkingColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonFunctionalColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.ThrusterOn_EmissiveMultiplier = 1f;
            ThrusterDataInstance.thrustData.ThrusterOff_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNotWorking_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNonFunctional_EmissiveMultiplier = 0f;
            // EDIT DYNAMIC
            ThrusterDataInstance.thrustData.ChangeColorByThrustOutput = true;
            ThrusterDataInstance.thrustData.AntiFlickerThreshold = 0.01f;
            ThrusterDataInstance.thrustData.ColorAtMaxThrust = new Color(255, 40, 10, 255);
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMin = 1f;
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMax = 50f;
            // EDIT DEFAULTS
            //ThrusterDataInstance.thrustData.ErrorColor = Color.Magenta;
            //ThrusterDataInstance.thrustData.CurrentColor = Color.Magenta;

            // DO NOT EDIT
            if (!ThrusterDataInstance.SavedThrusterData.ContainsKey(ThrusterDataInstance.MySubtypeID))
                ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID] = new List<ThrusterDataHandler.ThrusterData>();
            ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID].Add(ThrusterDataInstance.thrustData);
            // ============================================================== COPY BLOCK END

            // ============================================================== COPY BLOCK START
            // DO NOT EDIT
            ThrusterDataInstance.thrustData = new ThrusterDataHandler.ThrusterData();

            // EDIT THRUSTER
            ThrusterDataInstance.MySubtypeID = "LargeBlockSmallThrust";
            // EDIT MATERIAL
            ThrusterDataInstance.thrustData.EmissiveMaterialName = "Emissive";
            // EDIT STATIC
            ThrusterDataInstance.thrustData.OnColor = new Color(0, 20, 255);
            ThrusterDataInstance.thrustData.OffColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonWorkingColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonFunctionalColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.ThrusterOn_EmissiveMultiplier = 10f;
            ThrusterDataInstance.thrustData.ThrusterOff_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNotWorking_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNonFunctional_EmissiveMultiplier = 0f;
            // EDIT DYNAMIC
            ThrusterDataInstance.thrustData.ChangeColorByThrustOutput = true;
            ThrusterDataInstance.thrustData.AntiFlickerThreshold = 0.01f;
            ThrusterDataInstance.thrustData.ColorAtMaxThrust = new Color(255, 40, 10);
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMin = 1f;
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMax = 50f;
            // EDIT DEFAULTS
            //ThrusterDataInstance.thrustData.ErrorColor = Color.Magenta;
            //ThrusterDataInstance.thrustData.CurrentColor = Color.Magenta;

            // DO NOT EDIT
            if (!ThrusterDataInstance.SavedThrusterData.ContainsKey(ThrusterDataInstance.MySubtypeID))
                ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID] = new List<ThrusterDataHandler.ThrusterData>();
            ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID].Add(ThrusterDataInstance.thrustData);
            // ============================================================== COPY BLOCK END

            // ============================================================== COPY BLOCK START
            // DO NOT EDIT
            ThrusterDataInstance.thrustData = new ThrusterDataHandler.ThrusterData();

            // EDIT THRUSTER
            ThrusterDataInstance.MySubtypeID = "LargeBlockSmallThrust";
            // EDIT MATERIAL
            ThrusterDataInstance.thrustData.EmissiveMaterialName = "EmissiveCustom1";
            // EDIT STATIC
            ThrusterDataInstance.thrustData.OnColor = new Color(0, 20, 255);
            ThrusterDataInstance.thrustData.OffColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonWorkingColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonFunctionalColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.ThrusterOn_EmissiveMultiplier = 10f;
            ThrusterDataInstance.thrustData.ThrusterOff_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNotWorking_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNonFunctional_EmissiveMultiplier = 0f;
            // EDIT DYNAMIC
            ThrusterDataInstance.thrustData.ChangeColorByThrustOutput = true;
            ThrusterDataInstance.thrustData.AntiFlickerThreshold = 0.01f;
            ThrusterDataInstance.thrustData.ColorAtMaxThrust = new Color(255, 40, 10);
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMin = 1f;
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMax = 50f;
            // EDIT DEFAULTS
            //ThrusterDataInstance.thrustData.ErrorColor = Color.Magenta;
            //ThrusterDataInstance.thrustData.CurrentColor = Color.Magenta;

            // DO NOT EDIT
            if (!ThrusterDataInstance.SavedThrusterData.ContainsKey(ThrusterDataInstance.MySubtypeID))
                ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID] = new List<ThrusterDataHandler.ThrusterData>();
            ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID].Add(ThrusterDataInstance.thrustData);
            // ============================================================== COPY BLOCK END


            // ============================================================== COPY BLOCK START
            // DO NOT EDIT
            ThrusterDataInstance.thrustData = new ThrusterDataHandler.ThrusterData();

            // EDIT THRUSTER
            ThrusterDataInstance.MySubtypeID = "LargeBlockLargeThrust";
            // EDIT MATERIAL
            ThrusterDataInstance.thrustData.EmissiveMaterialName = "Emissive";
            // EDIT STATIC
            ThrusterDataInstance.thrustData.OnColor = new Color(0, 20, 255);
            ThrusterDataInstance.thrustData.OffColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonWorkingColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonFunctionalColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.ThrusterOn_EmissiveMultiplier = 10f;
            ThrusterDataInstance.thrustData.ThrusterOff_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNotWorking_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNonFunctional_EmissiveMultiplier = 0f;
            // EDIT DYNAMIC
            ThrusterDataInstance.thrustData.ChangeColorByThrustOutput = true;
            ThrusterDataInstance.thrustData.AntiFlickerThreshold = 0.01f;
            ThrusterDataInstance.thrustData.ColorAtMaxThrust = new Color(255, 40, 10);
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMin = 1f;
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMax = 50f;
            // EDIT DEFAULTS
            //ThrusterDataInstance.thrustData.ErrorColor = Color.Magenta;
            //ThrusterDataInstance.thrustData.CurrentColor = Color.Magenta;

            // DO NOT EDIT
            if (!ThrusterDataInstance.SavedThrusterData.ContainsKey(ThrusterDataInstance.MySubtypeID))
                ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID] = new List<ThrusterDataHandler.ThrusterData>();
            ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID].Add(ThrusterDataInstance.thrustData);
            // ============================================================== COPY BLOCK END

            // ============================================================== COPY BLOCK START
            // DO NOT EDIT
            ThrusterDataInstance.thrustData = new ThrusterDataHandler.ThrusterData();

            // EDIT THRUSTER
            ThrusterDataInstance.MySubtypeID = "LargeBlockLargeThrust";
            // EDIT MATERIAL
            ThrusterDataInstance.thrustData.EmissiveMaterialName = "EmissiveCustom1";
            // EDIT STATIC
            ThrusterDataInstance.thrustData.OnColor = new Color(0, 20, 255);
            ThrusterDataInstance.thrustData.OffColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonWorkingColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.NonFunctionalColor = new Color(0, 0, 0);
            ThrusterDataInstance.thrustData.ThrusterOn_EmissiveMultiplier = 10f;
            ThrusterDataInstance.thrustData.ThrusterOff_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNotWorking_EmissiveMultiplier = 0f;
            ThrusterDataInstance.thrustData.ThrusterNonFunctional_EmissiveMultiplier = 0f;
            // EDIT DYNAMIC
            ThrusterDataInstance.thrustData.ChangeColorByThrustOutput = true;
            ThrusterDataInstance.thrustData.AntiFlickerThreshold = 0.01f;
            ThrusterDataInstance.thrustData.ColorAtMaxThrust = new Color(255, 40, 10);
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMin = 1f;
            ThrusterDataInstance.thrustData.MaxThrust_EmissiveMultiplierMax = 50f;
            // EDIT DEFAULTS
            //ThrusterDataInstance.thrustData.ErrorColor = Color.Magenta;
            //ThrusterDataInstance.thrustData.CurrentColor = Color.Magenta;

            // DO NOT EDIT
            if (!ThrusterDataInstance.SavedThrusterData.ContainsKey(ThrusterDataInstance.MySubtypeID))
                ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID] = new List<ThrusterDataHandler.ThrusterData>();
            ThrusterDataInstance.SavedThrusterData[ThrusterDataInstance.MySubtypeID].Add(ThrusterDataInstance.thrustData);
            // ============================================================== COPY BLOCK END
        }

        // ==========================
        // END OF CHANGABLE VARIABLES
        // ==========================
    }
}
