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
using FarseerPhysics.Collision.Shapes;

namespace Surface_Bachelor_Project.Gameplay
{
    public class Shield : Body, IDisposable
    {
        TouchComponent touchComp;
        Vertices verts;
        List<Vertices> vertsList;
        Projectile[] projs;

        Thread updateThread;
        volatile Semaphore threadSem;
        volatile int threadSemCount;
#if DEBUG
        Texture2D debugImg;
        byte[] debugImgData;
#endif

        public Shield(World world, TouchComponent touch, Projectile[] projectiles)
            : base(world)
        {
            BodyType = BodyType.Static;

            touchComp = touch;
            verts = new Vertices(100);
            vertsList = new List<Vertices>(100);
            projs = projectiles;

            updateThread = new Thread(new ThreadStart(update));
            updateThread.Priority = ThreadPriority.Lowest;
            threadSem = new Semaphore(0, 2);
            threadSemCount = 0;
            updateThread.Start();
#if DEBUG
            debugImg = new Texture2D(touchComp.Game.GraphicsDevice, 1024, 768,
                1, TextureUsage.AutoGenerateMipMap, SurfaceFormat.Luminance8);
            debugImgData = new byte[1024 * 768];
#endif
        }

        public void Update()
        {
            if (threadSemCount <= 0)
            {
                threadSemCount++;
                threadSem.Release();
            }
        }

        private void update()
        {
            while (true)
            {
                //var sw = new System.Diagnostics.Stopwatch();
                //sw.Start();

                if (touchComp.RawImageBytes != null)
                {
                    vertsList.Clear();
                    foreach (var p in projs.Where(p1 => p1.Enabled))
                    {
                        verts.Clear();

                        for (int i = (int)(p.Position.X * 100) - 10; i < (int)(p.Position.X * 100) + 10; i++)
                            for (int j = (int)(p.Position.Y * 100) - 10; j < (int)(p.Position.Y * 100) + 10; j++)
                                if (touchComp.GetScaledFilteredValueAt(i, j) != 0)
                                    verts.Add(new Vector2(i / 100f, j / 100f));

                        if (verts.Count > 2)
                            vertsList.AddRange(EarclipDecomposer.ConvexPartition(verts)
                                .Where(v => v.GetArea() > FarseerPhysics.Settings.Epsilon).ToList());
                    }

                    for (int i = 0; i < vertsList.Count; i++)
                    {
                        if (i < FixtureList.Count)
                            ((PolygonShape)FixtureList[i].Shape).Set(vertsList[i]);
                        else
                            lock (World) FixtureFactory.AttachPolygon(vertsList[i], 1, this);
                    }

                    lock (World)
                    {
                        for (int i = vertsList.Count; i < FixtureList.Count; i++)
                            DestroyFixture(FixtureList[i]);
                    }
                }

                //sw.Stop();
                //System.Diagnostics.Debug.WriteLine(sw.ElapsedMilliseconds);

                threadSemCount--;
                threadSem.WaitOne();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
#if DEBUG
            if (touchComp.RawImageBytes != null)
            {
                for (int i = 0; i < debugImgData.Length; i++)
                    debugImgData[i] = 0;

                lock (World)
                {
                    foreach (var f in FixtureList)
                        foreach (var v in ((PolygonShape)f.Shape).Vertices)
                        {
                            int i = (int)(v.X * 100);
                            int j = (int)(v.Y * 100);
                            if (i > 0 && i < 1024 && j > 0 && j < 768)
                                debugImgData[j * 1024 + i] = 255;
                        }
                }

                debugImg.SetData<byte>(debugImgData, 0, debugImgData.Length, SetDataOptions.Discard);

                spriteBatch.Draw(debugImg, Vector2.Zero, null, Color.White,
                    0f, Vector2.Zero, 1, SpriteEffects.None, 1);
            }
#endif
        }

        public new void Dispose()
        {
            updateThread.Abort();
            base.Dispose();
        }
    }
}
