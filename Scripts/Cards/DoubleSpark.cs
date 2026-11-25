using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using marisamod.Scenes.Vfx.HitVfx;
using marisamod.Scenes.Vfx.SparkProjectile;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;
using marisamod.Scripts.Cards.Colorless;
using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace marisamod.Scripts.Cards
{
    public class DoubleSpark : AbstractMarisaCard
    {
        public DoubleSpark() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
        {
        }

        //public override string PortraitPath => $"res://img/cards/DoubleSpark_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(7m, ValueProp.Move)
        ];

        protected override HashSet<CardTag> CanonicalTags => [MarisaCardTags.Spark];

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Spark>(IsUpgraded)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this,cardPlay).Targeting(cardPlay.Target)
                //.WithHitVfxNode((Creature t) => VfxSparkProjectile.Create(this,new(0.4f,0.8f,0.8f,1.0f),NCombatRoom.Instance?.GetCreatureNode(t)!))
                .BeforeDamage(async delegate
                    {
                        var vfx = VfxSparkProjectile.Create(this, new(0.4f, 0.8f, 0.8f, 1.0f),
                            NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target)!);
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
                        await Cmd.Wait(vfx.VfxTime);
                    }
                )
                .Execute(choiceContext);
            if (CombatState != null)
            {
                var cards = await Spark.CreateInHand(Owner, 1, CombatState);
                if (IsUpgraded)
                {
                    var cardModels = cards as CardModel[] ?? cards.ToArray();
                    if (cardModels.Length != 0)
                    {
                        foreach (var card in cardModels)
                        {
                            CardCmd.Upgrade(card);
                        }
                    }
                }
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3m);
        }
    }
}