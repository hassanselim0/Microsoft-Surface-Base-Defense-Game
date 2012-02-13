using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project.Powerups
{
    public class PowerupManager
    {
        Type[] types;
        Powerup powerup;

        World phyWorld;
        Gameplay.GameplayScreen gameplay;

        Random rand;
        double remaining;

        ContentManager content;

        public PowerupManager(World world, Gameplay.GameplayScreen screen)
        {
            types = new[] { typeof(ScatterBomb) };
            phyWorld = world;
            gameplay = screen;
            rand = new Random();
            remaining = rand.NextDouble() * 10 + 10;
        }

        public void LoadContent(ContentManager Content)
        {
            content = Content;
        }

        public void Update(GameTime gameTime)
        {
            if (powerup == null)
            {
                if (remaining < 0)
                {
                    remaining = rand.NextDouble() * 10 + 10;
                    lock (phyWorld)
                        powerup = (Powerup)Activator.CreateInstance(
                            types[rand.Next(0, types.Length)], phyWorld, gameplay, rand);
                    powerup.LoadContent(content);
                }
                else
                    remaining -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                if (powerup.IsActive)
                    powerup.Update();
                else
                {
                    phyWorld.RemoveBody(powerup);
                    powerup = null;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (powerup != null && powerup.IsActive)
                powerup.Draw(spriteBatch);
        }
    }
}
