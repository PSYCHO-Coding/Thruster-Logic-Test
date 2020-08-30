using Sandbox.ModAPI;
using System.Linq;
using System.Threading.Tasks;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using SpaceEngineers.Game.ModAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;

// IMPORTANT
// Leftover upgrade handling code from the original mod 'Vanilla Flavored Thrusters' by Malogny which I simply commented out as it's not working as far as I'm aware and also don't have exit code.
// Will fix it some other time.
// -PSYCHO-

namespace ThrustUpgrade
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Thrust), false)]
    public class MyThrusterUpgradeLogic : MyGameLogicComponent
    {
        /*
        private IMyThrust m_thrust;
        private IMyCubeBlock m_parent;
        private MyObjectBuilder_EntityBase m_objectBuilder = null;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);

            m_thrust = Entity as IMyThrust;
            m_parent = Entity as IMyCubeBlock;

            //m_thrust.SetEmissiveParts("Emissive", Color.Black, 1f);

            m_parent.AddUpgradeValue("Thrust", 0f);

            m_objectBuilder = objectBuilder;

            //m_parent.OnUpgradeValuesChanged += OnUpgradeValuesChanged;

            //NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
        }

        public override void UpdateBeforeSimulation()
        {
            if (m_thrust == null)
                return;
            m_thrust.SetEmissiveParts("Emissive", Color.AliceBlue, 1f);
        }

        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return m_objectBuilder;
        }

        private void OnUpgradeValuesChanged()
        {
            m_thrust.ThrustMultiplier = m_parent.UpgradeValues["Thrust"] + 1f;
            m_thrust.PowerConsumptionMultiplier = m_parent.UpgradeValues["Thrust"] + 1f;
        }
        */
    }
}
