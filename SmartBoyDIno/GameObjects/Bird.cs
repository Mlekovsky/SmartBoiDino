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
    public class Bird : BaseEnemy
    {
        float w = 60;
        float h = 50;
        int flaps;
        int groundHeight = 750;
        Texture2D bird1;
        Texture2D bird2;

        public Bird(int type, int windowXSize, Texture2D bird1, Texture2D bird2)
        {
            posX = windowXSize;
            this.bird1 = bird1;
            this.bird2 = bird2;
            switch (type)
            {
                case 1: // low rider
                    posY = groundHeight - 5;
                    break;
                case 2: //mid rider
                    posY = groundHeight - 60;
                    break;
                case 3: // high rider
                    posY = groundHeight - 120;
                    break;
                default:
                    break;
            }
        }

        public void Update(float gameSpeed)
        {
            posX -= gameSpeed;

            flaps++;
            if (flaps > 10)
            {
                currentTexture = bird1;
            }
            else
            {          
                currentTexture = bird2;
            }
            if (flaps > 10)
                flaps -= 10;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(currentTexture, new Vector2(posX - currentTexture.Width / 2, posY - currentTexture.Height));
        }
    }
}
