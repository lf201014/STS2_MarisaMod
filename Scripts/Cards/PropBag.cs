using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;

namespace marisamod.Scripts.Cards;

public class PropBag : AbstractMarisaCard
{
    public PropBag() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var relic = await RelicCmd.Obtain(RelicFactory.PullNextRelicFromBack(Owner, RelicRarity.Uncommon, x=>x is not Pear), Owner);

        PropBagPower? pow;
        if (Owner.Creature.HasPower<PropBagPower>())
        {
            pow = Owner.Creature.GetPower<PropBagPower>();
        }
        else
        {
            pow = await PowerCmd.Apply<PropBagPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        pow?.AddRelicToList(relic);
    }
}