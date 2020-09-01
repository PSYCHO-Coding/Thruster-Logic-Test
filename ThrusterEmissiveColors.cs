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

using Sandbox.Game.Lights;
using VRageRender.Lights;

using System.Linq;
using BulletXNA;

//using PSYCHO.ThrusterVisualHandlerUserSettings;
using PSYCHO.ThrusterVisualHandlerData;

namespace PSYCHO.ThrusterEmissiveColors
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Thrust), false)]

    public class ThrusterEmissiveColorsLogic : MyGameLogicComponent
    {
        // DO NOT USE THIS!
        bool EXPERIMENTAL = false;

        IMyThrust block;
        string blockSubtypeID = "";
        MyEntitySubpart subpart;
        string subpartName = "Empty";

        ThrusterDataHandler ThrusterDataInstance => ThrusterDataHandler.ThrusterDataInstance;
        private PSYCHO.ThrusterVisualHandlerUserSettings.UserData MyUserData = new PSYCHO.ThrusterVisualHandlerUserSettings.UserData();

        bool OneEmissiveMaterial = false;

        List<string> MaterialNames = new List<string>();
        
        List<ThrusterDataHandler.ThrusterData> MyThrusterData = new List<ThrusterDataHandler.ThrusterData>();
        List<ThrusterDataHandler.ThrusterData> StaticThrusterData = new List<ThrusterDataHandler.ThrusterData>();
        List<ThrusterDataHandler.ThrusterData> DynamicThrusterData = new List<ThrusterDataHandler.ThrusterData>();

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

        Dictionary<string, IMyModelDummy> ModelDummy = new Dictionary<string, IMyModelDummy>();
        Vector3D LightInnerDummyPos;
        Vector3D LightOuterDummyPos;
        int DummyCount = 0;

        Vector4 DefaultFlameIdleColor;
        Vector4 DefaultFlameFullColor;

        ThrusterDataHandler.ThrusterData DynamicData;

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

            if (EXPERIMENTAL)
            {
                var blockDefinition = block.SlimBlock.BlockDefinition as MyThrustDefinition;

                DefaultFlameIdleColor = blockDefinition.FlameIdleColor;
                DefaultFlameFullColor = blockDefinition.FlameFullColor;

                block.Model.GetDummies(ModelDummy);
                if (ModelDummy.ContainsKey("flame_light_inner"))
                {
                    DummyCount++;
                    LightInnerDummyPos = ModelDummy["flame_light_inner"].Matrix.Translation;
                }
                if (ModelDummy.ContainsKey("flame_light_outer"))
                {
                    DummyCount++;
                    LightOuterDummyPos = ModelDummy["flame_light_outer"].Matrix.Translation;
                }
            }

            //subpart = block.GetSubpart(subpartName);
            block.TryGetSubpart(subpartName, out subpart);

            MyThrusterData = ThrusterDataInstance.GetThrusterData(blockSubtypeID);
            EmissiveMaterialCount = MyThrusterData.Count;

            foreach (var data in MyThrusterData)
            {
                PrepData(data);
                if (ChangeColorByThrustOutput)
                {
                    // Load data as new instance, set defaults where needed, check for errors or missing values and set to defaults as well.
                    DynamicData = new ThrusterDataHandler.ThrusterData();

                    DynamicData.EmissiveMaterialName = data.EmissiveMaterialName;
                    DynamicData.OnColor = data.OnColor;
                    DynamicData.OffColor = data.OffColor;
                    DynamicData.NotWorkingColor = data.NotWorkingColor;
                    DynamicData.NonFunctionalColor = data.NonFunctionalColor;
                    DynamicData.ThrusterOnEmissiveMultiplier = data.ThrusterOnEmissiveMultiplier;
                    DynamicData.ThrusterOffEmissiveMultiplier = data.ThrusterOffEmissiveMultiplier;
                    DynamicData.ThrusterNotWorkingEmissiveMultiplier = data.ThrusterNotWorkingEmissiveMultiplier;
                    DynamicData.ThrusterNonFunctionalEmissiveMultiplier = data.ThrusterNonFunctionalEmissiveMultiplier;
                    DynamicData.ChangeColorByThrustOutput = data.ChangeColorByThrustOutput;
                    DynamicData.AntiFlickerThreshold = data.AntiFlickerThreshold;
                    DynamicData.ColorAtMaxThrust = data.ColorAtMaxThrust;
                    DynamicData.MaxThrust_EmissiveMultiplierMin = data.MaxThrust_EmissiveMultiplierMin;
                    DynamicData.MaxThrust_EmissiveMultiplierMax = data.MaxThrust_EmissiveMultiplierMax;
                    DynamicData.ErrorColor = data.ErrorColor;
                    DynamicData.CurrentColor = data.CurrentColor;

                    DynamicData.ActiveColor = data.OnColor;
                    DynamicData.InactiveColor = data.NonFunctionalColor;
                    DynamicData.ActiveGlow = data.ThrusterOnEmissiveMultiplier;
                    DynamicData.InactiveGlow = data.ThrusterNonFunctionalEmissiveMultiplier;
                    DynamicData.ThrusterStatus = 0f;
                    DynamicData.ThrusterStrength = 0f;

                    if (data.ThrusterStatusRampUp == 0f)
                        data.ThrusterStatusRampUp = 0.005f;
                    DynamicData.ThrusterStatusRampUp = data.ThrusterStatusRampUp;

                    if (data.ThrusterStatusRampDown == 0f)
                        data.ThrusterStatusRampDown = 0.005f;
                    DynamicData.ThrusterStatusRampDown = data.ThrusterStatusRampDown;

                    if (data.ThrusterStrengthRampUp == 0f)
                        data.ThrusterStrengthRampUp = 0.005f;
                    DynamicData.ThrusterStrengthRampUp = data.ThrusterStrengthRampUp;

                    if (data.ThrusterStrengthRampDown == 0f)
                        data.ThrusterStrengthRampDown = 0.005f;
                    DynamicData.ThrusterStrengthRampDown = data.ThrusterStrengthRampDown;

                    if (data.ThrusterOffRampDown == 0f)
                        data.ThrusterOffRampDown = 0.005f;
                    DynamicData.ThrusterOffRampDown = data.ThrusterOffRampDown;

                    if (data.ThrusterNotWorkingRampDown == 0f)
                        data.ThrusterNotWorkingRampDown = 0.005f;
                    DynamicData.ThrusterNotWorkingRampDown = data.ThrusterNotWorkingRampDown;

                    if (data.ThrusterNonFunctionalRampDown == 0f)
                        data.ThrusterNonFunctionalRampDown = 0.005f;
                    DynamicData.ThrusterNonFunctionalRampDown = data.ThrusterNonFunctionalRampDown;

                    DynamicThrusterData.Add(DynamicData);
                }
                else
                {
                    StaticThrusterData.Add(data);
                    CheckAndSetEmissives();
                }
            }

            if (DynamicThrusterData.Any())
                NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;

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

            if (EXPERIMENTAL)
            {
                if (_light != null)
                {
                    MyLights.RemoveLight(_light);
                    _light = null;
                }
                if (_lightInner != null)
                {
                    MyLights.RemoveLight(_lightInner);
                    _lightInner = null;
                }
                if (_lightOuter != null)
                {
                    MyLights.RemoveLight(_lightOuter);
                    _lightOuter = null;
                }
            }


            block = null;
            subpart = null;

            NeedsUpdate = MyEntityUpdateEnum.NONE;
        }



        public void IsWorkingChanged(IMyCubeBlock block)
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

        void PrepData(ThrusterDataHandler.ThrusterData data)
        {
            EmissiveMaterialName = data.EmissiveMaterialName;

            OnColor = data.OnColor;
            OffColor = data.OffColor;
            NonWorkingColor = data.NotWorkingColor;
            NonFunctionalColor = data.NonFunctionalColor;

            ThrusterOn_EmissiveMultiplier = data.ThrusterOnEmissiveMultiplier;
            ThrusterOff_EmissiveMultiplier = data.ThrusterOffEmissiveMultiplier;
            ThrusterNotWorking_EmissiveMultiplier = data.ThrusterNotWorkingEmissiveMultiplier;
            ThrusterNonFunctional_EmissiveMultiplier = data.ThrusterNonFunctionalEmissiveMultiplier;

            ChangeColorByThrustOutput = data.ChangeColorByThrustOutput;
            AntiFlickerThreshold = data.AntiFlickerThreshold;
            ColorAtMaxThrust = data.ColorAtMaxThrust;
            MaxThrust_EmissiveMultiplierMin = data.MaxThrust_EmissiveMultiplierMin;
            MaxThrust_EmissiveMultiplierMax = data.MaxThrust_EmissiveMultiplierMax;

            //ErrorColor = data.ErrorColor;
            //CurrentColor = data.CurrentColor;
        }



        // Handle static color changes
        void CheckAndSetEmissives()
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
                //HandleEmissives();
            }
            else
            {
                ThrustPercent = block.CurrentThrust / block.MaxThrust;

                for (int i = 0; i < DynamicThrusterData.Count; i++)
                {
                    HandleEmissives(i);
                }

                ThrustPercentLast = ThrustPercent;
                /*
                foreach (var data in DynamicThrusterData)
                {
                    MaterialAppliedCount++;
                    //PrepData(data);
                    HandleEmissives(data);
                }
                */
            }
        }

        float ThrustPercent = 0f;
        float ThrustPercentLast = 0f;
        //float glow;
        float CurrentEmissiveMultiplier = 0f;
        int MaterialAppliedCount = 0;

        Color ActiveColor = Color.Black;
        Color InactiveColor = Color.Black;

        float thrustPercentLast = 0f;

        //float ThrusterStatus = 0f;
        //float ThrusterStrength = 0f;
        //public void HandleEmissives(UserData.ThrusterData data)

        // OPTIMIZE/SIMPLIFY
        void HandleEmissives(int index)
        {
            if (block.IsFunctional && block.IsWorking && block.Enabled)
            {
                DynamicThrusterData[index].ThrusterStatus = MathHelper.Clamp(DynamicThrusterData[index].ThrusterStatus + DynamicThrusterData[index].ThrusterStatusRampUp, 0f, 1f);

                if (DynamicThrusterData[index].ThrusterStatus == 1)
                {
                    if (ThrustPercent > ThrustPercentLast && DynamicThrusterData[index].ThrusterStrength < ThrustPercent)
                    {
                        DynamicThrusterData[index].ThrusterStrength = MathHelper.Clamp((DynamicThrusterData[index].ThrusterStrength + DynamicThrusterData[index].ThrusterStrengthRampUp), 0f, 1f);
                    }
                    else if (ThrustPercent < ThrustPercentLast && DynamicThrusterData[index].ThrusterStrength > ThrustPercent)
                    {
                        DynamicThrusterData[index].ThrusterStrength = MathHelper.Clamp((DynamicThrusterData[index].ThrusterStrength - DynamicThrusterData[index].ThrusterStrengthRampDown), 0f, 1f);
                    }
                    else
                    {
                        if (DynamicThrusterData[index].ThrusterStrength < ThrustPercent)
                        {
                            DynamicThrusterData[index].ThrusterStrength = MathHelper.Clamp((DynamicThrusterData[index].ThrusterStrength + DynamicThrusterData[index].ThrusterStrengthRampUp), 0f, ThrustPercent);
                        }
                        else if (DynamicThrusterData[index].ThrusterStrength > ThrustPercent)
                        {
                            DynamicThrusterData[index].ThrusterStrength = MathHelper.Clamp((DynamicThrusterData[index].ThrusterStrength - DynamicThrusterData[index].ThrusterStrengthRampDown), ThrustPercent, 1f);
                        }
                    }

                    DynamicThrusterData[index].ActiveGlow = MathHelper.Lerp(DynamicThrusterData[index].ThrusterOnEmissiveMultiplier, DynamicThrusterData[index].MaxThrust_EmissiveMultiplierMax, DynamicThrusterData[index].ThrusterStrength);
                    DynamicThrusterData[index].ActiveColor = Color.Lerp(DynamicThrusterData[index].OnColor, DynamicThrusterData[index].ColorAtMaxThrust, DynamicThrusterData[index].ThrusterStrength);
                }
                else
                {
                    DynamicThrusterData[index].ActiveGlow = MathHelper.Lerp(DynamicThrusterData[index].InactiveGlow, DynamicThrusterData[index].ThrusterOnEmissiveMultiplier, DynamicThrusterData[index].ThrusterStatus);
                    DynamicThrusterData[index].ActiveColor = Color.Lerp(DynamicThrusterData[index].InactiveColor, DynamicThrusterData[index].OnColor, DynamicThrusterData[index].ThrusterStatus);
                }

                block.SetEmissiveParts(DynamicThrusterData[index].EmissiveMaterialName, DynamicThrusterData[index].ActiveColor, DynamicThrusterData[index].ActiveGlow);
                if (subpart != null)
                {
                    subpart.SetEmissiveParts(DynamicThrusterData[index].EmissiveMaterialName, DynamicThrusterData[index].ActiveColor, DynamicThrusterData[index].ActiveGlow);
                }

                if (EXPERIMENTAL)
                {
                    if (DummyCount == 2)
                    {
                        LightingHandler(ref _lightInner, Vector3D.Transform(LightInnerDummyPos, block.WorldMatrix), 0.7f, Color.Orange, 1f, 2f, DynamicThrusterData[index].ThrusterStrength);
                        LightingHandler(ref _lightOuter, Vector3D.Transform(LightOuterDummyPos, block.WorldMatrix), 1f, Color.Red, 1f, 10f, DynamicThrusterData[index].ThrusterStrength);
                    }
                }
            }
            else
            {
                //DynamicThrusterData[index].ThrusterStatus = MathHelper.Clamp(DynamicThrusterData[index].ThrusterStatus - DynamicThrusterData[index].ThrusterStatusRampDown, 0f, 1f);

                DynamicThrusterData[index].ThrusterStrength = 0f;

                if (!block.IsFunctional)
                {
                    DynamicThrusterData[index].ThrusterStatus = MathHelper.Clamp(DynamicThrusterData[index].ThrusterStatus - DynamicThrusterData[index].ThrusterNonFunctionalRampDown, 0f, 1f);
                    DynamicThrusterData[index].InactiveGlow = MathHelper.Lerp(DynamicThrusterData[index].ThrusterNonFunctionalEmissiveMultiplier, DynamicThrusterData[index].ActiveGlow, DynamicThrusterData[index].ThrusterStatus);
                    DynamicThrusterData[index].InactiveColor = Color.Lerp(DynamicThrusterData[index].NonFunctionalColor, DynamicThrusterData[index].ActiveColor, DynamicThrusterData[index].ThrusterStatus);
                }
                else if (!block.Enabled)
                {
                    DynamicThrusterData[index].ThrusterStatus = MathHelper.Clamp(DynamicThrusterData[index].ThrusterStatus - DynamicThrusterData[index].ThrusterOffRampDown, 0f, 1f);
                    DynamicThrusterData[index].InactiveGlow = MathHelper.Lerp(DynamicThrusterData[index].ThrusterOffEmissiveMultiplier, DynamicThrusterData[index].ActiveGlow, DynamicThrusterData[index].ThrusterStatus);
                    DynamicThrusterData[index].InactiveColor = Color.Lerp(DynamicThrusterData[index].OffColor, DynamicThrusterData[index].ActiveColor, DynamicThrusterData[index].ThrusterStatus);
                }
                else
                {
                    DynamicThrusterData[index].ThrusterStatus = MathHelper.Clamp(DynamicThrusterData[index].ThrusterStatus - DynamicThrusterData[index].ThrusterNotWorkingRampDown, 0f, 1f);
                    DynamicThrusterData[index].InactiveGlow = MathHelper.Lerp(DynamicThrusterData[index].ThrusterNotWorkingEmissiveMultiplier, DynamicThrusterData[index].ActiveGlow, DynamicThrusterData[index].ThrusterStatus);
                    DynamicThrusterData[index].InactiveColor = Color.Lerp(DynamicThrusterData[index].NotWorkingColor, DynamicThrusterData[index].ActiveColor, DynamicThrusterData[index].ThrusterStatus);
                }

                block.SetEmissiveParts(DynamicThrusterData[index].EmissiveMaterialName, DynamicThrusterData[index].InactiveColor, DynamicThrusterData[index].InactiveGlow);
                if (subpart != null)
                {
                    subpart.SetEmissiveParts(DynamicThrusterData[index].EmissiveMaterialName, DynamicThrusterData[index].InactiveColor, DynamicThrusterData[index].InactiveGlow);
                }

                if (DynamicThrusterData[index].ThrusterStatus == 0)
                {
                    DynamicThrusterData[index].ThrusterStrength = 0f;
                }
            }

            // PROTOTYPE
            if (EXPERIMENTAL)
            {
                var flameColor = Color.Lerp(new Color(0.2745098f, 0.4090196f, 0.6505882f, 0.75f), Color.OrangeRed, DynamicThrusterData[index].ThrusterStrength);
                FlameHandler(flameColor);
            }
        }

        MyLight _light;
        MyLight _lightInner;
        MyLight _lightOuter;
        void LightingHandler()
        {
            if (_light == null)
            {
                _light = new MyLight();

                //These control the light settings on spawn.
                var lightRange = 2.5f; //Range of light
                var lightIntensity = 5.0f; //Light intensity
                var lightFalloff = 0.5f; //Light falloff
                //var lightAdjustment = 0.0f;
                var lightPosition = block.WorldMatrix.Translation + block.WorldMatrix.Forward * 0.2; //Sets the light to the center of the block you are spawning it on, if you need it elsehwere you will need help.

                _light = MyLights.AddLight(); //Ignore - adds the light to the games lighting system
                _light.Start(lightPosition, Color.Red, lightRange, ""); // Ignore- Determines the lights position, initial color and initial range.
                _light.Intensity = lightIntensity; //Ignore - sets light intensity from above values.
                _light.Falloff = lightFalloff; //Ignore - sets light fall off from above values.
                //_light.PointLightOffset = lightOffset; //Ignore - sets light offset from above values.
                _light.LightOn = true; //Ignore - turns light on
            }
            else
            {
                //_light.Intensity = 10 * ThrusterStrength;
                _light.Position = block.WorldMatrix.Translation + block.WorldMatrix.Forward; //Updates the lights position constantly. You'll need help if you want it somewhere else.
                _light.UpdateLight(); //Ignore - tells the game to update the light.
            }
        }

        //public void LightingHandler(ref MyLight light, float positionOffset, float lightRange, Color color)
        void LightingHandler(ref MyLight light, Vector3D position, float lightRange, Color color, float falloff, float intensity, float mult)
        {
            if (light == null)
            {
                light = new MyLight();

                //These control the light settings on spawn.
                //var lightRange = 2.5f; //Range of light
                var lightIntensity = 5.0f; //Light intensity
                //var lightFalloff = 0.5f; //Light falloff
                var lightFalloff = falloff; //Light falloff
                //var lightAdjustment = 0.0f;
                var lightPosition = position; //Sets the light to the center of the block you are spawning it on, if you need it elsehwere you will need help.

                light = MyLights.AddLight(); //Ignore - adds the light to the games lighting system
                light.Start(lightPosition, color, lightRange, ""); // Ignore- Determines the lights position, initial color and initial range.
                light.Intensity = lightIntensity; //Ignore - sets light intensity from above values.
                light.Falloff = lightFalloff; //Ignore - sets light fall off from above values.
                //_light.PointLightOffset = lightOffset; //Ignore - sets light offset from above values.
                light.LightOn = true; //Ignore - turns light on
            }
            else
            {
                light.Intensity = intensity * mult;
                light.Position = position; //Updates the lights position constantly. You'll need help if you want it somewhere else.
                light.UpdateLight(); //Ignore - tells the game to update the light.
            }
        }

        void FlameHandler(Color color)
        {
            if (Entity == null)
                return;

            var thrust = block as MyThrust;
            if (thrust == null || thrust.CubeGrid.Physics == null)
                return;

            uint renderObjectID = Entity.Render.GetRenderObjectID();
            if (renderObjectID == 4294967295u)
                return;

            MyThrustDefinition blockDefinition = thrust.BlockDefinition;

            //if (thrust.CurrentStrength > 0.001f)

            //blockDefinition.FlameIdleColor = color;
            blockDefinition.FlameFullColor = color;
            ((MyRenderComponentThrust)thrust.Render).UpdateFlameAnimatorData();
        }

        void FlameHandler()
        {
            if (Entity == null)
                return;

            var thrust = block as MyThrust;
            if (thrust == null || thrust.CubeGrid.Physics == null)
                return;

            uint renderObjectID = Entity.Render.GetRenderObjectID();
            if (renderObjectID == 4294967295u)
                return;

            MyThrustDefinition blockDefinition = thrust.BlockDefinition;

            //blockDefinition.FlameIdleColor = DefaultFlameIdleColor;
            blockDefinition.FlameFullColor = DefaultFlameFullColor;

            ((MyRenderComponentThrust)thrust.Render).UpdateFlameAnimatorData();
        }
    }
}
