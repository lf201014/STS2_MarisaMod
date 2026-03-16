using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MarisaMod.scripts.Cards
{
    public class DefendMarisa : AbstractMarisaModCard
    {
        public DefendMarisa() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
        {
        }

        //public override string PortraitPath => $"res://img/cards/Defend_MRS_p.png";

        public override bool GainsBlock => true;

        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };

        protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> { new BlockVar(5m, ValueProp.Move) };
    }
}