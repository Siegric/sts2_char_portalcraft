namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

// Marker interface. Cards implementing this must declare a DynamicVar named "Countdown"
// (see CountdownHelper.CountdownVarName). KeywordDispatcherPower decrements it each
// player turn start and exhausts the card when it hits 0.
public interface ICountdownCard
{
}
