using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project.Gameplay
{
    public class Base: Body
    {
        static Texture2D texV;
        static Vector2 originV;
        static Texture2D texH;
        static Vector2 originH;
        static SoundEffect explosionSnd;
        static SoundEffect hitSnd;

        int health;
        int damage;
        bool vertical;
        double warning;

        Particles.ParticleSystem fireParticles;

        public Base(World world, Vector2 position, bool isVertical, Particles.ParticleSystem fire)
            : base(world)
        {
            health = isVertical ? 20 : 40;
            vertical = isVertical;
            warning = 0;
            fireParticles = fire;

            Position = position / 100;
            BodyType = BodyType.Static;
            Rotation = 0;
            if (vertical) FixtureFactory.AttachRectangle(texV.Width / 100f, texV.Height / 100f, 1, Vector2.Zero, this);
            else FixtureFactory.AttachRectangle(texH.Width / 100f, texH.Height / 100f, 1, Vector2.Zero, this);
            OnCollision += (fA, fB, c) =>
            {
                if (!(fB.Body is Projectile)) return true;

                var p = (Projectile)fB.Body;

                if (p.Damage == 0) return true;

                damage += p.Damage;

                hitSnd.Play(0.2f, -p.Damage / 20f, 0);

                fireParticles.AddParticle(new Vector3(p.Position.X * 100 - 512, 0, p.Position.Y * 100 - 384),
                    Vector3.Zero, p.Damage * 2 + 8);
                
                if (damage >= health)
                {
                    var pos = new Vector3(position.X - 512, 0, position.Y - 384);
                    if (vertical) pos.Z -= texV.Height / 2f;
                    else pos.X -= texH.Width / 2f;

                    for (int i = 0; vertical && i <= texV.Height || !vertical && i <= texH.Width; i+=2)
                    {
                        fireParticles.AddParticle(pos, Vector3.Zero);

                        if (vertical) pos.Z += 2;
                        else pos.X += 2;
                    }
                }

                return true;
            };
        }

        public static void SetTexture(Texture2D textureV, Texture2D textureH, SoundEffect explosionSound, SoundEffect hitSound)
        {
            texV = textureV;
            originV = new Vector2(texV.Width / 2f, texV.Height / 2f);
            texH = textureH;
            originH = new Vector2(texH.Width / 2f, texH.Height / 2f);

            explosionSnd = explosionSound;
            hitSnd = hitSound;
        }

        public void Update()
        {
            if (damage >= health && Enabled)
            {
                Enabled = false;
                explosionSnd.Play(0.2f, -0.4f, 0);
            }

            if (health - damage <= 2) warning += 0.1;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Enabled) return;

            var clr = new Color(damage / (float)health,
                health - damage > 2 ? (1 - damage / (float)health) : ((float)Math.Sin(warning) * 0.2f + 0.2f),
                health - damage > 2 ? 0f : ((float)Math.Sin(warning) * 0.2f + 0.2f));

            if(vertical)
                spriteBatch.Draw(texV, Position * 100, null, clr, 0, originV, 1, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(texH, Position * 100, null, clr, 0, originH, 1, SpriteEffects.None, 0);
        }
    }
}
