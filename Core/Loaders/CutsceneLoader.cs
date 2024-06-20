using ReverieMod.Core.Abstraction.Interfaces;
using ReverieMod.Core.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.UI;

namespace ReverieMod.Core.Loaders
{
	internal sealed class CutsceneLoader : ILoadableReverieMod
    {
		public float Priority => 1f;

		public bool LoadOnDedServer => false;

		public static List<Cutscene> Cutscenes = new List<Cutscene>();

		public void Load()
		{
			foreach (Type t in ReverieMod.Instance.Code.GetTypes())
			{
				if (t.IsSubclassOf(typeof(Cutscene)))
				{
					var cutscene = (Cutscene)Activator.CreateInstance(t, null);

					Cutscenes.Add(cutscene);
				}
			}
		}

		public void Unload() => Cutscenes?.Clear();

		public static void AddCutsceneLayer(List<GameInterfaceLayer> layers, Cutscene cutscene, int index, bool visible)
		{
			string name = cutscene == null ? "Unknown" : cutscene.GetType().Name;

			layers.Insert(index, new LegacyGameInterfaceLayer(ReverieMod.AbbreviationPrefix + name,
				delegate 
				{
					if (visible)
						cutscene?.Draw();

					return true;
				},
				InterfaceScaleType.UI
				));
		}
		
		public static T GetCutscene<T>() where T : Cutscene => Cutscenes.FirstOrDefault(c => c is T) as T;
	}
}
