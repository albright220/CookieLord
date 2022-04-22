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
            if (itemRoster.GetItemNumber(artisanBeerObject) <= 0) return;

            // Remove one artisan beer
            itemRoster.AddToCounts(artisanBeerObject, -1);

            // Increase player hp
            Mission.MainAgent.Health += 20;
            InformationManager.DisplayMessage(new InformationMessage(
                String.Format("Health increased by {0}, now at {1}", 20, Mission.MainAgent.Health)
                ));
        }
    }
}