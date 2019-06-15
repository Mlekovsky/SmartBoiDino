using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmartBoyDIno.GameObjects;
using SmartBoyDIno.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoyDIno.AIComponents
{
    public class Population
    {
        public List<Player> pop = new List<Player>();
        public Player bestPlayer;
        public int bestScore = 0;
        public int gen;
        public List<ConnectionHistory> innovationHistory = new List<ConnectionHistory>();
        public List<Player> genPlayer = new List<Player>();
        public List<Species> species = new List<Species>();

        public bool massExtinctionEvent = false;
        public bool newStage = false;
        public int populationLife = 0;

        public Population(int size, DinoTextures dinoTexture)
        {
            for (int i = 0; i < size; i++)
            {
                pop.Add(new Player(dinoTexture));
                pop[i].brain.GenerateNetwork();
                pop[i].brain.Mutate(innovationHistory);
            }
        }

        public void UpdateAlive(GameTime gameTime, List<Cactus> cactuses, List<Bird> berds, float gameSpeed)
        {
            populationLife++;
            foreach (var player in pop)
            {
                if (!player.dead)
                {
                    player.Look(cactuses, berds, gameSpeed);
                    player.Think();
                    player.Update(gameTime, cactuses, berds);
                }
            }
        }

        public void DrawAlive(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var player in pop)
            {
                if (!player.dead)
                    player.Draw(spriteBatch, gameTime);
            }
        }
        /// <summary>
        /// If all players are dead
        /// </summary>
        /// <returns></returns>
        public bool Done()
        {
            foreach (var player in pop)
            {
                if (!player.dead)
                {
                    return false;
                }
            }
            return true;
        }

        public void SetBestPlayer()
        {
            Player tempBest = species[0].players[0];
            tempBest.gen = gen;

            if (tempBest.score > bestScore)
            {
                Console.WriteLine($"Previous best score: {bestScore} ");
                Console.WriteLine($"Current best score: {tempBest.bestScore} ");
                bestScore = tempBest.score;
            }
        }

        public void NaturalSelection()
        {
            Speciate(); // separate population into species
            CalculateFitness(); // calculating fitness of each players
            SortSpecies(); // best players first

            if (massExtinctionEvent)
            {
                MassExtinction();
                massExtinctionEvent = false;
            }

            CullSpecies();
            SetBestPlayer();
            KillStaleSpecies();
            KillBadSpeices();

            //TODO: Write some info about it, f.e generations, mutations itp

            double averageSum = GetAverageFitnessSum();
            List<Player> children = new List<Player>();

            for (int j = 0; j < species.Count; j++)
            {
                for (int i = 0; i < species[j].bestFitness; i++) //keep it for future imgui
                {
                    //Console.WriteLine($"Player: {i} fitness: {species[j].players[i].fitness} score: {species[j].players[i].score}");
                }

                children.Add(species[j].champ.Clone());

                int NumberOfChildren = (int)Math.Floor(species[j].averageFitness / averageSum * pop.Count) - 1;
                for (int i = 0; i < NumberOfChildren; i++)
                {
                    children.Add(species[j].MakeBaby(innovationHistory)); //sweet
                }
            }

            while (children.Count < pop.Count)
            {
                children.Add(species[0].MakeBaby(innovationHistory)); //make the best babies ever!
            }
            pop.Clear();
            pop = new List<Player>(children);
            gen += 1;

            foreach (var player in pop)
            {
                player.brain.GenerateNetwork();
            }

            populationLife = 0;
        }

        private void Speciate()
        {
            foreach (var specie in species)
            {
                specie.players.Clear();
            }

            foreach (var player in pop)
            {
                bool speciesFound = false;

                foreach (var specie in species)
                {
                    if (specie.SameSpecies(player.brain))
                    {
                        specie.AddToSpecies(player);
                        speciesFound = true;
                        break;
                    }
                }

                if (!speciesFound)
                {
                    species.Add(new Species(player));
                }
            }
        }

        private void CalculateFitness()
        {
            foreach (var player in pop)
            {
                player.CalculateFitness();
            }
        }

        private void SortSpecies()
        {
            foreach (var specie in species)
            {
                specie.SortSpecies();
            }

            List<Species> temp = new List<Species>();
            for (int i = 0; i < species.Count; i++)
            {
                double max = 0;
                int maxIndex = 0;
                for (int j = 0; j < species.Count; j++)
                {
                    if (species[j].bestFitness > max)
                    {
                        max = species[j].bestFitness;
                        maxIndex = j;
                    }
                }

                temp.Add(species[maxIndex]);
                species.RemoveAt(maxIndex);
                i--;
            }
            species = new List<Species>(temp);
        }
        /// <summary>
        /// Killing all species which haven't improved in 15 generations
        /// </summary>
        private void KillStaleSpecies()
        {
            for (int i = 2; i < species.Count; i++)
            {
                if (species[i].staleness >= 15)
                {
                    species.RemoveAt(i);
                    i--;
                }
            }
        }
        /// <summary>
        /// If species is so bad, that it wont have 1 child in next gen, then kill it now, before it lay eggs
        /// </summary>
        private void KillBadSpeices()
        {
            double averageSum = GetAverageFitnessSum();

            for (int i = 1; i < species.Count; i++)
            {
                if (species[i].averageFitness / averageSum * pop.Count < 1)
                {
                    species.RemoveAt(i); //bye bye
                    i--;
                }
            }
        }

        private double GetAverageFitnessSum()
        {
            double averageSum = 0;
            foreach (var specie in species)
            {
                averageSum += specie.averageFitness;
            }
            return averageSum;
        }

        private void CullSpecies()
        {
            foreach (var specie in species)
            {
                specie.Cull();
                specie.fitnessSharing();
                specie.SetAverage();
            }
        }
        /// <summary>
        /// Only 5 can be left alive
        /// </summary>
        public void MassExtinction()
        {
            for (int i = 5; i < species.Count; i++)
            {
                species.RemoveAt(i);
                i--;
            }
        }
    }
}
