using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.Entities;
using Sandbox.Game.Gui;
using Sandbox.Game;
using VRage.Common.Utils;
using VRageMath;
using VRage;
using VRage.Game;
using VRage.ObjectBuilders;
using VRage.Game.Components;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.ModAPI;
using VRage.Utils;
using VRage.Library.Utils;
using VRage.Game.ModAPI;
using Sandbox.Game.EntityComponents;
using VRage.Input;
using Sandbox.Game.GameSystems;
using VRage.Game.VisualScripting;
using Sandbox.Game.World;
using Sandbox.Game.Components;
using VRageRender.Animations;
using SpaceEngineers.Game.ModAPI;
using ProtoBuf;
using BlendTypeEnum = VRageRender.MyBillboard.BlendTypeEnum;
using Sandbox.ModAPI.Weapons;
using System.ComponentModel.Design;
using PSYCHO;
using MyVisualScriptLogicProvider = Sandbox.Game.MyVisualScriptLogicProvider;

using Sandbox.Game.Lights;
using VRage.Game.Entity;
using VRage.Game.ModAPI.Interfaces;
using VRage.Game.ObjectBuilders.Definitions;

using IMyControllableEntity = Sandbox.Game.Entities.IMyControllableEntity;
using Sandbox.Game.Entities.Character.Components;
using VRage.Game.SessionComponents;
using System.Data;
using VRage.Game.VisualScripting.Missions;

namespace PSYCHO.ThrusterVisualHandlerInit
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]

    public class DataInitLogic : MySessionComponentBase
    {
        public static bool DoOnce = false;

        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            Setup();
        }

        public override void LoadData()
        {

        }

        public void Setup()
        {
            if (!DoOnce)
            {
                MyAPIGateway.Utilities.ShowNotification("Setup.", 10000);
                DoOnce = true;

                ThrusterVisualHandlerData.UserData MyUserData = new PSYCHO.ThrusterVisualHandlerData.UserData();

                MyUserData.ConstructThrusterData();
            }
        }
    }
}
