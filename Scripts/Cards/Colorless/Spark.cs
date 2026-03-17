using BaseLib.Abstracts;
using BaseLib.Utils;
using marisamod.Scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards.Colorless
{
    [Pool(typeof(ColorlessCardPool))]
    public class Spark : AbstractMarisaCard
    {
        public Spark() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
        {
        }

        //public override string PortraitPath => $"res://MarisaMod/img/cards/Spark_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(4m, ValueProp.Move)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }

        public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, CombatState combatState)
        {
            if (count == 0)
            {
                return [];
            }

            if (CombatManager.Instance.IsOverOrEnding)
            {
                return [];
            }

            List<CardModel> sparks = [];
            for (int i = 0; i < count; i++)
            {
                sparks.Add(combatState.CreateCard<Spark>(owner));
            }

            await CardPileCmd.AddGeneratedCardsToCombat(sparks, PileType.Hand, addedByPlayer: true);
            return sparks;
        }
    }
}