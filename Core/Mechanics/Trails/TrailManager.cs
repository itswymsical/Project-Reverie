using ReverieMod.Core.Abstraction.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ReverieMod.Core.Mechanics.Trails
{
    public class TrailManager : ILoadableReverie
    {
        public float Priority => 1f;

        public bool LoadOnDedServer => true;

        public static TrailManager Instance { get; private set; }

        public List<Trail> Trails;

        public void Load()
        {
            Trails = new List<Trail>();

            Instance = this;
        }

        public void Unload()
        {
            Trails?.Clear();

            Instance = null;
        }

        public void DrawTrails()
        {
            foreach (var trail in Trails)
            {
                trail.Draw();
            }
        }

        public void UpdateTrails()
        {
            if (Main.gameInactive)
            {
                return;
            }

            foreach (var trail in Trails)
            {
                trail.Update();
            }
        }

        public Trail CreateTrail(Trail trail)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return trail;
            }

            Trails.Add(trail);

            return trail;
        }
    }
}
