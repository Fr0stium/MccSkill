using System;
using System.Linq;

namespace MccSkill
{
    internal static class Program
    {
        private static void Main()
        {
            BradleyTerry.GenerateSkillLevels();
            foreach (var player in BradleyTerry.PlayerList.OrderByDescending(p => p.Skill)) Console.WriteLine(player);
        }
    }
}