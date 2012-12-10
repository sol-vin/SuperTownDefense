using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components;
using EntityEngine.Data;
using EntityEngine.Engine;
using EntityEngine.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperTownDefense.Objects
{
    public class Bomb : Entity
    {
        private const float Gravity = .1f;
        private Animation _explodeanim;
        private bool _isexploding;
        private ExplosionEmitter _ee;

        public Bomb(EntityState es, Vector2 position, float angle) : base(es)
        {
            Body = new Body(this, position) {Angle = angle};
            Components.Add(Body);

            Physics = new Physics(this);
            Physics.Thrust(8.5f);
            Components.Add(Physics);

            
            _explodeanim = new Animation(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/explosion"), 
                new Vector2(16,16), 30, "explode");
            _explodeanim.Layer = 0.2f;
            _explodeanim.Scale = 2.5f;
            _explodeanim.LastFrameEvent += Destroy;

            Render = new Render(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/bomb"));
            Render.Layer = 0.1f;
            Render.Scale = 1.5f;
            Components.Add(Render);

            Collision = new Collision(this);
            Collision.CollideEvent += CollisionHandler;
            Components.Add(Collision);

            _ee = new ExplosionEmitter(this);
            Components.Add(_ee);
        }

        public override void Update()
        {
            base.Update();
            if (!_isexploding)
            {
                Physics.Velocity.Y += Gravity;
                Body.Angle = (float) Math.Atan2(Physics.Velocity.X, -Physics.Velocity.Y);
                if (Body.Position.Y > 500)
                    _isexploding = true;
            }
            else if (_isexploding)
            {
                Physics.Velocity = Vector2.Zero;
                if (Render != _explodeanim)
                {
                    Components.Remove(Render);
                    Render = _explodeanim;
                    Components.Add(Render);
                    Body.Position -= Render.Origin * Render.Scale;

                    _explodeanim.Start();
                    _ee.Emit(20);
                }
            }
        }

        public void Destroy()
        {
            Destroy(null);
        }

        public void CollisionHandler(Entity e)
        {
            e.Destroy();
        }
    }

    class ExplosionEmitter : Emitter
    {
        Random _rand = new Random(DateTime.Now.Millisecond);
        public ExplosionEmitter(Entity e) : base(e, e.StateRef.GameRef.Game.Content.Load<Texture2D>(@"game/explosionparticle"), Vector2.One * 5)
        {
            
        }

        protected override Particle GenerateNewParticle()
        {
            int index = _rand.Next(0, 3);
            int ttl = _rand.Next(50, 80);

            ExplosionParticle p = new ExplosionParticle(index, Entity.Body.Position + Entity.Render.Origin * Entity.Render.Scale, ttl, this);
            p.Body.Angle = (float)_rand.NextDouble() - .5f*1.5f;
            p.Physics.Thrust(((float)_rand.NextDouble() + 1f) * 2.5f);
            p.Render.Layer = .5f;
            return p;
        }
    }

    class ExplosionParticle : FadeParticle
    {
        
        public ExplosionParticle(int index, Vector2 position, int ttl, Emitter e) : base(index,position,10,ttl,e)
        {
            
        }

        public override void Update()
        {
            base.Update();
            Physics.Velocity.Y += .1f;
        }
    }
}
