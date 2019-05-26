using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmartBoyDIno.GameObjects;
using System;

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

        float ground = 750;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();
            base.Initialize();
        }
        protected override void LoadContent()
        {
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
            tempGenPlayer = new Player(dino1, dino2, dinoDuck1, dinoDuck2, dinoJump, dinoDead, scoreFont);

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboard = Keyboard.GetState();
            tempGenPlayer.Update(gameTime, keyboard);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.DrawLine(new Vector2(0, ground), new Vector2(Window.ClientBounds.Width, ground), Color.Black, 3);
            tempGenPlayer.Draw(spriteBatch, gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
