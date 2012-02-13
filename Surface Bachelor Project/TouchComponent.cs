using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Surface.Core;
using Microsoft.Surface.Core.Manipulations;
using Microsoft.Xna.Framework;

namespace Surface_Bachelor_Project
{
    public class TouchComponent : GameComponent
    {
        private SurfaceComponent surfaceComp;

        private ContactTarget contactTarget;

        public ReadOnlyContactCollection CurrentContacts { get; private set; }
        public ReadOnlyContactCollection PreviousContacts { get; private set; }

        public IEnumerable<Contact> NewContacts { get; private set; }
        public IEnumerable<Contact> OldContacts { get; private set; }

        private Affine2DManipulationProcessor manipulationProcessor;

        public bool IsManipulating { get; private set; }

        private byte[] rawImageBytes;
        public byte[] RawImageBytes { get { return rawImageBytes; } }
        private ImageMetrics rawImageMetrics;
        public ImageMetrics RawImageMetrics { get { return rawImageMetrics; } }

        public TouchComponent(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Initializes the surface input system. This should be called after any window
        /// initialization is done, and should only be called once.
        /// </summary>
        public override void Initialize()
        {
            surfaceComp = Game.Components.OfType<SurfaceComponent>().First();

            System.Diagnostics.Debug.Assert(Game.Window.Handle != System.IntPtr.Zero,
                "Window initialization must be complete before InitializeSurfaceInput is called");
            if (Game.Window.Handle == System.IntPtr.Zero)
                return;
            System.Diagnostics.Debug.Assert(contactTarget == null,
                "Surface input already initialized");
            if (contactTarget != null)
                return;

            // Create a target for surface input.
            contactTarget = new ContactTarget(Game.Window.Handle, EventThreadChoice.OnBackgroundThread);
            contactTarget.EnableInput();

            contactTarget.EnableImage(ImageType.Normalized);
            contactTarget.FrameReceived += new EventHandler<FrameReceivedEventArgs>(contactTarget_FrameReceived);

            CurrentContacts = contactTarget.GetState();

            manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.TranslateX | Affine2DManipulations.TranslateY
                | Affine2DManipulations.Rotate | Affine2DManipulations.Scale);
            manipulationProcessor.Affine2DManipulationDelta += new EventHandler<Affine2DOperationDeltaEventArgs>(manipulationDelta);
            manipulationProcessor.Affine2DManipulationStarted += (s, e) => IsManipulating = true;
            manipulationProcessor.Affine2DManipulationCompleted += (s, e) => IsManipulating = false;

            base.Initialize();
        }

        private void contactTarget_FrameReceived(object sender, FrameReceivedEventArgs e)
        {
            int padRight, padLeft;

            if (rawImageBytes == null)
            {
                e.TryGetRawImage(ImageType.Normalized,
                    0,
                    InteractiveSurface.DefaultInteractiveSurface.Top,
                    InteractiveSurface.DefaultInteractiveSurface.Width,
                    InteractiveSurface.DefaultInteractiveSurface.Height,
                    out rawImageBytes, out rawImageMetrics, out padLeft, out padRight);
            }
            else
            {
                e.UpdateRawImage(ImageType.Normalized, rawImageBytes,
                    0,
                    InteractiveSurface.DefaultInteractiveSurface.Top,
                    InteractiveSurface.DefaultInteractiveSurface.Width,
                    InteractiveSurface.DefaultInteractiveSurface.Height);
            }
        }

        private void manipulationDelta(object sender, Affine2DOperationDeltaEventArgs e)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!surfaceComp.IsApplicationActivated) return;

            PreviousContacts = CurrentContacts;
            CurrentContacts = contactTarget.GetState();

            NewContacts = CurrentContacts.Where(c => !PreviousContacts.Contains(c.Id));
            OldContacts = PreviousContacts.Where(c => !CurrentContacts.Contains(c.Id));

            //var removedManipulators = OldContacts.Select(c => new Manipulator(c.Id, c.CenterX, c.CenterY));
            //var currentManipulators = CurrentContacts.Select(c => new Manipulator(c.Id, c.CenterX, c.CenterY));

            //if (currentManipulators.Count() > 0 || removedManipulators.Count() > 0)
            //    manipulationProcessor.ProcessManipulators(currentManipulators, removedManipulators);

            base.Update(gameTime);
        }

        public byte GetScaledRawValueAt(Vector2 point)
        {
            return GetScaledRawValueAt((int)point.X, (int)point.Y);
        }

        public byte GetScaledRawValueAt(int x, int y)
        {
            x = (int)Math.Min(767, x * 0.75f + 2);
            y = (int)Math.Min(575, y * 0.75f + 2);

            return getRawValueAt(x, y);
        }

        public byte GetScaledFilteredValueAt(int x, int y)
        {
            x = (int)Math.Min(767, x * 0.75f + 2);
            y = (int)Math.Min(575, y * 0.75f + 2);

            bool[,] v = new bool[3, 3];

            for (int i = Math.Max(0, x - 1); i <= Math.Min(767, x + 1); i++)
                for (int j = Math.Max(0, y - 1); j <= Math.Min(575, y + 1); j++)
                    v[i - x + 1, j - y + 1] = getRawValueAt(i, j) > 60;

            //for (int i = 0; i < 5; i++)
            //    for (int j = 0; j < 5; j++)
            //        if (!(i == 0 && j == 0) && !(i == 4 && j == 0)
            //            && !(i == 0 && j == 4) && !(i == 4 && j == 4) && v[i, j])
            //            return 0;

            //if (v[0, 0] || v[4, 0] || v[0, 4] || v[4, 4])
            //    return 255;

            //if ((v[0, 0] || v[0, 2] || v[2, 2] || v[2, 0])
            //    && !(v[0, 1] || v[2, 1] || v[1, 0] || v[1, 2] || v[1, 1]))
            //    return 255;

            if ((v[0, 1] || v[2, 1] || v[1, 0] || v[1, 2]) && !v[1, 1])
                return 255;

            //int count = v.Cast<bool>().Count(b => b);

            //if (count > 1 && count < 4)
            //    return 255;

            return 0;
        }

        private byte getRawValueAt(int x, int y)
        {
            return rawImageBytes[y * 768 + x];
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Release managed resources.
                if (contactTarget != null)
                {
                    contactTarget.Dispose();
                    contactTarget = null;
                }
            }

            // Release unmanaged Resources.
            // Set large objects to null to facilitate garbage collection.

            base.Dispose(disposing);
        }
    }
}
