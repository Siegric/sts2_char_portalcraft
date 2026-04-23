using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;

// Super-Evolve: On play, also Summon each selected artifact (in addition to
// activating its effect). Exception to the "no OnPlay override" migration
// rule because the super-evolve effect depends on OnPlay-time selections.
public class RalmiaSonicBoomSuperEvolved : RalmiaSonicBoomEvolved
{
    public RalmiaSonicBoomSuperEvolved() : base(EvoTier.SuperEvolved) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selected = await SelectArtifacts(choiceContext);
        foreach (var card in selected)
        {
            if (card is ArtifactCard artifact)
            {
                await artifact.ActivateEffect(choiceContext);
                await SummonHelper.SummonCopyOf(artifact, Owner);
            }
        }
    }
}
