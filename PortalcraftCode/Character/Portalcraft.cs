using BaseLib.Abstracts;
using sts2_char_portalcraft.PortalcraftCode.Cards;
using sts2_char_portalcraft.PortalcraftCode.Extensions;
using MegaCrit.Sts2.Core.Models.Cards;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using sts2_char_portalcraft.PortalcraftCode.Relics;

namespace sts2_char_portalcraft.PortalcraftCode.Character;

public class Portalcraft : PlaceholderCharacterModel
{
    public const string CharacterId = "sts2_char_portalcraft";

    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikePortalcraft>(),
        ModelDb.Card<StrikePortalcraft>(),
        ModelDb.Card<StrikePortalcraft>(),
        ModelDb.Card<StrikePortalcraft>(),
        ModelDb.Card<DefendPortalcraft>(),
        ModelDb.Card<DefendPortalcraft>(),
        ModelDb.Card<DefendPortalcraft>(),
        ModelDb.Card<DefendPortalcraft>(),
        ModelDb.Card<ArtifactRecharge>(),
        ModelDb.Card<PuppetCat>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ResonanceCore>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<PortalcraftCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<PortalcraftRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<PortalcraftPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override string CustomVisualPath => "res://sts2_char_portalcraft/scenes/portalcraft.tscn";
    public override string CustomCharacterSelectBg => "res://sts2_char_portalcraft/scenes/char_select_bg.tscn";
    //public override string? CustomRestSiteAnimPath => "res://sts2_char_portalcraft/scenes/portalcraft.tscn";
    public override string CustomMerchantAnimPath => "res://sts2_char_portalcraft/scenes/portalcraft_merchant.tscn";
    public override string CustomIconPath => "res://sts2_char_portalcraft/scenes/portalcraft_icon.tscn";
    public override string CustomIconTexturePath => "character_icon_portalcraft.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_portalcraft.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_portalcraft_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_portalcraft.png".CharacterUiPath();
}