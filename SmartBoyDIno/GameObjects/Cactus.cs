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
    public class Cactus : BaseEnemy
    {
        public Cactus(int t, int windowXSize, Texture2D small, Texture2D big, Texture2D manySmall)
        {
            posX = windowXSize;
            posY = 750;
            type = t;
            switch (type)
            {
                case 1:
                    w = 40;
                    h = 80;
                    currentTexture = small;                   
                    break;
                case 2:
                    w = 60;
                    h = 120;
                    currentTexture = big;
                    break;
                case 3:
                    w = 120;
                    h = 80;
                    currentTexture = manySmall;
                    break;
                default:
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(currentTexture, new Vector2(posX - (currentTexture.Width / 2), posY - currentTexture.Height));

            if (DebugClass.displayObjectsInfo)
            {
                DrawRectangle(spriteBatch);
            }
        }
        public void Update(float gameSpeed)
        {
            posX -= gameSpeed;
        }
    }
}
