using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components;
using EntityEngine.Engine;

namespace SuperTownDefense.Objects
{
    public class EnemySpawner : Entity
    {
        public List<Entity> Enemies;
        public Timer SoldierTimer;
 
        public EnemySpawner(EntityState es) : base(es)
        {
            Enemies = new List<Entity>();

            SoldierTimer = new Timer(this);
            SoldierTimer.Milliseconds = 3000;
            SoldierTimer.LastEvent += AddSoldier;
            SoldierTimer.Start();
            Components.Add(SoldierTimer);
        }

        public override void Update()
        {
            base.Update();
        }

        public void AddSoldier()
        {
            Soldier s = new Soldier(StateRef);
            s.Render.Layer = .2f;
            Enemies.Add(s);
            AddEntity(s);
        }
    }
}
