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
    public class Player : BaseGameObject
    {
        Texture2D dinoRun1;
        Texture2D dinoRun2;
        Texture2D dinoDuck1;
        Texture2D dinoDuck2;
        Texture2D dinoJump;
        Texture2D dinoDead;
        SpriteFont font;
        bool duck;
        float ground = 750;
        int runCount = 0;
        private float velY = 0.0f;
        private float gravity = 0.7f;
        private int lifespan = 0;
        private int score;
        bool gameOver = false;

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
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(currentTexture, new Vector2(posX - (currentTexture.Width / 2), posY - currentTexture.Height));
            spriteBatch.DrawString(font, $"Score: {score}", new Vector2(50, 100), Color.Black);

            if (DebugClass.displayObjectsInfo)
            {
                spriteBatch.DrawString(font, $"Player positions", new Vector2(250, 70), Color.Black);
                spriteBatch.DrawString(font, $"Bottom: {getTextureRectangle().Bottom} ", new Vector2(250, 100), Color.Black);
                spriteBatch.DrawString(font, $"Top: {getTextureRectangle().Top} ", new Vector2(250, 130), Color.Black);
                spriteBatch.DrawString(font, $"Left: {getTextureRectangle().Left} ", new Vector2(250, 160), Color.Black);
                spriteBatch.DrawString(font, $"Right: {getTextureRectangle().Right} ", new Vector2(250, 190), Color.Black);
                spriteBatch.DrawString(font, $"Width: {getTextureRectangle().Width} ", new Vector2(250, 220), Color.Black);
                spriteBatch.DrawString(font, $"Height: {getTextureRectangle().Height} ", new Vector2(250, 250), Color.Black);
            }
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, ref bool gameOver)
        {
            //gravity stuff
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

            //chaning current texture which will be displayed and handle inputs
            if (!gameOver)
            {
                if (duck && posY == ground)
                {
                    if (runCount < 5)
                    {
                        currentTexture = dinoDuck1;
                    }
                    else
                    {
                        currentTexture = dinoDuck2;
                    }
                }
                else if (posY == ground)
                {
                    if (runCount < 5)
                    {
                        currentTexture = dinoRun1;
                    }
                    else
                    {

                        currentTexture = dinoRun2;
                    }
                }
                else
                {
                    currentTexture = dinoJump;
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
                if (keyboardState.IsKeyDown(Keys.S)) // for test purpose lets implement "pause-like" mechanism, reference works too slow for now
                {
                    gameOver = true;
                }

                IncrementPoints();
            }
            else
            {
                //currentTexture = dinoDead;
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    gameOver = false;
                }
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
    }
}
