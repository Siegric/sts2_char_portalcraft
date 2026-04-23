using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Extensions;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public class ImariDewdrop : PortalcraftCard, IEvolvableCard
{
    protected readonly EvoTier Tier;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<ImarisLittleBuddies>(),
        HoverTipFactory.FromPower<ImariDewdropPower>(),
        HoverTipFactory.FromKeyword(EvolutionKeyword.Evolution),
        HoverTipFactory.FromKeyword(EvolveKeyword.Evolve),
        HoverTipFactory.FromKeyword(SuperEvolutionKeyword.SuperEvolution),
        HoverTipFactory.FromKeyword(SuperEvolveKeyword.SuperEvolve),
    };

    public ImariDewdrop() : this(EvoTier.Base) { }

    protected ImariDewdrop(EvoTier tier)
        : base(1, CardType.Skill, tier.OverrideRarity(CardRarity.Rare), TargetType.Self,
               showInCardLibrary: tier == EvoTier.Base)
    {
        Tier = tier;
    }

    public virtual Type? EvolvedType      => Tier == EvoTier.Base ? typeof(ImariDewdropEvolved)      : null;
    public virtual Type? SuperEvolvedType => Tier == EvoTier.Base ? typeof(ImariDewdropSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => Tier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Discard a selected card, then draw 1 random Skill from the draw pile.
        bool AnyCard(CardModel c) => c != this;
        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(AnyCard).ToList();
        if (handCards.Count > 0)
        {
            var discardPrefs = new CardSelectorPrefs(
                new LocString("card_selection", "IMARI_DISCARD_PROMPT"),
                minCount: 1,
                maxCount: 1);

            var toDiscard = (await CardSelectCmd.FromHand(choiceContext, Owner, discardPrefs, AnyCard, this)).ToList();
            if (toDiscard.Count > 0)
            {
                await CardCmd.Discard(choiceContext, toDiscard[0]);
            }
        }

        await DrawRandomSkills(Owner, 1);
    }

    // Draws up to `count` random Skills from the owner's draw pile into hand.
    // Returns early (partial fulfillment) if the draw pile runs out of Skills.
    protected static async Task DrawRandomSkills(Player owner, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var skills = PileType.Draw.GetPile(owner).Cards.Where(c => c.Type == CardType.Skill).ToList();
            if (skills.Count == 0) return;
            var picked = owner.RunState.Rng.Shuffle.NextItem(skills);
            await CardPileCmd.Add(picked, PileType.Hand);
        }
    }
}
