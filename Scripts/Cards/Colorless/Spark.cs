using BaseLib.Utils;
using Godot;
using marisamod.Scenes.Vfx.SparkProjectile;
using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards.Colorless
{
    [Pool(typeof(TokenCardPool))]
    public class Spark : AbstractMarisaCard
    {
        public Spark() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(4m, ValueProp.Move)
        ];
        protected override HashSet<CardTag> CanonicalTags =>
        [
            MarisaCardTags.Spark
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this,cardPlay).Targeting(cardPlay.Target)
                //.WithHitFx("vfx/vfx_attack_slash")
                //.WithHitVfxNode((Creature t) => VfxSparkProjectile.Create(this,new Vector4(0.4f,0.8f,0.8f,1.0f),NCombatRoom.Instance?.GetCreatureNode(t)!))
                .BeforeDamage(async delegate
                    {
                        var vfx = VfxSparkProjectile.Create(this, new(0.4f, 0.8f, 0.8f, 1.0f),
                            NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target)!);
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
                        await Cmd.Wait(vfx.VfxTime);
                    })
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1m);
            AddKeyword(CardKeyword.Retain);
        }

        public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, ICombatState combatState)
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

            await CardPileCmd.AddGeneratedCardsToCombat(sparks, PileType.Hand, owner);
            return sparks;
        }
    }
}
