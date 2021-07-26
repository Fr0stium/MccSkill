using System.Collections.Generic;

namespace MccSkill
{
    public class Player
    {
        public Player(string username, List<int> coinHistory)
        {
            Username = username;
            CoinHistory = coinHistory;
        }

        private string Username { get; }
        private List<int> CoinHistory { get; }
        public double Skill { get; set; }

        public override string ToString()
        {
            return Username == null ? string.Empty : $"{Username}, {Skill}";
        }

        /// <summary>
        ///     Gets how many coins the player got in an MCC.
        /// </summary>
        private int GetCoinsAt(int mccNumber)
        {
            int i = mccNumber - 1;
            return CoinHistory[i];
        }

        /// <summary>
        ///     Gets the score (wins, losses) against an opponent.
        ///     Ties count as half a win and half a loss.
        /// </summary>
        public (double, double) ScoreAgainst(Player opponent)
        {
            if (this == opponent) return (0, 0);

            if (Username == null || opponent.Username == null) return (1, 1);

            double wins = 0;
            double losses = 0;

            for (int i = 1; i <= BradleyTerry.MccCount; i++)
            {
                int playerCoins = GetCoinsAt(i);
                int opponentCoins = opponent.GetCoinsAt(i);

                if (playerCoins == -1 || opponentCoins == -1) continue;

                if (playerCoins > opponentCoins)
                {
                    wins++;
                }
                else if (playerCoins < opponentCoins)
                {
                    losses++;
                }
                else
                {
                    wins += 0.5;
                    losses += 0.5;
                }
            }

            return (wins, losses);
        }
    }
}