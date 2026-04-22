namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public interface IFuseCard
{
    int FuseCost { get; }

    bool HasValidFusionPartnerInHand();
}
