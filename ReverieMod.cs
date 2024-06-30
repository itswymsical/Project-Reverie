using ReverieMod.Common.Players;
using ReverieMod.Common;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using ReverieMod.Core.Abstraction.Interfaces;
using System.Linq;
using System.Reflection;
using ReverieMod.Core.Mechanics.Particles;
using ReverieMod.Core.Mechanics.Trails;
using ReverieMod.Core.Mechanics.Verlet;
using Humanizer;
using ReverieMod.Core.MissionSystem;

namespace ReverieMod
{
    public class ReverieMod : Mod
    {
        public const string Abbreviation = "ReverieMod";

        public const string AbbreviationPrefix = Abbreviation + ":";
        public static ReverieMod Instance => ModContent.GetInstance<ReverieMod>();

        private List<ILoadableReverie> loadCache;

        public override void Load()
        {
            loadCache = new List<ILoadableReverie>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(ILoadableReverie)))
                {
                    object instance = Activator.CreateInstance(type);
                    loadCache.Add(instance as ILoadableReverie);
                }

                loadCache.Sort((n, t) => n.Priority.CompareTo(t.Priority));
            }

            for (int k = 0; k < loadCache.Count; k++)
            {
                loadCache[k].Load();
            }
        }
        public override void Unload()
        {
            if (loadCache != null)
            {
                foreach (ILoadableReverie loadable in loadCache)
                {
                    loadable.Unload();
                }

                loadCache = null;
            }
            else
            {
                Logger.Warn("load cache was null, IOrderedLoadable's may not have been unloaded...");
            }
        }
        public override void PostSetupContent()
        {
            if (!Main.dedServ)
            {
                ParticleManager.Instance.UpdateParticles();
                TrailManager.Instance.UpdateTrails();
                VerletManager.Instance.UpdateChains();
            }
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IPostLoadable)))
                {
                    object toLoad = Activator.CreateInstance(type);

                    ((IPostLoadable)toLoad).Load();
                }
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                case MessageType.AddExperience:
                    int playerID = reader.ReadInt32();
                    int experience = reader.ReadInt32();
                    if (playerID >= 0 && playerID < Main.maxPlayers)
                    {
                        Player player = Main.player[playerID];
                        ExperiencePlayer.AddExperience(player, experience);
                        CombatText.NewText(player.Hitbox, Color.LightGoldenrodYellow, $"+{experience} xp", true);
                    }
                    break;
            }
        }
    }
}