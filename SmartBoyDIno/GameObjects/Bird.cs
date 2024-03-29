﻿using System;
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
        int flaps;
        int groundHeight = 750;
        Texture2D bird1;
        Texture2D bird2;

        public Bird(int type, int windowXSize, Texture2D bird1, Texture2D bird2)
        {
            w = 60;
            h = 50;
            posX = windowXSize;
            this.bird1 = bird1;
            this.bird2 = bird2;
            this.type = type;
            switch (type)
            {
                case 1: // low
                    posY = groundHeight - 5;
                    break;
                case 2: // mid
                    posY = groundHeight - 70;
                    break;
                case 3: // high
                    posY = groundHeight - 130;
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
            spriteBatch.Draw(currentTexture, new Vector2(posX - (currentTexture.Width / 2), posY - currentTexture.Height));

            if (DebugClass.displayObjectsInfo)
            {
                DrawRectangle(spriteBatch);
            }
        }
    }
}
