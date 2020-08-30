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

using PSYCHO.ThrusterVisualHandlerData;

// TODO
// * Simplify even more for non-coders.
// * Add separate emissive mat handling with different colors by material name.

// REMARKS
// The code only runs when/while needed. Should be pretty performance friendly.
// However, some tests needed for SP VS MP because those two really don't like each others code. Thanks Keen for ensuring it's never boring... Or easy. :P
// Smack that coder. xD

// CHANGE THE NAMESPACE TO AVOID CONFLICTS!
// For instance, Your Space Engineers moniker, unless it's super common, then use some more unique ID or a combo of your player name and mod name.
// E.g. SomeSuperCoolPlayerNameYouProbablyHave_ModName.ThrusterEmissiveColors.
// FancyJoe_SuperCoolThrusters.ThrusterEmissiveColors
namespace PSYCHO_SuperThrusters.ThrusterEmissiveColors
{
    // CHANGE THE SUBTYPES HERE
    // ADD AS MANY AS YOU NEED
    // E.g.
    // [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Thrust), false, "YourSubTypeHere", "YourOtherSubTypeHere", "AndAnotherThruster", "AndYetAnotherOne")]
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

        //PSYCHO.ThrusterVisualHandlerData.UserData MyUserData = new PSYCHO.ThrusterVisualHandlerData.UserData();
        public List<UserData.ThrusterData> ThrusterData = new List<UserData.ThrusterData>();
        UserData.ThrusterData OneThrusterData;
        /*
        public object this[string propertyName]
        {
            get
            {
                var property = GetType().GetProperty(propertyName);
                return property.GetValue(this, null);
            }
            set
            {
                var property = GetType().GetProperty(propertyName);
                property.SetValue(this, value, null);
            }
        }
        */

        /*
        public object GetThrustData(string propertyName)
        {
            var property = UserData.GetType().GetProperty(propertyName);
            return property.GetValue(this, null);
        }
        */

        // ========================

        // ========================

        // MATERIAL NAMES
        string EmissiveMaterialName = "Emissive";

        // CHANGE THESE TO DESIRED IN RGB 0-255
        Color OnColor            = new Color(0, 20, 255); // On color, when thruster is ON.
        Color OffColor           = new Color(0, 0, 0);    // Off color, when thruster is OFF.
        Color NonWorkingColor    = new Color(0, 0, 0);    // When the block is not working, like no power.
        Color NonFunctionalColor = new Color(0, 0, 0);    // When the block is damaged, like from impact or weapon fire.

        // GLOW STRENGTH MULTIPLIERS
        float ThrusterOn_EmissiveMultiplier            = 10f;
        float ThrusterOff_EmissiveMultiplier           = 0f;
        float ThrusterNotWorking_EmissiveMultiplier    = 0f;
        float ThrusterNonFunctional_EmissiveMultiplier = 0f;

        bool ChangeColorByThrustOutput = true;           // Change this for static or dynamic colors.
        float AntiFlickerThreshold = 0.01f;              // If the emissive flicker, increase the threshold.
        Color ColorAtMaxThrust = new Color(255, 40, 10); // Color to reach at max thrust, otherwise 'OnColor' will be used when thruster is idle.
        float MaxThrust_EmissiveMultiplierMin = 1f;
        float MaxThrust_EmissiveMultiplierMax = 50f;

        // CHANGE THIS ONLY IF IT CLASHES WITH YOUR COLORS
        // Set these defaults to a color easy to spot if something isn't working correctly.
        Color ErrorColor = Color.Magenta;
        Color CurrentColor = Color.Magenta;

        // END
        // ============================================================================================================================

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            block = (IMyThrust)Entity;

            if (block == null)
                return;

            if (MyUserData.ThrusterSubtypeIDs == null)
            {
                //@MyAPIGateway.Utilities.ShowNotification("Hashset was null!", 10000);
            }

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

            block.TryGetSubpart(subpartName, out subpart);
            //subpart = block.GetSubpart(subpartName);

            ThrusterData = MyUserData.GetThrusterData(blockSubtypeID);

            //@MyAPIGateway.Utilities.ShowNotification(ThrusterData.Count.ToString(), 10000);
            //return;

            if (ThrusterData.Count == 1)
            {
                OneEmissiveMaterial = true;
                OneThrusterData = ThrusterData[0];
            }

            //MyAPIGateway.Utilities.ShowNotification(ThrusterData.Count.ToString(), 10000);

            /*
            if (OneEmissiveMaterial)
            {
                var data = ThrusterData[0];
                PrepData(data);

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

                ErrorColor = data.ErrorColor;
                CurrentColor = data.CurrentColor;
            }
            */

            //var ThrusterData = UserData.GetThrusterData();

            /*
                foreach (var person in people)
                {
                    Console.WriteLine(person.ID);
                    Console.WriteLine(person.Name);
                    Console.WriteLine(person.SomeOtherValue);
                }

            for (int i = 0; i < ThrusterData.Count; i++)
            {
                //Console.WriteLine(ThrusterData.Item1[i]);
                //Console.WriteLine(ThrusterData.Item2[i]);
            }

            foreach (var material in ThrusterData)
            {
                //MaterialNames.Add();
            }
            */

            if (ChangeColorByThrustOutput)
            {
                NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
            }
            else
            {
                if (OneEmissiveMaterial)
                {
                    //var data = ThrusterData[0];
                    PrepData(OneThrusterData);
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
            subpart = block.GetSubpart(subpartName);

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
            subpart = block.GetSubpart(subpartName);

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

            ErrorColor = data.ErrorColor;
            CurrentColor = data.CurrentColor;
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
                //@MyAPIGateway.Utilities.ShowNotification("foreach ThrusterData", 1000);
                foreach (var data in ThrusterData)
                {
                    //@MyAPIGateway.Utilities.ShowNotification("foreach data", 1000);
                    PrepData(data);
                    HandleEmissives();
                }
            }
        }



        public void HandleEmissives()
        {
            //@MyAPIGateway.Utilities.ShowNotification("HandleEmissives()", 1000);
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
                    CurrentEmissiveMultiplier = MaxThrust_EmissiveMultiplierMin + (MaxThrust_EmissiveMultiplierMax - MaxThrust_EmissiveMultiplierMin) * glow;

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
                    glow -= 0.005f;

                if (CurrentEmissiveMultiplier > 0)
                    CurrentEmissiveMultiplier -= 0.005f;

                Color color = ErrorColor;

                if (!block.IsWorking)
                {
                    color = Color.Lerp(CurrentColor, NonWorkingColor, glow);
                }
                else if (block.Enabled)
                {
                    color = Color.Lerp(CurrentColor, OnColor, glow);
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
