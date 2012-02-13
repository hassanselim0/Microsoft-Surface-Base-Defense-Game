using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project.Gameplay
{
    public class GameOverScreen : Screen
    {
        SoundEffect sound;
        SpriteFont font;
        Vector2 winOrig, loseOrig;

        Particles.ParticleSystem fireworksParticles;

        bool blue;

        Random rand;
        float timeToNextFirework;

        double elapsedSeconds;

        public GameOverScreen(ScreensComponent manager, bool blueWins)
            : base(manager)
        {
            blue = blueWins;

            rand = new Random();
            timeToNextFirework = 1;

            elapsedSeconds = 0;

            fireworksParticles = new Particles.FireworksParticleSystem(manager.Game, Content, blue);
            fireworksParticles.Initialize();
        }

        public override void LoadContent()
        {
            sound = Content.Load<SoundEffect>("fireworks sound");

            font = Content.Load<SpriteFont>("GameFont");
            
            winOrig = font.MeasureString("You Win") / 2f;
            loseOrig = font.MeasureString("You Lose") / 2f;

            fireworksParticles.CallLoadContent();
            fireworksParticles.SetCamera(Matrix.CreateLookAt(new Vector3(0, 2, 0), Vector3.Zero, Vector3.Forward),
                Matrix.CreateOrthographic(1024, 768, 1, 4));
        }

        public override void Update(GameTime gameTime)
        {
            elapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;

            if (TouchComp.OldContacts.Count() > 0
                && elapsedSeconds > 4 || elapsedSeconds > 30)
            {
                ScreensComp.CurrentScreen = new GameplayScreen(ScreensComp);
                ScreensComp.OverlayScreen = new StartScreen(ScreensComp);
            }

            timeToNextFirework -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if ((timeToNextFirework < 0 || rand.Next(100) == 0) && elapsedSeconds < 26)
            {
                sound.Play(0.4f, 0, 0);

                var pos = new Vector3((float)rand.NextDouble() * 824 - 412, 0, (float)rand.NextDouble() * 568 - 284);
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / 20)
                {
                    fireworksParticles.AddParticle(pos + new Vector3((float)Math.Cos(i), 0, (float)Math.Sin(i)),
                        new Vector3((float)Math.Cos(i) * 60, 0, (float)Math.Sin(i) * 60), 10);
                }

                if (timeToNextFirework < 0)
                    timeToNextFirework += 2;
            }

            fireworksParticles.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            fireworksParticles.Draw(gameTime);

            spriteBatch.Begin();

            if (blue)
                spriteBatch.DrawString(font, "You Lose", new Vector2(512, 464), Color.Red, 0, loseOrig, 1, SpriteEffects.None, 0);
            else
                spriteBatch.DrawString(font, "You Win", new Vector2(512, 464), Color.Red, 0, winOrig, 1, SpriteEffects.None, 0);

            if (blue)
                spriteBatch.DrawString(font, "You Win", new Vector2(512, 304), Color.Blue, MathHelper.Pi, winOrig, 1, SpriteEffects.None, 0);
            else
                spriteBatch.DrawString(font, "You Lose", new Vector2(512, 304), Color.Blue, MathHelper.Pi, loseOrig, 1, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public override void Dispose()
        {
            fireworksParticles.Dispose();
        }
    }
}
