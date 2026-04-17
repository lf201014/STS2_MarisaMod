using BaseLib.Abstracts;
using Godot;
using marisamod.Scripts.Cards;
using marisamod.Scripts.Enchantments;
using marisamod.Scripts.PatchesNModels;
using marisamod.Scripts.Relics;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Characters;

public class MarisaCharacter : PlaceholderCharacterModel
{
    public override Color NameColor => new(0f, 0.1f, 0.7f);

    public override CharacterGender Gender => CharacterGender.Feminine;

    public override int StartingHp => 75;

    public override string CustomVisualPath => "res://marisamod/scenes/test_character.tscn";

    // public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    public override string CustomIconTexturePath => "res://marisamod/images/MarisaButton.png";

    // public override string CustomIconPath => "res://scenes/ui/character_icons/ironclad_icon.tscn";
    public override string CustomIconPath => "res://marisamod/scenes/marisa_icon.tscn";

    public override string CustomEnergyCounterPath => "res://marisamod/scenes/test_energy_counter.tscn";

    // public override string CustomRestSiteAnimPath => "res://scenes/rest_site/characters/ironclad_rest_site.tscn";
    //public override string CustomRestSiteAnimPath => "res://marisamod/scenes/marisa_rest_site.tscn";

    public override string CustomMerchantAnimPath => "res://marisamod/scenes/marisa_merchant.tscn";

    public override string CustomArmPointingTexturePath => "res://marisamod/images/ui/hand_point.png";
    public override string CustomArmRockTexturePath => "res://marisamod/images/ui/hand_rock.png";
    public override string CustomArmPaperTexturePath => "res://marisamod/images/ui/hand_paper.png";
    public override string CustomArmScissorsTexturePath => "res://marisamod/images/ui/hand_scissors.png";

    public override string CustomCharacterSelectBg => "res://marisamod/scenes/test_bg.tscn";
    public override string CustomCharacterSelectIconPath => "res://marisamod/images/char_select_marisa.png";

    public override string CustomCharacterSelectLockedIconPath => "res://marisamod/images/char_select_marisa _locked.png";

    // public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";
    // public override string CustomMapMarkerPath => null;
    // public override string CustomAttackSfx => null;
    // public override string CustomCastSfx => null;
    // public override string CustomDeathSfx => null;
    // public override string CharacterSelectSfx => null;
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override CardPoolModel CardPool => ModelDb.CardPool<MarisaCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MarisaRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MarisaPotionPool>();

    // 初始卡组
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<SparkStrike>(),
        ModelDb.Card<SparkStrike>(),
        ModelDb.Card<SparkStrike>(),
        ModelDb.Card<SparkStrike>(),
        ModelDb.Card<DefendMarisa>(),
        ModelDb.Card<DefendMarisa>(),
        ModelDb.Card<DefendMarisa>(),
        ModelDb.Card<DefendMarisa>(),
        ModelDb.Card<MasterSpark>(),
        ModelDb.Card<UpSweep>(),
    ];

    // 初始遗物
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<MiniHakkero>(),
    ];

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];

    // public static EnchantmentModel? Enchant(EnchantmentModel enchantment, CardModel card, decimal amount = 1)
    // {
    //
    //     enchantment.AssertMutable();
    //     if (!enchantment.CanEnchant(card))
    //     {
    //         throw new InvalidOperationException($"Cannot enchant {card.Id} with {enchantment.Id}.");
    //     }
    //     if (card.Enchantment == null)
    //     {
    //         card.EnchantInternal(enchantment, amount);
    //         enchantment.ModifyCard();
    //     }
    //     else
    //     {
    //         if (!(card.Enchantment.GetType() == enchantment.GetType()))
    //         {
    //             throw new InvalidOperationException($"Cannot enchant {card.Id} with {enchantment.Id} because it already has enchantment {card.Enchantment.Id}.");
    //         }
    //         card.Enchantment.Amount += (int)amount;
    //     }
    //     card.FinalizeUpgradeInternal();
    //
    //     return card.Enchantment;
    // }
    
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var animState = new AnimState("idle_loop", isLooping: true);
        var animState2 = new AnimState("cast");
        var animState3 = new AnimState("attack");
        var animState4 = new AnimState("hurt");
        var state = new AnimState("die");
        var animState5 = new AnimState("spark");
        var animState6 = new AnimState("relaxed_loop", isLooping: true);
        animState2.NextState = animState;
        animState3.NextState = animState;
        animState4.NextState = animState;
        animState5.NextState = animState;
        animState6.AddBranch("Idle", animState);
        var creatureAnimator = new CreatureAnimator(animState, controller);
        creatureAnimator.AddAnyState("Idle", animState);
        creatureAnimator.AddAnyState("Dead", state);
        creatureAnimator.AddAnyState("Hit", animState4);
        creatureAnimator.AddAnyState("Attack", animState3);
        creatureAnimator.AddAnyState("Cast", animState2);
        creatureAnimator.AddAnyState("Spark", animState5);
        creatureAnimator.AddAnyState("Relaxed", animState6);
        return creatureAnimator;
    }
}