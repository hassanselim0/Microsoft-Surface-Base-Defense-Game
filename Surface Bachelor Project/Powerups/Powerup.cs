using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;

namespace Surface_Bachelor_Project.Powerups
{
    public abstract class Powerup : Body
    {
        public bool IsActive { get; protected set; }

        protected Gameplay.GameplayScreen gameplay;

        protected Texture2D tex;
        protected Vector2 texOrig;
        protected abstract string texFile { get; }

        public Powerup(World world, Gameplay.GameplayScreen screen, Random rand)
            : base(world)
        {
            IsActive = true;

            gameplay = screen;

            Position = new Vector2((float)rand.NextDouble() * 7.04f + 1.60f, (float)rand.NextDouble() * 2.88f + 1.60f);
            BodyType = BodyType.Static;
        }

        public virtual void LoadContent(ContentManager Content)
        {
            tex = Content.Load<Texture2D>(texFile);
            texOrig = new Vector2(tex.Width / 2f, tex.Height / 2f);
            FixtureFactory.AttachCircle(texOrig.X / 100, 1, this);
            OnCollision += new OnCollisionEventHandler(onCollision);
        }

        public virtual void Update()
        {
        }

        protected bool onCollision(Fixture fA, Fixture fB, FarseerPhysics.Dynamics.Contacts.Contact c)
        {
            if (fB.Body is Gameplay.Projectile)
            {
                activate(fB.Body);
                fB.Body.Enabled = false;
                return false;
            }
            else
                return true;
        }

        protected abstract void activate(Body owner);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Enabled)
                spriteBatch.Draw(tex, Position * 100, null, Color.White, 0, texOrig, 1, SpriteEffects.None, 0);
        }
    }
}
