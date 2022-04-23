using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.ObjectSystem;

namespace CookieLord
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            mission.AddMissionBehavior(new ArtisanBeerMissionView());
        }
    }

    public class ArtisanBeerMissionView : MissionView 
    {
        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            if (Input.IsKeyPressed(TaleWorlds.InputSystem.InputKey.Q)) 
            {
                DrinkBeer();
            }
        }

        private void DrinkBeer()
        {
            // Check for artisan beer in inventory
            var itemRoster = MobileParty.MainParty.ItemRoster;
            var artisanBeerObject = MBObjectManager.Instance.GetObject<ItemObject>("artisan_beer");
            var dm = new DisplayMessages();
            var ma = Mission.MainAgent;

            if (itemRoster.GetItemNumber(artisanBeerObject) <= 0) {
                dm.ShowMessage(dm.CreateBasicInfoMessage("No Artisan Beer in Inventory"));
                return;
            }
            
            // Check for player health at limit
            if (ma.Health >= ma.HealthLimit)
            {
                dm.ShowMessage(dm.CreateBasicInfoMessage("Health is full; cannot use Artisan Beer"));
                return;
            }

            // Remove one artisan beer
            itemRoster.AddToCounts(artisanBeerObject, -1);

            // Check that player hp is not at the limit

            // Increase player hp
            var healthAdded = ma.HealthLimit - ma.Health;
            if (healthAdded >= 20)
            {
                ma.Health += 20;
                healthAdded = 20;
            }
            else
            {
                ma.Health += healthAdded;
            }

            // Display Health Message
            var mesArgs = new object[] { healthAdded, ma.Health };
            dm.ShowMessage(dm.CreateFormattedInfoMessage("Gained {0} Health. Health at {1}", mesArgs));
        }
    }
}