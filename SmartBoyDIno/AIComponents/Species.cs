using SmartBoyDIno.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoyDIno.AIComponents
{
    public class Species
    {
        public List<Player> players = new List<Player>();
        public double bestFitness = 0;
        public Player champ;
        public double averageFitness = 0;
        public int staleness = 0; //how many generations the species has gone without an improvement
        Genome rep;

        double excessCoeff = 1;
        double weightDiffCoeff = 0.5;
        double compatibilityThreshhold = 3;

        public Species()
        {

        }

        public Species(Player player)
        {
            players.Add(player);
            bestFitness = player.fitness;
            rep = player.brain.Clone();
            champ = player.CloneForReplay();
        }

        public void AddToSpecies(Player player)
        {
            players.Add(player);
        }

        public bool SameSpecies(Genome gen)
        {
            double compatibility;
            double excessAndDisjoint = GetExcessDisjoint(gen, rep);
            double averageWeightiff = AverageWeightDiff(gen, rep);

            double largeGenomeNormaliser = gen.genes.Count - 20;
            if (largeGenomeNormaliser < 1)
            {
                largeGenomeNormaliser = 1;
            }

            compatibility = (excessCoeff * excessAndDisjoint / largeGenomeNormaliser) + (weightDiffCoeff * averageWeightiff);

            return compatibilityThreshhold > compatibility;
        }

        private double AverageWeightDiff(Genome brain1, Genome brain2)
        {
            if (brain1.genes.Count == 0 || brain2.genes.Count == 0)
            {
                return 0;
            }

            double matching = 0;
            double totalDiff = 0;

            for (int i = 0; i < brain1.genes.Count; i++)
            {
                for (int j = 0; j < brain2.genes.Count; j++)
                {
                    if (brain1.genes[i].innovationNo == brain2.genes[j].innovationNo)
                    {
                        matching++;
                        totalDiff += Math.Abs(brain1.genes[i].weight - brain2.genes[j].weight);
                        break;
                    }
                }
            }

            if (matching == 0)
            {
                return 100;
            }

            return totalDiff / matching;
        }

        private double GetExcessDisjoint(Genome brain1, Genome brain2)
        {
            double matching = 0;
            for (int i = 0; i < brain1.genes.Count; i++)
            {
                for (int j = 0; j < brain2.genes.Count; j++)
                {
                    if (brain1.genes[i].innovationNo == brain2.genes[j].innovationNo)
                    {
                        matching++;
                        break;
                    }
                }
            }

            return brain1.genes.Count + brain2.genes.Count - (2 * matching);
        }

        public void SortSpecies()
        {
            List<Player> temp = new List<Player>();

            for (int i = 0; i < players.Count; i++)
            {
                double max = 0;
                int maxIndex = 0;
                for (int j = 0; j < players.Count; j++)
                {
                    if (players[j].fitness > max)
                    {
                        max = players[j].fitness;
                        maxIndex = j;
                    }
                }

                temp.Add(players[maxIndex]);
                players.RemoveAt(maxIndex);
                i--;
            }

            players = new List<Player>(temp);
            if (players.Count == 0)
            {
                Console.WriteLine("sad");
                staleness = 200;
                return;
            }
            //if we have new best player
            if (players[0].fitness > bestFitness)
            {
                staleness = 0;
                bestFitness = players[0].fitness;
                rep = players[0].brain.Clone();
                champ = players[0].CloneForReplay();
            }
            else
            {
                staleness++;
            }
        }

        public void SetAverage()
        {
            double sum = 0;
            foreach (var player in players)
            {
                sum += player.fitness;
            }
            averageFitness = sum / players.Count;
        }

        public Player SelectPlayer()
        {
            double fitnessSum = 0;
            foreach (var player in players)
            {
                fitnessSum += player.fitness;
            }

            Random random = new Random();
            double rand = random.NextDouble(0, fitnessSum);
            double runningSum = 0;

            foreach (var player in players)
            {
                runningSum += player.fitness;
                if (runningSum > rand)
                {
                    return player;
                }
            }

            return players[0]; //just for error not to show
        }

        public Player MakeBaby(List<ConnectionHistory> innovationHistory)
        {
            Player baby;
            Random random = new Random();

            if (random.NextDouble() < 0.25) //no crossover for 25% time, clone of random player
            {
                baby = SelectPlayer().Clone();
            }
            else //otherwise, at 75% of time, make crossover
            {
                Player parent1 = SelectPlayer();
                Player parent2 = SelectPlayer();

                if (parent1.fitness < parent2.fitness) //we pass highest fitness as object, and lowest as parameter
                {
                    baby = parent2.Crossover(parent1);
                }
                else
                {
                    baby = parent1.Crossover(parent2);
                }
            }

            baby.brain.Mutate(innovationHistory);
            return baby;
        }
        /// <summary>
        /// Killing worse half of species
        /// </summary>
        public void Cull()
        {
            if (players.Count > 2)
            { 
                for (int i = players.Count / 2; i < players.Count; i++)
                {
                    players.RemoveAt(i);
                    i--;
                }
            }
        }

        public void fitnessSharing()
        {
            foreach (var player in players)
            {
                player.fitness /= players.Count;
            }
        }
    }
}
