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

        public void DrawRectangle(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(new Vector2(posX - currentTexture.Width / 2, posY - currentTexture.Height), new Vector2(posX + currentTexture.Width / 2, posY - currentTexture.Height), Color.Red);
            spriteBatch.DrawLine(new Vector2(posX - currentTexture.Width / 2, posY), new Vector2(posX + currentTexture.Width / 2, posY), Color.Red);
            spriteBatch.DrawLine(new Vector2(posX - currentTexture.Width / 2, posY - currentTexture.Height), new Vector2(posX - currentTexture.Width / 2, posY), Color.Red);
            spriteBatch.DrawLine(new Vector2(posX + currentTexture.Width / 2, posY - currentTexture.Height), new Vector2(posX + currentTexture.Width / 2, posY), Color.Red);
        }
    }
}
