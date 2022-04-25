using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.ObjectSystem;

namespace ArtisanBeer
{
    public class ArtisanBeerMissionView : MissionView 
    {
        GauntletLayer _layer;
        IGauntletMovie _movie;
        ArtisanBeerMissionVM _dataSource;

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();
            _dataSource = new ArtisanBeerMissionVM();
            _layer = new GauntletLayer(1);
            _movie = _layer.LoadMovie("ArtisanBeerHUD", _dataSource);
            MissionScreen.AddLayer(_layer);
        }
        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();
            MissionScreen.RemoveLayer(_layer);
            _movie = null;
            _layer = null;
            _dataSource = null;
        }
        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            if (Input.IsKeyPressed(TaleWorlds.InputSystem.InputKey.Q)) 
            {
                if (Mission.Mode is MissionMode.Battle or MissionMode.Stealth or MissionMode.Duel or MissionMode.Tournament)
                {
                    DrinkBeer();
                }
                else return;
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
                dm.ShowMessage(dm.CreateBasicInfoMessage("Health is full, cannot use Artisan Beer"));
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
            dm.ShowMessage(dm.CreateFormattedInfoMessage("Healed {0} hp. Currently at {1}hp", mesArgs));
        }
    }

    public class ArtisanBeerMissionVM : ViewModel
    {
        int _beerAmount;
        [DataSourceProperty]
        public int BeerAmount
        {
            get
            {
                return this._beerAmount;
            }
            set
            {
                if (value != this._beerAmount)
                {
                    this._beerAmount = value;
                    base.OnPropertyChangedWithValue(value, "BeerAmount");
                }
            }
        }
    }
}