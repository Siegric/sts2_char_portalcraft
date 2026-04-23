using MegaCrit.Sts2.Core.Entities.Cards;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public enum EvoTier
{
    Base,
    Evolved,
    SuperEvolved,
}

public static class EvoTierExtensions
{
    public static CardRarity OverrideRarity(this EvoTier tier, CardRarity baseRarity) => tier switch
    {
        EvoTier.Evolved      => CardRarity.Rare,
        EvoTier.SuperEvolved => CardRarity.Ancient,
        _                    => baseRarity,
    };
    
    public static string PortraitSubfolder(this EvoTier tier) => tier switch
    {
        EvoTier.Evolved      => "evolved/",
        EvoTier.SuperEvolved => "superevolved/",
        _                    => "",
    };
}
