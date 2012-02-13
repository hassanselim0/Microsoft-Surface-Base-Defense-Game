using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project.Gameplay
{
    public class Projectile : Body
    {
        static Texture2D tex, trailTex;
        static Vector2 origin, trailOrigin;

        public int Damage { get; private set; }

        int collided;
        Body parent;
        float scale;
        Color clr1, clr2;

        Vector2[] trailPos;

        public Projectile(World world)
            : base(world)
        {
            trailPos = new Vector2[10];

            Enabled = false;
            BodyType = BodyType.Dynamic;

            FixtureFactory.AttachCircle(tex.Width / 200f, 1, this);
            FixtureList[0].OnCollision = new OnCollisionEventHandler(collision);
        }

        public void Init(Vector2 position, float rotation, int power, Body owner)
        {
            Damage = Math.Min(power / 10 + 1, 10);
            collided = 0;
            parent = owner;
            scale = Damage / 20f + 0.22f;

            if (parent is Powerups.Powerup)
                clr1 = clr2 = new Color(1f, 1f, 0f);
            else if (parent.Position.Y > 3.84f)
                clr1 = clr2 = new Color(1, 0.4f, 0);
            else
                clr1 = clr2 = new Color(0, 0.4f, 1);
            clr2.A = 50;

            lock (World)
            {
                Rotation = rotation;
                LinearVelocity = new Vector2((float)Math.Cos(Rotation) * 4,
                    (float)Math.Sin(Rotation) * 4);
                Restitution = 0.2f;
                ((CircleShape)FixtureList[0].Shape).Radius = tex.Width / 200f * scale;
                Position = position + Vector2.Normalize(LinearVelocity)
                    * (parent.FixtureList[0].Shape.Radius + FixtureList[0].Shape.Radius + 0.01f);
                Enabled = true;
            }

            for (int i = 0; i < trailPos.Length; i++) trailPos[i] = Position;
        }

        public static void SetTexture(Texture2D texture, Texture2D trailTexture)
        {
            tex = texture;
            origin = new Vector2(tex.Width / 2f, tex.Height / 2f);
            trailTex = trailTexture;
            trailOrigin = new Vector2(trailTexture.Width / 2f, trailTexture.Height / 2f);
        }

        public void Update()
        {
            var pos = Position * 100;
            if (pos.X < 0 || pos.X > 1024 || pos.Y < 0 || pos.Y > 768)
                lock (World) Enabled = false;

            //if (Enabled && touchComp.GetScaledRawValueAt(Position * 100) > 60)
            //{
            //    if (collided == 0) collided = 80;
            //    LinearVelocity *= -0.6f;
            //}

            if (collided > 1)
            {
                collided--;
                LinearVelocity /= 1.04f;
            }
            else if (collided == 1)
            {
                collided = 0;
                lock (World) Enabled = false;
            }

            if (Enabled)
            {
                trailPos[0] = Position + LinearVelocity / 80f;
                for (int i = 1; i < trailPos.Length; i++)
                    trailPos[i] += (trailPos[i - 1] - trailPos[i]) / (1 + scale);
            }
        }

        private bool collision(Fixture fA, Fixture fB, FarseerPhysics.Dynamics.Contacts.Contact c)
        {
            if (parent is Powerups.Powerup && fB.Body is Projectile && ((Projectile)fB.Body).parent is Powerups.Powerup)
                return false;

            if (collided == 0)
            {
                collided = 80;
                Damage = 0;
                LinearVelocity *= 1.8f;
            }

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Enabled) return;

            float s = scale;
            for (int i = 0; i < trailPos.Length; i++)
            {
                s -= scale / 24;
                spriteBatch.Draw(trailTex, trailPos[i] * 100, null, clr2, Rotation, trailOrigin, s, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(tex, Position * 100, null, clr1, Rotation, origin, scale, SpriteEffects.None, 0);
        }
    }
}
