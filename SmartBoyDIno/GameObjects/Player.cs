using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmartBoyDIno.AIComponents;
using SmartBoyDIno.Helpers;
using SmartBoyDIno.Structs;

namespace SmartBoyDIno.GameObjects
{
    public class Player : BaseGameObject
    {
        //switched to use structs but too lazy to remove :( 
        Texture2D dinoRun1;
        Texture2D dinoRun2;
        Texture2D dinoDuck1;
        Texture2D dinoDuck2;
        Texture2D dinoJump;
        Texture2D dinoDead;
        SpriteFont font;

        DinoTextures dinoTextures;
        InfoObjectsPositions InfoObjectsPositions;

        bool duck;
        float ground = 750;
        public int runCount = 0;
        private float velY = 0.0f;
        private float gravity = 0.7f;
        public int lifespan = 0;
        public int score;
        public int bestScore = 0;
        public bool dead;

        //AI
        public double fitness;
        public double unadjustedFitness;
        public Genome brain;
        public int gen = 0;

        public int genomeInputs = 7;
        public int genomeOutputs = 3;

        // make them public so i can acces it from Population, to write some info without displaying it for 500x times each frame
        public double[] vision = new double[7]; // as genome inputs
        public double[] decision = new double[3]; //as genome outputs (for nn)



        public Player(Texture2D dino1, Texture2D dino2, Texture2D dinoDuck1, Texture2D dinoDuck2, Texture2D dinoJump, Texture2D dinoDead, SpriteFont font)
        {
            this.dinoRun1 = dino1;
            this.dinoRun2 = dino2;
            this.dinoDuck1 = dinoDuck1;
            this.dinoDuck2 = dinoDuck2;
            this.dinoJump = dinoJump;
            this.dinoDead = dinoDead;
            this.font = font;
            posX = 150;
            posY = 750;

            brain = new Genome(genomeInputs, genomeOutputs);
        }
        public Player(in DinoTextures dinoTextures, in InfoObjectsPositions infoObjectsPositions)
        {
            this.InfoObjectsPositions = infoObjectsPositions;
            this.dinoTextures = dinoTextures;
            brain = new Genome(genomeInputs, genomeOutputs);
            posX = 150;
            posY = 750;
        }

        public Player()
        {
            brain = new Genome(genomeInputs, genomeOutputs);
            posX = 150;
            posY = 750;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (currentTexture != null)
                spriteBatch.Draw(currentTexture, new Vector2(posX - (currentTexture.Width / 2), posY - currentTexture.Height));

            spriteBatch.DrawString(dinoTextures.font, $"Score: {score}", InfoObjectsPositions.Column1.Row2, Color.Black);

            if (DebugClass.displayObjectsInfo)
            {
                spriteBatch.DrawString(dinoTextures.font, $"Player positions", InfoObjectsPositions.Column2.Row1, Color.Black);
                spriteBatch.DrawString(dinoTextures.font, $"Bottom: {getTextureRectangle().Bottom} ", InfoObjectsPositions.Column2.Row2, Color.Black);
                spriteBatch.DrawString(dinoTextures.font, $"Top: {getTextureRectangle().Top} ", InfoObjectsPositions.Column2.Row3, Color.Black);
                spriteBatch.DrawString(dinoTextures.font, $"Left: {getTextureRectangle().Left} ", InfoObjectsPositions.Column2.Row4, Color.Black);
                spriteBatch.DrawString(dinoTextures.font, $"Right: {getTextureRectangle().Right} ", InfoObjectsPositions.Column2.Row5, Color.Black);
                spriteBatch.DrawString(dinoTextures.font, $"Width: {getTextureRectangle().Width} ", InfoObjectsPositions.Column2.Row6, Color.Black);
                spriteBatch.DrawString(dinoTextures.font, $"Height: {getTextureRectangle().Height} ", InfoObjectsPositions.Column2.Row7, Color.Black);

                DrawRectangle(spriteBatch);
            }
        }

        /// <summary>
        /// Update Logic when player controls dino
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="keyboardState"></param>
        /// <param name="gameOver"></param>
        public void Update(GameTime gameTime, KeyboardState keyboardState, ref bool gameOver)
        {
            //gravity stuff
            HandleGravity();

            if (!gameOver)
            {
                if (duck && posY == ground)
                {
                    if (runCount < 5)
                    {
                        currentTexture = dinoTextures.dinoDuck1;
                    }
                    else
                    {
                        currentTexture = dinoTextures.dinoDuck2;
                    }
                }
                else if (posY == ground)
                {
                    if (runCount < 5)
                    {
                        currentTexture = dinoTextures.dinoRun1;
                    }
                    else
                    {

                        currentTexture = dinoTextures.dinoRun2;
                    }
                }
                else
                {
                    currentTexture = dinoTextures.dinoJump;
                }

                runCount++;
                if (runCount > 10)
                {
                    runCount -= 10;
                }

                //Control for human to play
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    Jump(true);
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    duck = true;
                    Ducking(duck);
                }
                if (!keyboardState.IsKeyDown(Keys.Down))
                {
                    duck = false;
                }

                IncrementPoints();
            }
            else
            {
                //currentTexture = dinoDead; //waiting for new texture :( 
            }

        }
        /// <summary>
        /// Update logic when AI works
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="cactuses"></param>
        /// <param name="berds"></param>
        public void Update(GameTime gameTime, List<Cactus> cactuses, List<Bird> berds)
        {
            HandleGravity();
            if (!dead)
            {
                if (duck && posY == ground)
                {
                    if (runCount < 5)
                    {
                        currentTexture = dinoTextures.dinoDuck1;
                    }
                    else
                    {
                        currentTexture = dinoTextures.dinoDuck2;
                    }
                }
                else if (posY == ground)
                {
                    if (runCount < 5)
                    {
                        currentTexture = dinoTextures.dinoRun1;
                    }
                    else
                    {

                        currentTexture = dinoTextures.dinoRun2;
                    }
                }
                else
                {
                    currentTexture = dinoTextures.dinoJump;
                }

                runCount++;
                if (runCount > 10)
                {
                    runCount -= 10;
                }

                dead = CheckCollision(cactuses, berds);
                IncrementPoints();
            }
        }

        private bool CheckCollision(List<Cactus> cactuses, List<Bird> berds)
        {
            return (from berd in berds where berd.PlayerTouched(this) select berd).Any()
               || (from cacti in cactuses where cacti.PlayerTouched(this) select cacti).Any();
        }

        private void HandleGravity()
        {
            posY -= velY;
            if (posY < ground)
            {
                velY -= gravity;
            }
            else
            {
                velY = 0;
                posY = ground;
            }
        }
        void Jump(bool bigJump)
        {
            if (posY == ground)
            {
                if (bigJump)
                {
                    gravity = 1;
                    velY = 20;
                }
                else
                {
                    gravity = 1.2f;
                    velY = 16;
                }
            }
        }
        void Ducking(bool isDucking)
        {
            if (posY != ground && isDucking)
            {
                gravity = 3;
            }
            duck = isDucking;
        }
        void IncrementPoints()
        {
            lifespan++;
            if (lifespan % 3 == 0)
            {
                score += 1;
            }
        }

        //AI PART
        /// <summary>
        /// getting informations to pass into NN
        /// </summary>
        /// Vision definitions:
        /// 0 - distance to enemy
        /// 1 - height of enemy
        /// 2 - width of enemy
        /// 3 - height above the ground of the enemy
        /// 4 - current game speed
        /// 5 - player Y position (above the ground)
        /// 6 - distance between 2 enemies on screen (if there are 2 of them at once)
        public void Look(List<Cactus> cactuses, List<Bird> berds, float gameSpeed)
        {
            //ATM i do not implement replay system
            float temp = 0;
            float min = 10000;
            int minIndex = -1;
            bool berd = false;

            for (int i = 0; i < cactuses.Count; i++)
            {
                if (cactuses[i].posX + cactuses[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2) < min && cactuses[i].posX + cactuses[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2) > 0)
                {
                    min = cactuses[i].posX + cactuses[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2);
                    minIndex = i;
                }
            }

            for (int i = 0; i < berds.Count; i++)
            {
                if (berds[i].posX + berds[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2) < min && berds[i].posX + berds[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2) > 0)
                {
                    min = berds[i].posX + berds[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2);
                    minIndex = i;
                    berd = true;
                }
            }

            vision[4] = gameSpeed;
            vision[5] = this.posY;

            if (minIndex == -1) // no obstacles
            {
                vision[0] = 0;
                vision[1] = 0;
                vision[2] = 0;
                vision[3] = 0;
                vision[6] = 0;
            }
            else // get more info about enemies neraby
            {
                vision[0] = min;
                if (berd)
                {
                    vision[1] = berds[minIndex].h;
                    vision[2] = berds[minIndex].w;
                    if (berds[minIndex].type == 1)
                    {
                        vision[3] = 0; // ground bird
                    }
                    else
                    {
                        vision[3] = ground - berds[minIndex].posY;
                    }
                }
                else
                {
                    vision[1] = cactuses[minIndex].h;
                    vision[2] = cactuses[minIndex].w;
                    vision[3] = 0;
                }
            }

            int bestIndex = minIndex;
            float closesDist = min;
            min = 10000;
            minIndex = -1;

            for (int i = 0; i < cactuses.Count; i++)
            {
                if ((berd || i != bestIndex) && cactuses[i].posX + cactuses[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2) < min && cactuses[i].posX + cactuses[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2) > 0)
                {
                    min = cactuses[i].posX + cactuses[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2);
                    minIndex = i;
                }
            }

            for (int i = 0; i < berds.Count; i++)
            {
                if ((!berd || i != bestIndex) && berds[i].posX + berds[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2) < min && berds[i].posX + berds[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2) > 0)
                {
                    min = berds[i].posX + berds[i].w / 2 - (this.posX - dinoTextures.dinoRun1.Width / 2);
                    minIndex = i;
                }
            }

            if (minIndex == -1)
            {
                vision[6] = 0; // there are only one objects on screen
            }
            else
            {
                vision[6] = min - closesDist;
            }
        }

        public void Think()
        {
            double max = 0;
            double maxIndex = 0;

            //getting output of NN
            decision = brain.feedForward(vision);

            for (int i = 0; i < decision.Length; i++)
            {
                if (decision[i] > max)
                {
                    max = decision[i];
                    maxIndex = i;
                }
            }

            if (max < 0.7)
            {
                Ducking(false);
                return;
            }

            switch (maxIndex)
            {
                case 0:
                    Jump(false);
                    break;
                case 1:
                    Jump(true);
                    break;
                case 2:
                    Ducking(true);
                    break;
                default:
                    break;
            }

        }

        public Player Clone()
        {
            Player clone = new Player();
            clone.dinoTextures = this.dinoTextures;
            clone.InfoObjectsPositions = this.InfoObjectsPositions;
            clone.brain = brain.Clone();
            clone.fitness = fitness;
            clone.brain.GenerateNetwork();
            clone.gen = gen;
            clone.bestScore = score;
            return clone;
        }

        public Player CloneForReplay()
        {
            Player clone = new Player();
            clone.brain = this.brain.Clone();
            clone.dinoTextures = this.dinoTextures;
            clone.InfoObjectsPositions = this.InfoObjectsPositions;
            clone.fitness = this.fitness;
            clone.gen = this.gen;
            clone.bestScore = score;

            return clone;
        }

        public void CalculateFitness()
        {
            fitness = score * score;
        }

        public Player Crossover(Player parent2)
        {
            Player child = new Player();
            child.brain = brain.Crossover(parent2.brain);
            child.brain.GenerateNetwork();
            child.dinoTextures = parent2.dinoTextures;
            child.InfoObjectsPositions = this.InfoObjectsPositions;
            return child;
        }
    }
}
