using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Engine;

namespace SuperTownDefense.Objects
{
    public class Enemy : Entity
    {
        public int Points;
        public List<Entity> Enemies;
        public bool KilledByPlayer;

        public Enemy(EntityState es) : base(es)
        {
            Enemies = new List<Entity>();
        }

        public override void Destroy(Entity e = null)
        {
            base.Destroy(e);
            if(KilledByPlayer)
                GameState.Score += Points;
        }

        public void RemoveEnemy(Entity e)
        {
            Enemies.Remove(e);
        }

        public void AddEnemy(Entity e)
        {
            Enemies.Add(e);
        }
    }
}
