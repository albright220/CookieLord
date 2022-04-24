using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace ArtisanBeer
{
    public class ArtisanBeerBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnWorkshopChangedEvent.AddNonSerializedListener(this, OnWorkshopChangedEvent);
            CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, DailyTick);
            CampaignEvents.LocationCharactersAreReadyToSpawnEvent
                .AddNonSerializedListener(this,LocationCharactersAreReadyToSpawn);
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }
        ItemObject _artisanBeer;

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            _artisanBeer = MBObjectManager.Instance.GetObject<ItemObject>("artisan_beer");
            AddDialogs(starter);
        }

        private void AddDialogs(CampaignGameStarter starter)
        {
            // Artisan Beer Tavernkeep conversation
            {
                starter.AddPlayerLine("tavernkeeper_talk_ask_artisan_beer",
                "tavernkeeper_talk", "tavernkeeper_artisan_beer", "Do you have any Artisan Beer?", null, null);
                starter.AddDialogLine("tavernkeeper_talk_artisan_beer_a",
                    "tavernkeeper_artisan_beer", "tavernkeeper_talk", "Sorry, I don't sell the good stuff to just anyone. Best head to the brewery if you want to get your hands on it.", () =>
                    {
                        foreach (var workshop in Settlement.CurrentSettlement.Town.Workshops)
                        {
                            if (workshop.WorkshopType.StringId == "brewer") return true;
                        }
                        return false;
                    },
                    null);
                starter.AddDialogLine("tavernkeeper_talk_artisan_beer_b",
                    "tavernkeeper_artisan_beer", "tavernkeeper_talk", "Sorry, you'll have to look elsewhere. There's no brewery here, and without a local supplier I can't get my hands on the good stuff.", null, null);
            }

            // Artisan Beer Brewer conversation
            {
                starter.AddDialogLine("artisan_brewer_talk", "start", "artisan_brewer", "You here for the good stuff?",
                    () => CharacterObject.OneToOneConversationCharacter == Settlement.CurrentSettlement.Culture.CaravanMaster, null);

                // Buy beer conversation tree
                starter.AddPlayerLine("artisan_brewer_buy", "artisan_brewer", "artisan_brewer_purchased", "Of course!", null, () => {
                    // Replace '200' with artisan_beer_price once we implement it as an object
                    GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, 200, false);
                    MobileParty.MainParty.ItemRoster.AddToCounts(_artisanBeer, 1);
                }, 100, (out TextObject explanation) =>
                {
                    if (Hero.MainHero.Gold < 200)
                    {
                        explanation = new TextObject("Not enough Denars.");
                        return false;
                    } else
                    {
                        explanation = TextObject.Empty;
                        return true;
                    }
                });
                starter.AddDialogLine("artisan_brewer_beer_sold", "artisan_brewer_purchased", "end", "Hehehe, I could tell just by lookin' at ya. Here you go, one artisan beer.", null, null);

                // Decline beer conversation tree
                starter.AddPlayerLine("artisan_brewer_refuse", "artisan_brewer", "artisan_brewer_declined", "...what?", null, null);
                starter.AddDialogLine("artisan_brewer_not_sold", "artisan_brewer_declined", "end", "Tch. Beat it kid.", null, null);
            }
        }

        private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
        {
            Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("center");
            if (!(CampaignMission.Current.Location == locationWithId && CampaignTime.Now.IsDayTime)) return;

            Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
            foreach (Workshop workshop in settlement.Town.Workshops)
            {
                if (workshop.IsRunning && workshop.WorkshopType.StringId == "brewery")
                {
                    int num;
                    unusedUsablePointCount.TryGetValue(workshop.Tag, out num);
                    if (num > 0f)
                    {
                        CharacterObject caravanMaster = Settlement.CurrentSettlement.Culture.CaravanMaster;
                        LocationCharacter locationCharacter = new LocationCharacter(
                            new AgentData(new SimpleAgentOrigin(caravanMaster)).Monster(Campaign.Current.HumanMonsterSettlement), 
                            new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), 
                            workshop.Tag, true, LocationCharacter.CharacterRelations.Neutral, null, true, false, null, false, false, true);
                        locationWithId.AddCharacter(locationCharacter);
                    }
                 }
            }
        }

        private void DailyTick(Town town)
        {
            foreach (var workshop in town.Workshops)
            {
                if (workshop.WorkshopType.StringId == "brewery")
                {
                    workshop.ChangeGold(-TaleWorlds.Library.MathF.Round(workshop.Expense * 0.15f));
                }
            }
        }

        private void OnWorkshopChangedEvent(Workshop workshop, Hero hero, WorkshopType type)
        {
            
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }
    }
}