namespace Trelamium.Core.Detours
{
    public abstract class Detour
    {
        public virtual void LoadDetours() { }

        public virtual void UnloadDetours() { }
    }
}
