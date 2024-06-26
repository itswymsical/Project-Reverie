using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Core.Abstraction.Interfaces;
using System.Collections.Generic;

namespace ReverieMod.Core.Mechanics.Verlet
{
    public class VerletManager : ILoadableReverieMod
    {
        public float Priority => 1f;

        public bool LoadOnDedServer => true;

        public static VerletManager Instance { get; private set; }

        public List<VerletChain> Chains;

        public void Load()
        {
            Chains = new List<VerletChain>();

            Instance = this;
        }

        public void Unload()
        {
            Chains?.Clear();

            Instance = null;
        }

        public void UpdateChains()
        {
            foreach (var chain in Chains)
            {
                chain.Update();
            }
        }

        public void DrawChains(SpriteBatch spriteBatch)
        {
            foreach (var chain in Chains)
            {
                chain.Draw(spriteBatch);
            }
        }

        public VerletChain CreateChain(VerletChain chain)
        {
            Chains.Add(chain);

            return chain;
        }
    }
}
