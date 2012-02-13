using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Surface_Bachelor_Project.Gameplay
{
    public class GameplayScreen : Screen
    {
        World phyWorld;

        Shield shield;
        Cannon[] cannons;
        Base[] bases;
        Projectile[] projectiles;
        
        Powerups.PowerupManager powerupMgr;

        Texture2D bgTex;
        Texture2D bgLine;

        Particles.ParticleSystem fireParticles;

        public GameplayScreen(ScreensComponent manager)
            : base(manager)
        {
            phyWorld = new World(Vector2.Zero);
            
            powerupMgr = new Powerups.PowerupManager(phyWorld, this);

            fireParticles = new Particles.FireParticleSystem(manager.Game, Content);
            fireParticles.Initialize();
        }

        public override void LoadContent()
        {
            Cannon.SetTexture(Content.Load<Texture2D>("Turret"),
                Content.Load<Texture2D>("Turret Bar Back"), Content.Load<Texture2D>("Turret Bar Front"),
                Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Shoot"));

            Projectile.SetTexture(Content.Load<Texture2D>("Projectile"), Content.Load<Texture2D>("Projectile Trail"));

            Base.SetTexture(Content.Load<Texture2D>("BaseV"), Content.Load<Texture2D>("BaseH"),
                Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Explode"),
                Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Hit"));
            
            powerupMgr.LoadContent(Content);

            cannons = new Cannon[4];
            cannons[0] = new Cannon(phyWorld, this, new Vector2(150, 110));
            cannons[1] = new Cannon(phyWorld, this, new Vector2(874, 110));
            cannons[2] = new Cannon(phyWorld, this, new Vector2(150, 658));
            cannons[3] = new Cannon(phyWorld, this, new Vector2(874, 658));

            bases = new Base[6];
            bases[0] = new Base(phyWorld, new Vector2(512, 40), false, fireParticles);
            bases[1] = new Base(phyWorld, new Vector2(40, 200), true, fireParticles);
            bases[2] = new Base(phyWorld, new Vector2(984, 200), true, fireParticles);
            bases[3] = new Base(phyWorld, new Vector2(40, 568), true, fireParticles);
            bases[4] = new Base(phyWorld, new Vector2(984, 568), true, fireParticles);
            bases[5] = new Base(phyWorld, new Vector2(512, 728), false, fireParticles);

            projectiles = Enumerable.Range(0, 100).Select(x => new Projectile(phyWorld)).ToArray();

            shield = new Shield(phyWorld, TouchComp, projectiles);

            bgTex = Content.Load<Texture2D>("Diagonal Strip");
            bgLine = Content.Load<Texture2D>("Horizontal Strip");

            fireParticles.CallLoadContent();
            fireParticles.SetCamera(Matrix.CreateLookAt(new Vector3(0, 2, 0), Vector3.Zero, Vector3.Forward),
                Matrix.CreateOrthographic(1024, 768, 1, 4));
        }

        public override void Update(GameTime gameTime)
        {
            shield.Update();

            if (ScreensComp.OverlayScreen == null)
                foreach (var c in cannons)
                    c.Update(TouchComp);

            foreach (var b in bases)
                b.Update();

            foreach (var p in projectiles)
                p.Update();

            if (ScreensComp.OverlayScreen == null)
                powerupMgr.Update(gameTime);

            lock (phyWorld)
                phyWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            fireParticles.Update(gameTime);

            if (ScreensComp.OverlayScreen == null)
                if (!(bases[0].Enabled || bases[1].Enabled || bases[2].Enabled))
                    ScreensComp.OverlayScreen = new GameOverScreen(ScreensComp, false);
                else if (!(bases[3].Enabled || bases[4].Enabled || bases[5].Enabled))
                    ScreensComp.OverlayScreen = new GameOverScreen(ScreensComp, true);
        }

        public void CreateProjectile(Vector2 position, float rotation, int power, Body owner)
        {
            foreach (var p in projectiles)
                if (!p.Enabled)
                {
                    p.Init(position, rotation, power, owner);
                    break;
                }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
#if DEBUG
            shield.Draw(spriteBatch);
#endif
            spriteBatch.Draw(bgLine, new Vector2(0, 383), new Color(0, 0.2f, 0.6f));
            spriteBatch.Draw(bgLine, new Vector2(0, 384), new Color(0.6f, 0.2f, 0));

            for (int i = 0; i < 49; i++)
            {
                for (int j = 0; j < 19; j++)
                    spriteBatch.Draw(bgTex, new Vector2(i * 21, j * 21 - 15), new Color(0, 0.2f, 0.6f));
                for (int j = 0; j < 19; j++)
                    spriteBatch.Draw(bgTex, new Vector2(i * 21, j * 21 + 384), new Color(0.6f, 0.2f, 0));
            }

            foreach (var c in cannons) c.Draw(spriteBatch);
            foreach (var b in bases) b.Draw(spriteBatch);
            foreach (var p in projectiles) p.Draw(spriteBatch);
            if (ScreensComp.OverlayScreen == null)
                powerupMgr.Draw(spriteBatch);

            spriteBatch.End();

            fireParticles.Draw(gameTime);
        }

        public override void Dispose()
        {
            shield.Dispose();
            foreach (var c in cannons) c.Dispose();
            foreach (var b in bases) b.Dispose();
            foreach (var p in projectiles) p.Dispose();

            fireParticles.Dispose();
        }
    }
}
