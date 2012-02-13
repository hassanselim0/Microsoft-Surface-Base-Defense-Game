#region File Description
//-----------------------------------------------------------------------------
// FireParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Surface_Bachelor_Project.Particles
{
    /// <summary>
    /// Custom particle system for creating a flame effect.
    /// </summary>
    public class FireworksParticleSystem : ParticleSystem
    {
        bool blue;

        public FireworksParticleSystem(Game game, ContentManager content, bool blueWins)
            : base(game, content)
        {
            blue = blueWins;
        }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "fireworks";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 0;

            settings.MinHorizontalVelocity = -20;
            settings.MaxHorizontalVelocity = 20;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            // Set gravity upside down, so the flames will 'fall' upward.
            if (blue)
                settings.Gravity = new Vector3(0, 0, -40);
            else
                settings.Gravity = new Vector3(0, 0, 40);

            if (blue)
            {
                settings.MinColor = new Color(0, 0, 255, 100);
                settings.MaxColor = new Color(0, 0, 255, 140);
            }
            else
            {
                settings.MinColor = new Color(255, 0, 0, 100);
                settings.MaxColor = new Color(255, 0, 0, 140);
            }

            settings.MinStartSize = 10;
            settings.MaxStartSize = 20;

            settings.MinEndSize = 20;
            settings.MaxEndSize = 80;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.InverseSourceAlpha;
        }
    }
}
