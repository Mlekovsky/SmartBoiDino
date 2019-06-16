using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoyDIno.Helpers
{
    public readonly struct DinoTextures
    {
        public readonly Texture2D dinoRun1;
        public readonly Texture2D dinoRun2;
        public readonly Texture2D dinoDuck1;
        public readonly Texture2D dinoDuck2;
        public readonly Texture2D dinoJump;
        public readonly Texture2D dinoDead;
        public readonly SpriteFont font;

        public DinoTextures(Texture2D dino1, Texture2D dino2, Texture2D dinoDuck1, Texture2D dinoDuck2, Texture2D dinoJump, Texture2D dinoDead, SpriteFont font)
        {
            this.dinoRun1 = dino1;
            this.dinoRun2 = dino2;
            this.dinoDuck1 = dinoDuck1;
            this.dinoDuck2 = dinoDuck2;
            this.dinoJump = dinoJump;
            this.dinoDead = dinoDead;
            this.font = font;
        }
    }
}
