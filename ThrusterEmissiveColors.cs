using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using VRageMath;
using VRage.ObjectBuilders;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.Game.ModAPI;
using VRage.Game.Entity;

using System.Text;
using Sandbox.Definitions;
using Sandbox.Game.Components;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Game;
using VRage.Game.Models;
using VRage.Utils;
using VRageRender;
using VRageRender.Import;

using PSYCHO.ThrusterVisualHandlerData;

namespace PSYCHO_SuperThrusters.ThrusterEmissiveColors
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Thrust), false)]

    public class ThrusterEmissiveColorsLogic : MyGameLogicComponent
    {
        public IMyThrust block;
        public string blockSubtypeID = "";
        public MyEntitySubpart subpart;
        public string subpartName = "Empty";

        UserData MyUserData => UserData.UserDataInstance;

        bool OneEmissiveMaterial = false;

        public List<string> MaterialNames = new List<string>();

        public List<UserData.ThrusterData> ThrusterData = new List<UserData.ThrusterData>();

        string EmissiveMaterialName = "Emissive";

        Color OnColor            = new Color(0, 20, 255);
        Color OffColor           = new Color(0, 0, 0);
        Color NonWorkingColor    = new Color(0, 0, 0);
        Color NonFunctionalColor = new Color(0, 0, 0);

        float ThrusterOn_EmissiveMultiplier            = 10f;
        float ThrusterOff_EmissiveMultiplier           = 0f;
        float ThrusterNotWorking_EmissiveMultiplier    = 0f;
        float ThrusterNonFunctional_EmissiveMultiplier = 0f;

        bool ChangeColorByThrustOutput = true;
        float AntiFlickerThreshold = 0.01f;
        Color ColorAtMaxThrust = new Color(255, 40, 10);
        float MaxThrust_EmissiveMultiplierMin = 1f;
        float MaxThrust_EmissiveMultiplierMax = 50f;

        Color ErrorColor = Color.Magenta;
        Color CurrentColor = Color.Magenta;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            block = (IMyThrust)Entity;

            if (block == null)
                return;

            if (!MyUserData.ThrusterSubtypeIDs.Contains(block.BlockDefinition.SubtypeId))
            {
                block = null;
                return;
            }

            NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            if (block == null) // Null check all the things.
                return;
            
            blockSubtypeID = block.BlockDefinition.SubtypeId;

            //subpart = block.GetSubpart(subpartName);
            block.TryGetSubpart(subpartName, out subpart);

            ThrusterData = MyUserData.GetThrusterData(blockSubtypeID);

            if (ThrusterData.Count == 1)
            {
                OneEmissiveMaterial = true;
            }

            if (ChangeColorByThrustOutput)
            {
                if (OneEmissiveMaterial)
                    PrepData(ThrusterData[0]);
                NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
            }
            else
            {
                if (OneEmissiveMaterial)
                {
                    PrepData(ThrusterData[0]);
                    CheckAndSetEmissives();
                }
                else
                {
                    foreach (var data in ThrusterData)
                    {
                        PrepData(data);
                        CheckAndSetEmissives();
                    }
                }
            }

            // Hook to events.
            block.IsWorkingChanged += IsWorkingChanged;
            block.PropertiesChanged += PropertiesChanged;
        }



        public override void Close()
        {
            if (block == null)
                return;

            // Unhook from events.
            block.IsWorkingChanged -= IsWorkingChanged;
            block.PropertiesChanged -= PropertiesChanged;

            block = null;
            subpart = null;

            NeedsUpdate = MyEntityUpdateEnum.NONE;
        }



        private void IsWorkingChanged(IMyCubeBlock block)
        {
            if (block == null)
            {
                //MyAPIGateway.Utilities.ShowNotification("IsWorkingChanged was null for some reason,");
                return;
            }

            //subpart = block.GetSubpart(subpartName);
            block.TryGetSubpart(subpartName, out subpart);

            if (ChangeColorByThrustOutput)
            {
                NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
                return;
            }

            if (OneEmissiveMaterial)
            {
                //var data = ThrusterData[0];
                //PrepData(OneThrusterData);
                CheckAndSetEmissives();
            }
            else
            {
                foreach (var data in ThrusterData)
                {
                    PrepData(data);
                    CheckAndSetEmissives();
                }
            }
        }



        public void PropertiesChanged(IMyTerminalBlock block)
        {
            if (block == null)
            {
                //MyAPIGateway.Utilities.ShowNotification("PropertiesChanged was null for some reason,");
                return;
            }

            //subpart = block.GetSubpart(subpartName);
            block.TryGetSubpart(subpartName, out subpart);

            if (ChangeColorByThrustOutput)
            {
                NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
                return;
            }

            if (OneEmissiveMaterial)
            {
                //var data = ThrusterData[0];
                //PrepData(OneThrusterData);
                CheckAndSetEmissives();
            }
            else
            {
                foreach (var data in ThrusterData)
                {
                    PrepData(data);
                    CheckAndSetEmissives();
                }
            }
        }

        public void PrepData(UserData.ThrusterData data)
        {
            EmissiveMaterialName = data.EmissiveMaterialName;

            OnColor = data.OnColor;
            OffColor = data.OffColor;
            NonWorkingColor = data.NonWorkingColor;
            NonFunctionalColor = data.NonFunctionalColor;

            ThrusterOn_EmissiveMultiplier = data.ThrusterOn_EmissiveMultiplier;
            ThrusterOff_EmissiveMultiplier = data.ThrusterOff_EmissiveMultiplier;
            ThrusterNotWorking_EmissiveMultiplier = data.ThrusterNotWorking_EmissiveMultiplier;
            ThrusterNonFunctional_EmissiveMultiplier = data.ThrusterNonFunctional_EmissiveMultiplier;

            ChangeColorByThrustOutput = data.ChangeColorByThrustOutput;
            AntiFlickerThreshold = data.AntiFlickerThreshold;
            ColorAtMaxThrust = data.ColorAtMaxThrust;
            MaxThrust_EmissiveMultiplierMin = data.MaxThrust_EmissiveMultiplierMin;
            MaxThrust_EmissiveMultiplierMax = data.MaxThrust_EmissiveMultiplierMax;

            //ErrorColor = data.ErrorColor;
            //CurrentColor = data.CurrentColor;
        }



        // Handle static color changes
        public void CheckAndSetEmissives()
        {
            if (block.IsFunctional)
            {
                CurrentColor = ErrorColor; // Set to error color by default to easily spot an error.
                float mult = 1f;

                if (!block.IsWorking)
                {
                    NeedsUpdate = MyEntityUpdateEnum.NONE;
                    CurrentColor = NonWorkingColor;
                    mult = ThrusterNotWorking_EmissiveMultiplier;
                }
                else if (block.Enabled)
                {
                    CurrentColor = OnColor;
                    mult = ThrusterOn_EmissiveMultiplier;
                }
                else
                {
                    CurrentColor = OffColor;
                    mult = ThrusterOff_EmissiveMultiplier;
                }

                block.SetEmissiveParts(EmissiveMaterialName, CurrentColor, mult);
                if (subpart != null)
                {
                    subpart.SetEmissiveParts(EmissiveMaterialName, CurrentColor, mult);
                }
            }
            else
            {
                if (!ChangeColorByThrustOutput)
                {
                    NeedsUpdate = MyEntityUpdateEnum.NONE;
                    block.SetEmissiveParts(EmissiveMaterialName, NonFunctionalColor, ThrusterNonFunctional_EmissiveMultiplier);
                    if (subpart != null)
                    {
                        subpart.SetEmissiveParts(EmissiveMaterialName, NonFunctionalColor, ThrusterNonFunctional_EmissiveMultiplier);
                    }
                }
            }
        }



        float glow;
        float CurrentEmissiveMultiplier = 0f;
        // Handle dynamic color changes.
        public override void UpdateAfterSimulation()
        {
            if (block == null || block.MarkedForClose || block.Closed)
            {
                NeedsUpdate = MyEntityUpdateEnum.NONE;
                return;
            }

            if (OneEmissiveMaterial)
            {
                //var data = ThrusterData[0];
                //PrepData(OneThrusterData);
                HandleEmissives();
            }
            else
            {
                foreach (var data in ThrusterData)
                {
                    PrepData(data);
                    HandleEmissives();
                }
            }
        }



        public void HandleEmissives()
        {
            if (block.IsFunctional && block.IsWorking && block.Enabled)
            {
                float thrustPercent = block.CurrentThrust / block.MaxThrust;

                if (glow <= 0 && thrustPercent <= AntiFlickerThreshold)
                    glow = 0f;
                else if (glow < thrustPercent)
                    glow += 0.005f;
                else if (glow > thrustPercent)
                    glow -= 0.005f;

                float mult = MaxThrust_EmissiveMultiplierMin + (MaxThrust_EmissiveMultiplierMax - MaxThrust_EmissiveMultiplierMin);
                if (CurrentEmissiveMultiplier < mult)
                    CurrentEmissiveMultiplier += 0.005f;
                else
                    CurrentEmissiveMultiplier = mult * glow;

                CurrentColor = Color.Lerp(OnColor, ColorAtMaxThrust, glow);

                block.SetEmissiveParts(EmissiveMaterialName, CurrentColor, CurrentEmissiveMultiplier);
                if (subpart != null)
                {
                    subpart.SetEmissiveParts(EmissiveMaterialName, CurrentColor, CurrentEmissiveMultiplier);
                }
            }
            else
            {
                if (glow > 0)
                    glow -= 0.01f;

                if (CurrentEmissiveMultiplier > 0)
                    CurrentEmissiveMultiplier -= 0.01f;

                Color color = ErrorColor;

                if (!block.IsFunctional)
                {
                    color = Color.Lerp(CurrentColor, NonFunctionalColor, glow);
                }
                else if (!block.IsWorking)
                {
                    color = Color.Lerp(CurrentColor, NonWorkingColor, glow);
                }
                else
                {
                    color = Color.Lerp(CurrentColor, OffColor, glow);
                }

                block.SetEmissiveParts(EmissiveMaterialName, color, CurrentEmissiveMultiplier);
                if (subpart != null)
                {
                    subpart.SetEmissiveParts(EmissiveMaterialName, color, CurrentEmissiveMultiplier);
                }

                if (glow <= 0 && CurrentEmissiveMultiplier <= 0)
                {
                    glow = 0f;
                    CurrentEmissiveMultiplier = 0f;
                    block.SetEmissiveParts(EmissiveMaterialName, NonFunctionalColor, ThrusterNonFunctional_EmissiveMultiplier);
                    if (subpart != null)
                    {
                        subpart.SetEmissiveParts(EmissiveMaterialName, NonFunctionalColor, ThrusterNonFunctional_EmissiveMultiplier);
                    }

                    NeedsUpdate = MyEntityUpdateEnum.NONE;
                }
            }
        }
    }
}
