using BaseLib.Abstracts;
using Godot;
using marisamod.Scripts.Cards;
using marisamod.Scripts.Enchantments;
using marisamod.Scripts.PatchesNModels;
using marisamod.Scripts.Relics;
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

    // public override string CustomMerchantAnimPath => "res://scenes/merchant/characters/ironclad_merchant.tscn";
    public override string CustomMerchantAnimPath => "res://marisamod/scenes/marisa_merchant.tscn";

    // public override string CustomArmPointingTexturePath => null;
    // public override string CustomArmRockTexturePath => null;
    // public override string CustomArmPaperTexturePath => null;
    // public override string CustomArmScissorsTexturePath => null;

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

    public static StarlitEnchantment StarlitEnchantment { get; } = new();
}