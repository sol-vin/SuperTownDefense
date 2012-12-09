using EntityEngine.Components;
using EntityEngine.Engine;

namespace SuperTownDefense.Objects.Components
{
    public class Weapon : Component
    {
        public Weapon(Entity e)
            : base(e)
        { }

        public virtual void Fire()
        {
        }
    }
}