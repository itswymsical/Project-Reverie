using System;
using System.Collections.Generic;
using ReverieMod.Core.Abstraction.Interfaces;
using ReverieMod.Core.Detours;

namespace ReverieMod.Core.Loaders
{
    public sealed class DetourLoader : ILoadableReverie
    {
        public float Priority => 1f;

        public bool LoadOnDedServer => false;

        public List<Detour> Detours;

        public void Load()
        {
            Detours = new List<Detour>();

            foreach (var type in ReverieMod.Instance.Code.GetTypes())
            {
                if (!type.IsAbstract && type.IsSubclassOf(typeof(Detour)))
                {
                    Detour detour = Activator.CreateInstance(type) as Detour;
                    detour?.LoadDetours();

                    Detours.Add(detour);
                }
            }
        }

        public void Unload()
        {
            foreach (var detour in Detours)
            {
                detour?.UnloadDetours();
            }

            Detours?.Clear();
        }
    }
}
