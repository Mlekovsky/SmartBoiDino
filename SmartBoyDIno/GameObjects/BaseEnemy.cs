﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoyDIno.GameObjects
{
    public class BaseEnemy : BaseGameObject
    {
        public BaseEnemy()
        {
        }

        public bool PlayerTouched(Player player)
        {
            return new Rectangle
            {
                X = (int)player.posX - player.currentTexture.Width,
                Y = (int)player.posY - player.currentTexture.Height,
                Height = player.currentTexture.Height,
                Width = player.currentTexture.Width
            }.Intersects(
                    new Rectangle
                    {
                        X = (int)this.posX - this.currentTexture.Width,
                        Y = (int)this.posY - this.currentTexture.Height,
                        Height = this.currentTexture.Height,
                        Width = this.currentTexture.Width,
                    });
        }
    }
}