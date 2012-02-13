using System;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;

namespace Surface_Bachelor_Project
{
    public class SurfaceComponent : GameComponent
    {
        private readonly GraphicsDeviceManager graphics;

        private UserOrientation currentOrientation = UserOrientation.Bottom;
        private Matrix screenTransform = Matrix.Identity;
        private Matrix inverted;

        public bool IsApplicationActivated = true;
        public bool IsApplicationPreviewed;

        public SurfaceComponent(Game game, GraphicsDeviceManager graphicsManager)
            : base(game)
        {
            graphics = graphicsManager;
        }

        public override void Initialize()
        {
            SetWindowOnSurface();

            // Set the application's orientation based on the current launcher orientation
            currentOrientation = ApplicationLauncher.Orientation;

            // Subscribe to surface application activation events
            ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;

            // Setup the UI to transform if the UI is rotated.
            // Create a rotation matrix to orient the screen so it is viewed correctly
            // when the user orientation is 180 degress different.
            inverted = Matrix.CreateRotationZ(MathHelper.ToRadians(180)) *
                       Matrix.CreateTranslation(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height, 0);

            if (currentOrientation == UserOrientation.Top)
            {
                screenTransform = inverted;
            }

            base.Initialize();
        }

        /// <summary>
        /// Moves and sizes the window to cover the input surface.
        /// </summary>
        private void SetWindowOnSurface()
        {
            System.Diagnostics.Debug.Assert(Game.Window.Handle != System.IntPtr.Zero,
                "Window initialization must be complete before SetWindowOnSurface is called");
            if (Game.Window.Handle == System.IntPtr.Zero)
                return;

            // We don't want to run in full-screen mode because we need
            // overlapped windows, so instead run in windowed mode
            // and resize to take up the whole surface with no border.

            // Make sure the graphics device has the correct back buffer size.
            InteractiveSurface interactiveSurface = InteractiveSurface.DefaultInteractiveSurface;
            if (interactiveSurface != null)
            {
                graphics.PreferredBackBufferWidth = interactiveSurface.Width;
                graphics.PreferredBackBufferHeight = interactiveSurface.Height;
                graphics.ApplyChanges();

                // Remove the border and position the window.
                Program.RemoveBorder(Game.Window.Handle);
                Program.PositionWindow(Game.Window);
            }
        }

        /// <summary>
        /// Reset the application's orientation and transform based on the current launcher orientation.
        /// </summary>
        private void ResetOrientation()
        {
            UserOrientation newOrientation = ApplicationLauncher.Orientation;

            if (newOrientation == currentOrientation) { return; }

            currentOrientation = newOrientation;

            if (currentOrientation == UserOrientation.Top)
            {
                screenTransform = inverted;
            }
            else
            {
                screenTransform = Matrix.Identity;
            }
        }

        /// <summary>
        /// This is called when application has been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            // Update application state.
            IsApplicationActivated = true;
            IsApplicationPreviewed = false;

            // Orientaton can change between activations.
            ResetOrientation();

            //TODO: Enable audio, animations here

            //TODO: Optionally enable raw image here
        }

        /// <summary>
        /// This is called when application is in preview mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationPreviewed(object sender, EventArgs e)
        {
            // Update application state.
            IsApplicationActivated = false;
            IsApplicationPreviewed = true;

            //TODO: Disable audio here if it is enabled

            //TODO: Optionally enable animations here
        }

        /// <summary>
        ///  This is called when application has been deactivated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
            // Update application state.
            IsApplicationActivated = false;
            IsApplicationPreviewed = false;

            //TODO: Disable audio, animations here

            //TODO: Disable raw image if it's enabled
        }
    }
}
