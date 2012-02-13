using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project.Gameplay
{
    public class Cannon : Body
    {
        static Texture2D tex, barBackTex, barFrontTex;
        static Vector2 origin, barBackOrig, barFrontOrig;
        static float radius;
        static SoundEffect shootSnd;

        GameplayScreen screen;

        Contact contact;
        float angle;
        int count;

        public Cannon(World world, GameplayScreen gameplayScreen, Vector2 position)
            : base(world)
        {
            screen = gameplayScreen;
            Position = position / 100;

            BodyType = BodyType.Static;
            FixtureFactory.AttachCircle(radius / 100f, 1, this);
        }

        public static void SetTexture(Texture2D texture, Texture2D barBackTexture, Texture2D barFrontTexture, SoundEffect shootSound)
        {
            tex = texture;
            origin = new Vector2(tex.Width / 2f, tex.Height / 2f);
            radius = MathHelper.Max(origin.X, origin.Y);

            barBackTex = barBackTexture;
            barBackOrig = new Vector2(barBackTex.Width / 2f, -42);

            barFrontTex = barFrontTexture;
            barFrontOrig = new Vector2(barFrontTex.Width / 2f, -40);

            shootSnd = shootSound;
        }

        public void Update(TouchComponent touchComp)
        {
            foreach (var c in touchComp.CurrentContacts)
                if (new Vector2(c.CenterX - Position.X * 100, c.CenterY - Position.Y * 100).Length() < radius)
                {
                    if (contact != null) contact = c;
                    if (touchComp.NewContacts.Any(c1 => c1.Id == c.Id))
                    {
                        contact = c;
                        angle = c.Orientation;
                        count = 0;
                    }
                }

            if (contact != null && touchComp.CurrentContacts.Contains(contact.Id))
            {
                var deltaRot = contact.Orientation - angle;

                if (deltaRot > 5.8f && deltaRot < 7)
                    angle += MathHelper.TwoPi;
                else if (deltaRot < -5.8f && deltaRot > -7)
                    angle -= MathHelper.TwoPi;

                if (Math.Abs(deltaRot) < 1)
                {
                    angle = (angle * Math.Min(count, 19) + contact.Orientation) / Math.Min(count + 1, 20);
                    count++;
                }
            }

            if (contact != null && touchComp.OldContacts.Any(c => c.Id == contact.Id))
            {
                screen.CreateProjectile(Position, angle, count, this);
                shootSnd.Play(0.2f, -Math.Min(count, 100) / 100f, 0);
                contact = null;
                count = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, Position * 100, null, Color.White, 0, origin, 1, SpriteEffects.None, 0);
            
            if (contact != null)
            {
                if (Position.X < 5.12 && Position.Y > 3.84 || Position.X > 5.12 && Position.Y < 3.84)
                {
                    spriteBatch.Draw(barBackTex, Position * 100, null, Color.White, angle,
                       barBackOrig, 1, SpriteEffects.None, 0);

                    spriteBatch.Draw(barFrontTex, Position * 100,
                        new Rectangle(0, 0, Math.Min((int)(count * 0.48f), 44), barFrontTex.Height),
                        Color.White, angle, barFrontOrig, 1, SpriteEffects.None, 0);
                }
                else
                {
                    var rot = angle + MathHelper.Pi;

                    spriteBatch.Draw(barBackTex, Position * 100, null, Color.White, rot,
                       barBackOrig, 1, SpriteEffects.None, 0);

                    spriteBatch.Draw(barFrontTex, Position * 100 + (barFrontTex.Width - Math.Min((int)(count * 0.48f), 44)) * new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)),
                        new Rectangle(barFrontTex.Width - Math.Min((int)(count * 0.48f), 44), 0, Math.Min((int)(count * 0.48f), 44), barFrontTex.Height),
                        Color.White, rot, barFrontOrig, 1, SpriteEffects.None, 0);
                }
            }
        }
    }
}
