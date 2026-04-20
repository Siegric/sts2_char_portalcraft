namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

public interface IFuseCard
{
    int FuseCost { get; }

    bool HasValidFusionPartnerInHand();
}
