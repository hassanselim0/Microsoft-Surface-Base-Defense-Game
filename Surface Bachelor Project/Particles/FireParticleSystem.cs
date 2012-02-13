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
    public class FireParticleSystem : ParticleSystem
    {
        public FireParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "fire";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 0;

            settings.MinHorizontalVelocity = -40;
            settings.MaxHorizontalVelocity = 40;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 0, 0);

            settings.MinColor = new Color(255, 255, 255, 100);
            settings.MaxColor = new Color(255, 255, 255, 140);

            settings.MinStartSize = 25;
            settings.MaxStartSize = 50;

            settings.MinEndSize = 50;
            settings.MaxEndSize = 200;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.InverseSourceAlpha;
        }
    }
}
