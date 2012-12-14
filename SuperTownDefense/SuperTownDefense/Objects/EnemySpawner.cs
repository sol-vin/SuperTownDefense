using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components;
using EntityEngine.Engine;
using EntityEngine.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperTownDefense.Objects
{
    public class EnemySpawner : Entity
    {
        public List<Entity> Enemies;
        public List<Entity> Targets;
 
        public Timer SoldierTimer;
        public Timer HelicopterTimer;
 
        public EnemySpawner(EntityState es) : base(es)
        {
            Enemies = new List<Entity>();
            Targets = new List<Entity>();

            SoldierTimer = new Timer(this);
            SoldierTimer.Milliseconds = 4500;
            SoldierTimer.LastEvent += AddSoldier;
            SoldierTimer.Start();
            Components.Add(SoldierTimer);

            HelicopterTimer = new Timer(this);
            HelicopterTimer.Milliseconds = 5000;
            HelicopterTimer.LastEvent += AddHelicopter;
            HelicopterTimer.Start();
            Components.Add(HelicopterTimer);
        }

        public override void Update()
        {
            base.Update();
        }

        public void AddSoldier()
        {
            Soldier s = new Soldier(StateRef);
            Enemies.Add(s);
            AddEntity(s);
            s.Enemies = Targets;

            foreach (var entity in Targets)
            {
                entity.DestroyEvent += s.RemoveEnemy;
            }
        }

        public void AddHelicopter()
        {
            Helicopter helicopter = new Helicopter(StateRef);
            helicopter.Render.Layer = .2f;
            Enemies.Add(helicopter);
            AddEntity(helicopter);
            helicopter.Enemies = Targets;

            foreach (var entity in Targets)
            {
                entity.DestroyEvent += helicopter.RemoveEnemy;
            }
        }

        public void RemoveEnemy(Entity e)
        {
            Enemies.Remove(e);
        }
    }
}
