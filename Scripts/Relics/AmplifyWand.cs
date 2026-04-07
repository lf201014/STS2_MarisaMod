using marisamod.Scripts.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Relics
{
    public class AmplifyWand : AbstractMarisaRelic
    {
        public override RelicRarity Rarity => RelicRarity.Rare;

        //private bool _trigger;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(4,ValueProp.Unpowered)
        ];

        public override async Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == Owner && cardPlay.Card is AbstractAmplifiedCard { IsAmplified: true })
            {
                //_trigger = true;
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Unpowered, null);
            }
        }

        // public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        // {
        //     if (_trigger)
        //     {
        //         _trigger = false;
        //     }
        // }
    }
}