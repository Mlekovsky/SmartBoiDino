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
    public class Bird
    {
        float w = 60;
        float h = 50;
        public float posX;
        float posY;
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
        }

        public void Draw(SpriteBatch spriteBatch ,GameTime gameTime)
        {
            flaps++;
            if(flaps > 10)
            {
                spriteBatch.Draw(bird1, new Vector2(posX - bird1.Width/2, posY - bird1.Height));
            }
            else
            {
                spriteBatch.Draw(bird2, new Vector2(posX - bird2.Width / 2, posY - bird2.Height));
            }
            if (flaps > 10)
                flaps -= 10;
        }
    }
}
