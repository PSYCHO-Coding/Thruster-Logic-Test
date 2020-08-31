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

namespace PSYCHO.ThrusterVisualHandlerData
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]

    public class UserData : MySessionComponentBase
    {
        // DO NOT EDIT THIS!
        public static UserData UserDataInstance;

        public override void LoadData()
        {
            UserDataInstance = this;

            ConstructThrusterData();
        }

        protected override void UnloadData()
        {
            UserDataInstance = null;
        }

        string MySubtypeID;
        public Dictionary<string, List<ThrusterData>> MyThrusterData = new Dictionary<string, List<ThrusterData>>();
        ThrusterData thrustData;

        public List<ThrusterData> GetThrusterData(string _subtypeID)
        {
            if (!MyThrusterData.ContainsKey(_subtypeID))
            {
                MyAPIGateway.Utilities.ShowNotification("GetThrusterData was NULL.", 10000);
                return null;
            }

            return MyThrusterData[_subtypeID];
        }

        public class ThrusterData
        {
            public string EmissiveMaterialName { get; set; }
            public Color OnColor { get; set; }
            public Color OffColor { get; set; }
            public Color NonWorkingColor { get; set; }
            public Color NonFunctionalColor { get; set; }
            public float ThrusterOn_EmissiveMultiplier { get; set; }
            public float ThrusterOff_EmissiveMultiplier { get; set; }
            public float ThrusterNotWorking_EmissiveMultiplier { get; set; }
            public float ThrusterNonFunctional_EmissiveMultiplier { get; set; }
            public bool ChangeColorByThrustOutput { get; set; }
            public float AntiFlickerThreshold { get; set; }
            public Color ColorAtMaxThrust { get; set; }
            public float MaxThrust_EmissiveMultiplierMin { get; set; }
            public float MaxThrust_EmissiveMultiplierMax { get; set; }
            public Color ErrorColor { get; set; }
            public Color CurrentColor { get; set; }
        }

        // ========================
        // USER CHANGABLE VARIABLES
        // ========================

        // Add your thrusters.
        public readonly HashSet<string> ThrusterSubtypeIDs = new HashSet<string>()
        {
            "SuperThruster_Small"
        };

        public void ConstructThrusterData()
        {
            // ==============================================================
            // COPY AND EDIT THIS PER THRUSTER / THRUSTER + EMISSIVE MATERIAL
            // ==============================================================

            // ONLY EDIT VALUES THAT PROPERLY ANOTATED WITH '// EDIT'

            // ============================================================== COPY BLOCK START
            // DO NOT EDIT
            thrustData = new ThrusterData();

            // EDIT THRUSTER
            MySubtypeID = "SuperThruster_Small";
            // EDIT MATERIAL
            thrustData.EmissiveMaterialName = "Emissive";
            // EDIT STATIC
            thrustData.OnColor = new Color(0, 20, 255);
            thrustData.OffColor = new Color(0, 0, 0);
            thrustData.NonWorkingColor = new Color(0, 0, 0);
            thrustData.NonFunctionalColor = new Color(0, 0, 0);
            thrustData.ThrusterOn_EmissiveMultiplier = 10f;
            thrustData.ThrusterOff_EmissiveMultiplier = 0f;
            thrustData.ThrusterNotWorking_EmissiveMultiplier = 0f;
            thrustData.ThrusterNonFunctional_EmissiveMultiplier = 0f;
            // EDIT DYNAMIC
            thrustData.ChangeColorByThrustOutput = true;
            thrustData.AntiFlickerThreshold = 0.01f;
            thrustData.ColorAtMaxThrust = new Color(255, 40, 10);
            thrustData.MaxThrust_EmissiveMultiplierMin = 1f;
            thrustData.MaxThrust_EmissiveMultiplierMax = 50f;
            // EDIT DEFAULTS
            //thrustData.ErrorColor = Color.Magenta;
            //thrustData.CurrentColor = Color.Magenta;

            // DO NOT EDIT
            if (!MyThrusterData.ContainsKey(MySubtypeID))
                MyThrusterData[MySubtypeID] = new List<ThrusterData>();
            MyThrusterData[MySubtypeID].Add(thrustData);
            // ============================================================== COPY BLOCK END
        }

        // ==========================
        // END OF CHANGABLE VARIABLES
        // ==========================
    }
}
