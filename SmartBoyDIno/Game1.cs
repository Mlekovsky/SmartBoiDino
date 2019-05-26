using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmartBoyDIno.GameObjects;
using System;
using System.Collections.Generic;

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

        int obstacleTimer;
        int minimumTimeBetweenObstacles = 60;
        double randomAdd = 0;
        double maxRandomAdd = 50;

        float ground = 750;
        float gameSpeed = 10;

        int windowWidth = 1600;
        int windowHeight = 900;

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

            #endregion

            tempGenPlayer = new Player(dino1, dino2, dinoDuck1, dinoDuck2, dinoJump, dinoDead, scoreFont);

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboard = Keyboard.GetState();
            tempGenPlayer.Update(gameTime, keyboard);
            UpdateObstacles();
            MoveObstacles();


            base.Update(gameTime);
        }

        private void MoveObstacles()
        {
            Console.WriteLine($"Current speed: {gameSpeed}");

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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.DrawLine(new Vector2(0, ground), new Vector2(Window.ClientBounds.Width, ground), Color.Black, 3);
            tempGenPlayer.Draw(spriteBatch, gameTime);

            for (int i = 0; i < cactuses.Count; i++)
            {
                cactuses[i].Draw(spriteBatch, gameTime);
                if (cactuses[i].posX < tempGenPlayer.posX)
                {
                    cactuses.RemoveAt(i);
                    i--;
                }

            }

            for (int i = 0; i < berds.Count; i++)
            {
                berds[i].Draw(spriteBatch, gameTime);
                if (berds[i].posX < tempGenPlayer.posX)
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
