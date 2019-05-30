using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmartBoyDIno.GameObjects
{
    public class BaseGameObject
    {
        public Texture2D currentTexture;
        public float posY;
        public float posX;

        public Rectangle getTextureRectangle()
        {
            return new Rectangle
            {
                X = (int)this.posX - this.currentTexture.Width,
                Y = (int)this.posY - this.currentTexture.Height,
                Height = this.currentTexture.Height,
                Width = this.currentTexture.Width
            };
        }
    }
}
