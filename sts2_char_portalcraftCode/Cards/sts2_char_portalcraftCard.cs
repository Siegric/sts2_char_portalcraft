using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public abstract class sts2_char_portalcraftCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // Prevent Token and Basic cards from appearing in combat generation (potions, random card effects)
    public override bool CanBeGeneratedInCombat => Rarity != CardRarity.Token && Rarity != CardRarity.Basic;

    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}