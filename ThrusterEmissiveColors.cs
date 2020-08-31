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
using System.Linq;

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
        public List<UserData.ThrusterData> StaticThrusterData = new List<UserData.ThrusterData>();
        public List<UserData.ThrusterData> DynamicThrusterData = new List<UserData.ThrusterData>();

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

        int EmissiveMaterialCount = 0;

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
            EmissiveMaterialCount = ThrusterData.Count;

            if (EmissiveMaterialCount == 1)
            {
                OneEmissiveMaterial = true;
                PrepData(ThrusterData[0]);

                if (ChangeColorByThrustOutput)
                    NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
                else
                    CheckAndSetEmissives();
            }
            else
            {
                foreach (var data in ThrusterData)
                {
                    PrepData(data);
                    if (ChangeColorByThrustOutput)
                    {
                        DynamicThrusterData.Add(data);
                    }
                    else
                    {
                        StaticThrusterData.Add(data);
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
                return;
            }

            //subpart = block.GetSubpart(subpartName);
            block.TryGetSubpart(subpartName, out subpart);

            if (OneEmissiveMaterial)
            {
                CheckAndSetEmissives();
            }
            else
            {
                if (StaticThrusterData.Any())
                {
                    foreach (var data in StaticThrusterData)
                    {
                        PrepData(data);
                        CheckAndSetEmissives();
                    }
                }

                if (DynamicThrusterData.Any())
                {
                    NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
                }
            }
        }



        public void PropertiesChanged(IMyTerminalBlock block)
        {
            if (block == null)
            {
                return;
            }

            //subpart = block.GetSubpart(subpartName);
            block.TryGetSubpart(subpartName, out subpart);

            if (OneEmissiveMaterial)
            {
                CheckAndSetEmissives();
            }
            else
            {
                if (StaticThrusterData.Any())
                {
                    foreach (var data in StaticThrusterData)
                    {
                        PrepData(data);
                        CheckAndSetEmissives();
                    }
                }

                if (DynamicThrusterData.Any())
                {
                    NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
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
                foreach (var data in DynamicThrusterData)
                {
                    MaterialAppliedCount++;
                    PrepData(data);
                    HandleEmissives();
                }

                MaterialAppliedCount = 0;
            }
        }



        float glow;
        float CurrentEmissiveMultiplier = 0f;
        int MaterialAppliedCount = 0;
        public void HandleEmissives()
        {
            if (block.IsFunctional && block.IsWorking && block.Enabled)
            {
                MaterialAppliedCount = 0;

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
                Color color = ErrorColor;
                Color colorApply = ErrorColor;
                float mult = 1f;

                if (!block.IsFunctional)
                {
                    color = NonFunctionalColor;
                    mult = ThrusterNonFunctional_EmissiveMultiplier + (MaxThrust_EmissiveMultiplierMax - ThrusterNonFunctional_EmissiveMultiplier);
                }
                else if (!block.Enabled)
                {
                    color = OffColor;
                    mult = ThrusterOff_EmissiveMultiplier + (MaxThrust_EmissiveMultiplierMax - ThrusterOff_EmissiveMultiplier);
                }
                else
                {
                    color = NonWorkingColor;
                    mult = ThrusterNotWorking_EmissiveMultiplier + (MaxThrust_EmissiveMultiplierMax - ThrusterNotWorking_EmissiveMultiplier);
                }

                float minRamp = 0.005f;

                if (glow > 0)
                    glow -= minRamp;

                colorApply = Color.Lerp(color, ColorAtMaxThrust, glow);
                CurrentEmissiveMultiplier = mult * glow;

                block.SetEmissiveParts(EmissiveMaterialName, colorApply, CurrentEmissiveMultiplier);
                if (subpart != null)
                {
                    subpart.SetEmissiveParts(EmissiveMaterialName, colorApply, CurrentEmissiveMultiplier);
                }

                if (glow <= 0)
                {
                    if (!block.IsFunctional)
                    {
                        color = NonFunctionalColor;
                        mult = ThrusterNonFunctional_EmissiveMultiplier;
                    }
                    else if (!block.Enabled)
                    {
                        color = OffColor;
                        mult = ThrusterOff_EmissiveMultiplier;
                    }
                    else
                    {
                        color = NonWorkingColor;
                        mult = ThrusterNotWorking_EmissiveMultiplier;
                    }

                    CurrentEmissiveMultiplier = mult;

                    block.SetEmissiveParts(EmissiveMaterialName, color, mult);
                    if (subpart != null)
                    {
                        subpart.SetEmissiveParts(EmissiveMaterialName, color, mult);
                    }

                    if (MaterialAppliedCount < EmissiveMaterialCount)
                        return;

                    NeedsUpdate = MyEntityUpdateEnum.NONE;
                }
            }
        }
    }
}
