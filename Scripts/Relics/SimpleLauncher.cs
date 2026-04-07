using BaseLib.Utils;
using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Entities.Relics;


namespace marisamod.Scripts.Relics;

[Pool(typeof(MarisaRelicPool))]
public class SimpleLauncher : AbstractMarisaRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
}