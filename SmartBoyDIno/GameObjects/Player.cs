using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SmartBoyDIno.GameObjects
{
    public class Player
    {
        Texture2D dinoRun1;
        Texture2D dinoRun2;
        Texture2D dinoDuck1;
        Texture2D dinoDuck2;
        Texture2D dinoJump;
        Texture2D dinoDead;
        SpriteFont font;
        bool duck;
        int posX = 150;
        float posY = 750;
        float ground = 750;
        int runCount = 0;
        private float velY = 0.0f;
        private float gravity = 1.2f;
        private int lifespan = 0;
        private int score;

        public Player(Texture2D dino1, Texture2D dino2, Texture2D dinoDuck1, Texture2D dinoDuck2, Texture2D dinoJump, Texture2D dinoDead, SpriteFont font)
        {
            this.dinoRun1 = dino1;
            this.dinoRun2 = dino2;
            this.dinoDuck1 = dinoDuck1;
            this.dinoDuck2 = dinoDuck2;
            this.dinoJump = dinoJump;
            this.dinoDead = dinoDead;
            this.font = font;
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (duck && posY == ground)
            {
                if (runCount < 5)
                {
                    spriteBatch.Draw(dinoDuck1, new Vector2(posX - dinoDuck2.Width / 2, posY - dinoDuck2.Height));
                }
                else
                {
                    spriteBatch.Draw(dinoDuck2, new Vector2(posX - dinoDuck1.Width / 2, posY - dinoDuck1.Height));
                }
            }
            else if (posY == ground)
            {
                if (runCount < 5)
                {
                    spriteBatch.Draw(dinoRun1, new Vector2(posX - dinoRun1.Width / 2, posY - dinoRun1.Height));
                }
                else
                {
                    spriteBatch.Draw(dinoRun2, new Vector2(posX - dinoRun2.Width / 2, posY - dinoRun2.Height));
                }
            }
            else
            {
                spriteBatch.Draw(dinoJump, new Vector2(posX - dinoJump.Width / 2, posY - dinoJump.Height));
            }
            runCount++;
            if (runCount > 10)
            {
                runCount -= 10;
            }

            spriteBatch.DrawString(font, $"Score: {score}", new Vector2(100, 100), Color.Black);
        }
        public void Update(GameTime gameTime, KeyboardState keyboardState)
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

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                Jump(false);
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
    }
}
