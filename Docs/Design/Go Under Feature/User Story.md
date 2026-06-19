# User Story

When it is the player's turn to decide if they want the suit of the displayed kitty card as trump, and they have 3 or more cards ranked less than a Jack in their hand, they can choose to "Go Under".  They can only do this during the kitty round.  If they "Go Under", and they have more than 3 eligible cards, the player has to pick 3 cards to replace.  Using the "Go Under" feature sets the user's bid to "Pass".  The game takes the 3 eligible cards from the player, and gives the 3 hidden cards in exchange to be replaced in the player's hand.  If a previous player had invoked the feature, the game informs the player of the fact.

## Definitions:

### Kitty
The stack of cards remaining when the cards are dealt to the players of the game.

### Kitty Round
The first round of bidding right after the cards have been dealt to the players.

### Card Ranks
How the cards are ranked in descending order:
1. Ace
2. King
3. Queen
4. Jack
5. Ten
6. Nine

## Conditions:

1. Only occurs during the kitty round.
2. It is the player's turn during the kitty round.
3. The player must have 3 or more cards less than the rank of Jack.

## Results:

If the player chooses to "Go Under":

1. The player must select 3 cards to swap if more than 3 cards are below the rank of Jack, otherwise the 3 cards in the player's hand below the rank of Jack are selected.
2. The selected cards are exchanged with the 3 cards in the kitty.
3. The player's bid for the round automatically becomes Pass and the bidding moves onto the next player.

If the player chooses not to "Go Under":

1. The bidding options are presented to the player and they can choose to order up the card or pass as per the normal flow of the process.

