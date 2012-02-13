using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project.Gameplay
{
    public class StartScreen : Screen
    {
        Texture2D semiAlpha;
        SpriteFont font;
        Vector2 startOrig;

        bool redTouched, blueTouched;

        public StartScreen(ScreensComponent manager)
            : base(manager)
        {
        }

        public override void LoadContent()
        {
            semiAlpha = Content.Load<Texture2D>("Semi Alpha");
            font = Content.Load<SpriteFont>("GameFont");
            startOrig = font.MeasureString("Touch To Start") / 2f;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in TouchComp.OldContacts)
                if (c.CenterY < 384) blueTouched = true;
                else redTouched = true;

            if (blueTouched && redTouched)
                ScreensComp.OverlayScreen = null;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(semiAlpha, new Rectangle(0, 0, 1024, 768), Color.White);

            if (!redTouched)
                spriteBatch.DrawString(font, "Touch To Start", new Vector2(512, 424), Color.Red, 0, startOrig, 1, SpriteEffects.None, 0);

            if (!blueTouched)
                spriteBatch.DrawString(font, "Touch To Start", new Vector2(512, 344), Color.Blue, MathHelper.Pi, startOrig, 1, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public override void Dispose()
        {
        }
    }
}
