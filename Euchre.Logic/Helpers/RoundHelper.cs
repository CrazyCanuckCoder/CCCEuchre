using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.Helpers;

/// <summary>
/// A class with methods to help with the round logic of the game.
/// </summary>
public class RoundHelper
{
    /// <summary>
    /// Determines if the leading player can win all of the remaining tricks in the round based on their hand 
    /// and the hands of the other players.
    /// </summary>
    /// <param name="leadingPlayer">The player that is going to lead a card in the current round.</param>
    /// <param name="gameStateManager">The game state manager.</param>
    /// <returns>True if the leading player can win all remaining tricks; otherwise, false.</returns>
    public static bool PlayerCanWinRemainingTricks(IPlayer leadingPlayer, IGameStateManager gameStateManager)
    {
        bool canWinRest = true;

        // Verify inputs.

        ArgumentNullException.ThrowIfNull(leadingPlayer);
        ArgumentNullException.ThrowIfNull(gameStateManager);

        if (leadingPlayer.Hand.Count == 0)
        {
            throw new ArgumentException("Leading player has no cards in hand.");
        }

        if (gameStateManager.Players == null || gameStateManager.Players.Length < 2)
        {
            throw new ArgumentException("Game state manager has insufficient players.");
        }

        // Determine which players are participating in the round.

        Dictionary<int, List<Card>> playersHands = [];
        foreach (var player in gameStateManager.Players!)
        {
            if (player.PlayerIndex != leadingPlayer.PlayerIndex)
            {
                playersHands[player.PlayerIndex] = player.Hand;
            }
        }

        // Check if a lone hand is being played and remove the lone player's partner from consideration since
        //  they are sitting out the round.

        if (gameStateManager.GoingAlone && gameStateManager.AlonePlayer != null)
        {
            var partnerIndex = GetPlayersPartnerIndex(gameStateManager.AlonePlayer, gameStateManager.Players);
            playersHands.Remove(partnerIndex);
        }

        // Check each card in the player's hand to see if it can win against the remaining players' hands.

        foreach (var card in leadingPlayer.Hand)
        {
            // Is the card a trump card?

            if (card.EffectiveSuit(gameStateManager.Trump!.Value) == gameStateManager.Trump.Value)
            {
                // Check if any of the remaining players have a higher trump card.

                foreach (var playerHand in playersHands.Values)
                {
                    if (CardHelper.CardListHasHigherTrumpThanCard(playerHand, card, 
                        gameStateManager.Trump.Value))
                    {
                        return false;
                    }
                }
            }
            else
            {
                // Since the card is not a trump card, check if any of the other players could beat it - 
                //   either with a higher card of the same suit or with a trump card.

                foreach (var playerHand in playersHands.Values)
                {
                    if (CardHelper.CardListCanDefeatCard(playerHand, card, gameStateManager.Trump.Value))
                    {
                        return false;
                    }
                }
            }
        }

        return canWinRest;
    }

    /// <summary>
    /// Gets the index of the partner of a specified player.
    /// </summary>
    /// <param name="player">The specified player to find use to find their partner.</param>
    /// <param name="players">The list of all players.</param>
    /// <returns>The index of the partner player.</returns>
    private static int GetPlayersPartnerIndex(IPlayer player, IPlayer[] players)
    {
        var partnerIndex = 0;

        foreach (var gamePlayer in players)
        {
            if (gamePlayer.TeamIndex == player.TeamIndex && gamePlayer.PlayerIndex != player.PlayerIndex)
            {
                partnerIndex = gamePlayer.PlayerIndex;
                break;
            }
        }

        return partnerIndex;
    }
}
