using BaseLib.Utils;
using marisamod.Scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.scripts.Cards.Colorless
{
    [Pool(typeof(TokenCardPool))]
    public class WhiteDwarf : AbstractMarisaCard
    {
        public WhiteDwarf() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords => [
            CardKeyword.Exhaust
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Burn>()];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CalculationBaseVar(0),
            new ExtraDamageVar(2m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => PileType.Discard.GetPile(card.Owner).Cards.Count),
            new CardsVar(4)
            ];


        protected override bool IsPlayable => PileType.Hand.GetPile(Owner).Cards.Count <= DynamicVars.Cards.BaseValue;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            await CardPileCmd.AddGeneratedCardsToCombat(
                [CombatState!.CreateCard<Burn>(Owner), CombatState!.CreateCard<Burn>(Owner)], PileType.Hand, addedByPlayer: true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.ExtraDamage.UpgradeValueBy(1m);
        }
    }
}