using System.Collections.Generic;
using Terraria.UI;

namespace EmpyreanDreamscape.Core.Mechanics
{   /// <summary>
    /// Cutscene class created by naka. Allows you to draw text, textures, and modify screen elements.
    /// </summary>
    public abstract class Cutscene
	{
		public virtual int InsertionIndex(List<GameInterfaceLayer> layers) => layers.FindIndex(l => l.Name.Equals("Vanilla: Mouse Text"));

		public virtual bool Visible { get; set; } = false;

		public virtual void Draw() { }

		public virtual void ModifyScreenPosition() { }

		public virtual void Begin() => Visible = true;

		public virtual void End() => Visible = false;
	}
}