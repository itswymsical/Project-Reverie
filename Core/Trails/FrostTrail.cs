using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Core.Mechanics.Trails;
using Terraria;
public class FrostTrail : Trail
{
    private Player player;

    public FrostTrail(GraphicsDevice graphicsDevice, Player player) : base(graphicsDevice, null)
    {
        this.player = player;
        SetDefaults();
    }

    public override void SetDefaults()
    {
        Alpha = 0.75f;
        Color = Color.Cyan;
        Width = 5f;
        Size = 40;
    }

    public override void SetVertices()
    {
        if (player == null || player.HeldItem == null) return;

        Vector2 swordTip = player.MountedCenter + new Vector2(0, player.gravDir * -1) + new Vector2(0, -player.height / 2);
        swordTip += player.HeldItem.Size / 2 * new Vector2(player.direction, 0).RotatedBy(player.itemRotation);

        Points.Add(swordTip);

        if (Points.Count > Size)
        {
            Points.RemoveAt(0);
        }

        for (int i = 0; i < Points.Count; i++)
        {
            AddVertex(Points[i], new Vector2((float)i / Points.Count, 0));
            AddVertex(Points[i] + new Vector2(Width, 0), new Vector2((float)i / Points.Count, 1));
        }

        PointCount = Points.Count * 2;
    }

    public override void Update()
    {
        if (player == null || !player.active || player.HeldItem == null || player.itemAnimation <= 0)
        {
            Kill();
        }
    }
}