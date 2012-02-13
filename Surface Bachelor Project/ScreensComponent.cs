using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project
{
    public class ScreensComponent : DrawableGameComponent
    {
        public TouchComponent TouchComp { get; private set; }

        public SpriteBatch spriteBatch { get; private set; }

        private Screen currentScreen;
        public Screen CurrentScreen
        {
            get { return currentScreen; }
            set
            {
                if (currentScreen != null)
                    currentScreen.Dispose();
                currentScreen = value;
                currentScreen.LoadContent();
            }
        }

        private Screen overlayScreen;
        public Screen OverlayScreen
        {
            get { return overlayScreen; }
            set
            {
                if (overlayScreen != null)
                    overlayScreen.Dispose();

                overlayScreen = value;

                if (overlayScreen != null)
                    overlayScreen.LoadContent();
            }
        }

        public ScreensComponent(Game game, SpriteBatch sb)
            : base(game)
        {
            currentScreen = new Gameplay.GameplayScreen(this);
            //overlayScreen = new Gameplay.StartScreen(this);
            spriteBatch = sb;
        }

        public override void Initialize()
        {
            TouchComp = Game.Components.OfType<TouchComponent>().First();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            currentScreen.LoadContent();
            if (overlayScreen != null)
                overlayScreen.LoadContent();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);

            if (overlayScreen != null)
                overlayScreen.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            currentScreen.Draw(gameTime);

            if (overlayScreen != null)
                overlayScreen.Draw(gameTime);

            base.Draw(gameTime);

            //if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            //{
            //    var rt = new ResolveTexture2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth,
            //        GraphicsDevice.PresentationParameters.BackBufferHeight, 1, SurfaceFormat.Color);
            //    GraphicsDevice.ResolveBackBuffer(rt);
            //    rt.Save("Snapshot.png", ImageFileFormat.Png);
            //}
        }

        protected override void Dispose(bool disposing)
        {
            currentScreen.Dispose();
            if (overlayScreen != null) overlayScreen.Dispose();

            base.Dispose(disposing);
        }
    }
}
