﻿using System;
using System.Collections.Generic;
using Trelamium.Core.Abstraction.Interfaces;
using Trelamium.Core.Detours;

namespace Trelamium.Core.Loaders
{
    public sealed class DetourLoader : ILoadableTrelamium
    {
        public float Priority => 1f;

        public bool LoadOnDedServer => false;

        public List<Detour> Detours;

        public void Load()
        {
            Detours = new List<Detour>();

            foreach (var type in Trelamium.Instance.Code.GetTypes())
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