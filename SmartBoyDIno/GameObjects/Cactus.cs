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
    public class Cactus
    {
        Texture2D currentCactus;

        public float posX;
        int w;
        int h;
        int type;
        int posY = 750;

        public Cactus(int t, int windowXSize, Texture2D small, Texture2D big, Texture2D manySmall)
        {
            posX = windowXSize;
            type = t;
            switch (type)
            {
                case 1:
                    w = 40;
                    h = 80;
                    currentCactus = small;
                    ;
                    break;
                case 2:
                    w = 60;
                    h = 120;
                    currentCactus = big;
                    break;
                case 3:
                    w = 120;
                    h = 80;
                    currentCactus = manySmall;
                    break;
                default:
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(currentCactus, new Vector2(posX - (currentCactus.Width / 2), posY - currentCactus.Height));
        }
        public void Update(float gameSpeed)
        {
            posX -= gameSpeed;
        }
    }
}
