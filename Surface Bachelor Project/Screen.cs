using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project
{
    public abstract class Screen : IDisposable
    {
        protected ScreensComponent ScreensComp { get; private set; }

        protected ContentManager Content { get { return ScreensComp.Game.Content; } }

        protected SpriteBatch spriteBatch { get { return ScreensComp.spriteBatch; } }

        protected Viewport viewport { get { return ScreensComp.GraphicsDevice.Viewport; } }

        protected TouchComponent TouchComp { get { return ScreensComp.TouchComp; } }

        public Screen(ScreensComponent screensComp)
        {
            ScreensComp = screensComp;
        }

        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        public abstract void Dispose();
    }
}
