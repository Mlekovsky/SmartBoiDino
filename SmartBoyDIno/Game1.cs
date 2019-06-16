using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmartBoyDIno.AIComponents;
using SmartBoyDIno.GameObjects;
using SmartBoyDIno.Helpers;
using SmartBoyDIno.Structs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartBoyDIno
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player tempGenPlayer;
        Texture2D dino1;
        Texture2D dino2;
        Texture2D cactusSmall;
        Texture2D cactusBig;
        Texture2D bird1;
        Texture2D bird2;
        Texture2D cactusSmallMany;
        Texture2D dinoJump;
        Texture2D dinoDead;
        Texture2D dinoDuck1;
        Texture2D dinoDuck2;
        SpriteFont scoreFont;
        KeyboardState keyboard;

        List<Cactus> cactuses = new List<Cactus>();
        List<Bird> berds = new List<Bird>();
        Population population;
        int upToGen = 0;

        int obstacleTimer;
        int minimumTimeBetweenObstacles = 60;
        double randomAdd = 0;
        double maxRandomAdd = 50;
        bool gameOver = false;
        bool stopGame = false;

        float ground = 750;
        float gameSpeed = 10;

        int windowWidth = 1600;
        int windowHeight = 900;

        //structs
        DinoTextures dinoTextures;
        InfoObjectsPositions InfoObjectsPositions;

        public static int nextConnectionNo = 1000;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.ApplyChanges();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            #region LoadImages


            spriteBatch = new SpriteBatch(GraphicsDevice);
            dino1 = this.Content.Load<Texture2D>("dinorun0000");
            dino2 = this.Content.Load<Texture2D>("dinorun0001");
            dinoDuck1 = this.Content.Load<Texture2D>("dinoduck0000");
            dinoDuck2 = this.Content.Load<Texture2D>("dinoduck0001");
            dinoDead = this.Content.Load<Texture2D>("dinoDead0000");
            dinoJump = this.Content.Load<Texture2D>("dinoJump0000");
            bird1 = this.Content.Load<Texture2D>("berd");
            bird2 = this.Content.Load<Texture2D>("berd2");
            cactusBig = this.Content.Load<Texture2D>("BigCactus");
            cactusSmall = this.Content.Load<Texture2D>("SmallCactus");
            cactusSmallMany = this.Content.Load<Texture2D>("ManySmallCactus");
            scoreFont = this.Content.Load<SpriteFont>("Score");

            dinoTextures = new DinoTextures(dino1, dino2, dinoDuck1, dinoDuck2, dinoJump, dinoDead, scoreFont);
            #endregion
            InitializeInfoVectors();

            if (DebugClass.AiGameplay)
                population = new Population(500, dinoTextures, InfoObjectsPositions);
            else
                tempGenPlayer = new Player(dinoTextures, InfoObjectsPositions);


            // tempGenPlayer = new Player(dino1, dino2, dinoDuck1, dinoDuck2, dinoJump, dinoDead, scoreFont);
        }

        private void InitializeInfoVectors()
        {
            int column1X = 50;
            int column2X = 250;
            int coulmn3X = 450;
            int coulmn4x = 650;
            InfoObjectsPositions = new InfoObjectsPositions(
                new InfoColumn(
                    new Vector2(column1X, 70),
                    new Vector2(column1X, 100),
                    new Vector2(column1X, 130),
                    new Vector2(column1X, 160),
                    new Vector2(column1X, 190),
                    new Vector2(column1X, 220),
                    new Vector2(column1X, 250),
                    new Vector2(column1X, 280)
                    ),
                new InfoColumn(new Vector2(column2X, 70),
                    new Vector2(column2X, 100),
                    new Vector2(column2X, 130),
                    new Vector2(column2X, 160),
                    new Vector2(column2X, 190),
                    new Vector2(column2X, 220),
                    new Vector2(column2X, 250),
                    new Vector2(column2X, 280)
                    ),
                new InfoColumn(new Vector2(coulmn3X, 70),
                    new Vector2(coulmn3X, 100),
                    new Vector2(coulmn3X, 130),
                    new Vector2(coulmn3X, 160),
                    new Vector2(coulmn3X, 190),
                    new Vector2(coulmn3X, 220),
                    new Vector2(coulmn3X, 250),
                    new Vector2(coulmn3X, 280)
                    ),
                new InfoColumn(new Vector2(coulmn4x, 70),
                    new Vector2(coulmn4x, 100),
                    new Vector2(coulmn4x, 130),
                    new Vector2(coulmn4x, 160),
                    new Vector2(coulmn4x, 190),
                    new Vector2(coulmn4x, 220),
                    new Vector2(coulmn4x, 250),
                    new Vector2(coulmn4x, 280))
                );
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (DebugClass.AiGameplay)
            {
                if (!population.Done())
                {
                    UpdateObstacles();
                    MoveObstacles();
                    population.UpdateAlive(gameTime, cactuses, berds, gameSpeed);
                }
                else
                {
                    population.NaturalSelection();
                    ResetObstacle();
                }
            }
            else
            {
                keyboard = Keyboard.GetState();
                tempGenPlayer.Update(gameTime, keyboard, ref gameOver);
                HandleDebugControls(keyboard);

                if (!gameOver)
                {
                    UpdateObstacles();
                    MoveObstacles();

                    gameOver = HandleCollision();
                }
            }

            base.Update(gameTime);
        }

        private bool HandleCollision()
        {
            return (from berd in berds where berd.PlayerTouched(tempGenPlayer) select berd).Any()
               || (from cacti in cactuses where cacti.PlayerTouched(tempGenPlayer) select cacti).Any();
        }

        private void HandleDebugControls(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.D))
            {
                DebugClass.displayObjectsInfo = !DebugClass.displayObjectsInfo;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                gameOver = !gameOver;
            }
        }

        private void MoveObstacles()
        {
            foreach (var cacti in cactuses)
            {
                cacti.Update(gameSpeed);
            }
            foreach (var berdy in berds)
            {
                berdy.Update(gameSpeed);
            }
        }

        private void UpdateObstacles()
        {
            obstacleTimer++;
            gameSpeed += 0.002f;
            if (obstacleTimer > minimumTimeBetweenObstacles + randomAdd)
            {
                CreateObstacle();
            }
        }

        private void CreateObstacle()
        {
            Random random = new Random();
            int tempForType = random.Next(1, 4);
            int berdChance = random.Next(0, 101);

            if (berdChance < 15)
            {
                Bird temp = new Bird(tempForType, windowWidth, bird1, bird2);
                berds.Add(temp);
            }
            else
            {
                Cactus temp = new Cactus(tempForType, windowWidth, cactusSmall, cactusBig, cactusSmallMany);
                cactuses.Add(temp);
            }

            randomAdd = Math.Floor(maxRandomAdd);
            obstacleTimer = 0;
        }

        private void ResetObstacle()
        {
            cactuses = new List<Cactus>();
            berds = new List<Bird>();
            obstacleTimer = 0;
            randomAdd = 0;
            gameSpeed = 10;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.DrawLine(new Vector2(0, ground), new Vector2(Window.ClientBounds.Width, ground), Color.Black, 3);
            spriteBatch.DrawString(scoreFont, $"Game speed: {gameSpeed}", InfoObjectsPositions.Column1.Row3, Color.Black);

            if (DebugClass.AiGameplay)
            {
                spriteBatch.DrawString(scoreFont, $"Generation: {population.gen}", InfoObjectsPositions.Column1.Row4, Color.Black);
                if (!population.Done())
                    population.DrawAlive(gameTime, spriteBatch);

            }
            else
            {
                tempGenPlayer.Draw(spriteBatch, gameTime);
            }


            for (int i = 0; i < cactuses.Count; i++)
            {
                cactuses[i].Draw(spriteBatch, gameTime);
                if (DebugClass.displayObjectsInfo)
                {
                    spriteBatch.DrawString(scoreFont, $"Currenct closest object: Cactus", InfoObjectsPositions.Column3.Row1, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Bottom: {cactuses[0].getTextureRectangle().Bottom} ", InfoObjectsPositions.Column3.Row2, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Top: {cactuses[0].getTextureRectangle().Top} ", InfoObjectsPositions.Column3.Row3, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Left: {cactuses[0].getTextureRectangle().Left} ", InfoObjectsPositions.Column3.Row4, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Right: {cactuses[0].getTextureRectangle().Right} ", InfoObjectsPositions.Column3.Row5, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Width: {cactuses[0].getTextureRectangle().Width} ", InfoObjectsPositions.Column3.Row6, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Height: {cactuses[0].getTextureRectangle().Height} ", InfoObjectsPositions.Column3.Row7, Color.Black);
                }

                if (cactuses[i].posX < 100)
                {
                    cactuses.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < berds.Count; i++)
            {
                if (DebugClass.displayObjectsInfo)
                {
                    spriteBatch.DrawString(scoreFont, $"Currenct closest object: Bird", InfoObjectsPositions.Column3.Row1, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Bottom: {cactuses[0].getTextureRectangle().Bottom} ", InfoObjectsPositions.Column3.Row2, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Top: {cactuses[0].getTextureRectangle().Top} ", InfoObjectsPositions.Column3.Row3, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Left: {cactuses[0].getTextureRectangle().Left} ", InfoObjectsPositions.Column3.Row4, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Right: {cactuses[0].getTextureRectangle().Right} ", InfoObjectsPositions.Column3.Row5, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Width: {cactuses[0].getTextureRectangle().Width} ", InfoObjectsPositions.Column3.Row6, Color.Black);
                    spriteBatch.DrawString(scoreFont, $"Height: {cactuses[0].getTextureRectangle().Height} ", InfoObjectsPositions.Column3.Row7, Color.Black);
                }

                berds[i].Draw(spriteBatch, gameTime);
                if (berds[i].posX < 100)
                {
                    berds.RemoveAt(i);
                    i--;
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
