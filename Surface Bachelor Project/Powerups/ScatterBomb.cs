using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project.Powerups
{
    public class ScatterBomb : Powerup
    {
        protected override string texFile { get { return "Scatter Bomb"; } }

        Random random;

        public ScatterBomb(World world, Gameplay.GameplayScreen screen, Random rand)
            : base(world, screen, rand)
        {
            random = rand;
        }

        protected override void activate(Body owner)
        {
            for (int i = 0; i <= 10; i++)
                gameplay.CreateProjectile(Position, (float)random.NextDouble() * MathHelper.TwoPi, random.Next(51), this);

            Enabled = false;
            IsActive = false;
        }
    }
}
