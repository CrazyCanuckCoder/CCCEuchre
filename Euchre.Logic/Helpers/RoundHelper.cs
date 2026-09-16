using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Euchre.Logic.Helpers;

/// <summary>
/// A class with methods to help with the round logic of the game.
/// </summary>
public class RoundHelper
{
    public static bool PlayerCanWinRemainingTricks(IPlayer leadingPlayer, IGameStateManager gameStateManager)
    {
        bool canWinRest = true;

        // Determine which players are participating in the round.

        Dictionary<int, List<Card>> playersHands = [];
        foreach (var player in gameStateManager.Players!)
        {
            if (player.PlayerIndex != leadingPlayer.PlayerIndex)
            {
                playersHands[player.PlayerIndex] = player.Hand;
            }
        }

        // Check if a lone hand is being played and remove the lone player's partner from consideration.

        if (gameStateManager.GoingAlone && gameStateManager.AlonePlayer != null)
        {
            var partnerIndex = GetPlayersPartnerIndex(gameStateManager.AlonePlayer, gameStateManager.Players);
            playersHands.Remove(partnerIndex);
        }

        // Check each card in the player's hand to see if it can win against the remaining players' hands.

        return canWinRest;
    }

    /// <summary>
    /// Gets the index of the partner of the player who is going alone.
    /// </summary>
    /// <param name="alonePlayer">The player who is going alone.</param>
    /// <param name="players">The list of all players.</param>
    /// <returns>The index of the partner player.</returns>
    private static int GetPlayersPartnerIndex(IPlayer alonePlayer, IPlayer[] players)
    {
        var partnerIndex = 0;

        foreach (var player in players)
        {
            if (player.TeamIndex == alonePlayer.TeamIndex && player.PlayerIndex != alonePlayer.PlayerIndex)
            {
                partnerIndex = player.PlayerIndex;
                break;
            }
        }

        return partnerIndex;
    }
}
