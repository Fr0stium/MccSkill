using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MccSkill
{
    public static class BradleyTerry
    {
        // Number of MCCs.
        public const int MccCount = 31;

        // List of players that have played in an MCC.
        public static readonly List<Player> PlayerList = new();

        // A grid of the wins and losses of each player.
        // ReSharper disable once InconsistentNaming
        private static double[,] Scores;

        /// <summary>
        ///     Reads the text file and stores the information in PlayerList.
        /// </summary>
        private static void InitializePlayerList()
        {
            string[] lines = File.ReadAllLines("mccResults.txt");
            foreach (string line in lines)
            {
                string[] info = line.Split(',');
                string username = info[0];
                List<int> coinHistory = new();

                bool hasPlayed = false;

                for (int i = 1; i < MccCount + 1; i++)
                {
                    int coins = int.Parse(info[i]);
                    coinHistory.Add(coins);

                    if (coins > 0) hasPlayed = true;
                }

                if (!hasPlayed) continue;

                Player player = new(username, coinHistory);
                PlayerList.Add(player);
            }
        }

        /// <summary>
        ///     Fills the Scores grid with wins and losses of each player.
        ///     Location i,j is the amount of times player i won against player j.
        /// </summary>
        private static void InitializeScores()
        {
            InitializePlayerList();
            Scores = new double[PlayerList.Count, PlayerList.Count];

            for (int i = 0; i < PlayerList.Count; i++)
            {
                var player = PlayerList[i];
                for (int j = 0; j < PlayerList.Count; j++)
                {
                    var opponent = PlayerList[j];
                    (double wins, double losses) = player.ScoreAgainst(opponent);
                    Scores[i, j] = wins;
                    Scores[j, i] = losses;
                }
            }
        }

        /// <summary>
        ///     Uses the Bradley-Terry model to generate skill ratings for each player.
        ///     An iterative method is used to continuously improve the likelihood function.
        ///     A player's rating can be interpreted as the chance of beating every other player
        ///     if every player played in one giant MCC. Additionally, to find the probability
        ///     that player i beats player j, simply divide player i's rating by the sum of
        ///     player i and player j's rating: i / (i + j).
        /// </summary>
        public static void GenerateSkillLevels()
        {
            InitializeScores();

            double defaultSkillLevel = 1.0 / PlayerList.Count;
            double[] skillLevels = new double[PlayerList.Count];

            // Give each player an initial skill level.

            for (int i = 0; i < skillLevels.Length; i++) skillLevels[i] = defaultSkillLevel;

            // 1000 iterations is pretty good for around 9 decimal places accuracy.

            const int iterations = 1000;

            for (int z = 0; z < iterations; z++)
            {
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    double totalWins = 0;
                    double sum = 0;
                    for (int j = 0; j < PlayerList.Count; j++)
                    {
                        if (j == i) continue;

                        double wins = Scores[i, j];
                        double losses = Scores[j, i];
                        double iSkill = skillLevels[i];
                        double jSkill = skillLevels[j];

                        totalWins += wins;
                        sum += (wins + losses) / (iSkill + jSkill);
                    }

                    skillLevels[i] = double.IsNaN(totalWins / sum) ? 0 : totalWins / sum;
                }

                double skillSum = skillLevels.Sum();
                for (int i = 0; i < skillLevels.Length; i++) skillLevels[i] /= skillSum;
            }
            
            for (int i = 0; i < skillLevels.Length; i++)
                PlayerList[i].Skill = skillLevels[i];
        }
    }
}