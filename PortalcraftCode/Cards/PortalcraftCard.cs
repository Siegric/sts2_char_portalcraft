using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public abstract class PortalcraftCard : CustomCardModel
{
    public override bool CanBeGeneratedInCombat => Rarity != CardRarity.Token && Rarity != CardRarity.Basic;

    // Glow gold while the card is in an Evolved / Super Evolved tier, so players
    // see which cards in their hand carry an active evolution stat boost.
    protected override bool ShouldGlowGoldInternal => EvoRuntime.GetTier(this) != null;

    //Image size:
    //Normal art: 1000x760
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190
    
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected PortalcraftCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target, showInCardLibrary: rarity != CardRarity.Token)
    {
    }

    protected PortalcraftCard(int cost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary)
        : base(cost, type, rarity, target, showInCardLibrary: showInCardLibrary)
    {
    }
}
