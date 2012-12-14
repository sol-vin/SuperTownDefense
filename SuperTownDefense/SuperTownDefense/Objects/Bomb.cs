using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components;
using EntityEngine.Data;
using EntityEngine.Engine;
using EntityEngine.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace SuperTownDefense.Objects
{
    public class Bomb : Entity
    {
        private const float Gravity = .1f;
        private Animation _explodeanim;
        private bool _isexploding;
        private ExplosionEmitter _ee;
        private SmokeEmitter _se;
        private Sound _explodesound;

        public float Damage = 1;

        public Bomb(EntityState es, Vector2 position, float angle, float thrust) : base(es)
        {
            Body = new Body(this, position) {Angle = angle};
            Components.Add(Body);

            Physics = new Physics(this);
            Physics.Thrust(thrust);
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

            _se = new SmokeEmitter(this);
            Components.Add(_se);

            _explodesound = new Sound(this, StateRef.GameRef.Game.Content.Load<SoundEffect>(@"game/sounds/explosion"));
            _explodesound.Volume = .5f;
            Components.Add(_explodesound);
        }

        public override void Update()
        {
            base.Update();
            if (!_isexploding)
            {
                Physics.Velocity.Y += Gravity;
                Body.Angle = (float) Math.Atan2(Physics.Velocity.X, -Physics.Velocity.Y);
                _se.Emit(1);
                if (Body.Position.Y > 510)
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

                    _explodesound.Play();

                    Collision.Update();
                    Collision.NewPartners = new List<Entity>();
                    Collision.Partners = new List<Entity>();
                }
            }
        }

        public void Destroy()
        {
            Destroy(null);
        }

        public void CollisionHandler(Entity e)
        {
            e.Health.Hurt(Damage);
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

    class SmokeEmitter : Emitter
    {
        Random _rand = new Random(DateTime.Now.Millisecond);
        private List<Color> Colors;
 
        public SmokeEmitter(Entity e)
            : base(e, e.StateRef.GameRef.Game.Content.Load<Texture2D>(@"game/explosionparticle"), Vector2.One * 5)
        {
            Colors = new List<Color>();
            Colors.Add(Color.Gray);
            Colors.Add(Color.DarkGray);
            Colors.Add(Color.LightGray);
            Colors.Add(Color.LightSlateGray);
            Colors.Add(Color.SlateGray);
        }

        protected override Particle GenerateNewParticle()
        {
            int index = _rand.Next(0, 3);
            int ttl = _rand.Next(40, 80);
            //Rotate the point based on the center of the sprite
            // p = unrotated point, o = rotation origin
            //p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
            //p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy

            var origin = Entity.Body.Position + Entity.Render.Origin * Entity.Render.Scale; 

            var unrotatedposition = new Vector2(
                Entity.Body.BoundingBox.X + Entity.Render.Origin.X * Entity.Render.Scale,
                Entity.Body.BoundingBox.Bottom);
            var angle = Entity.Body.Angle;

            var position = new Vector2(
                (float)(Math.Cos(angle) * (unrotatedposition.X - origin.X) - Math.Sin(angle) * (unrotatedposition.Y - origin.Y) + origin.X),
                (float)(Math.Sin(angle) * (unrotatedposition.X - origin.X) + Math.Cos(angle) * (unrotatedposition.Y - origin.Y) + origin.Y)
            );

            FadeParticle p = new FadeParticle(index, position, 10, ttl, this);
            p.Body.Angle = (float)_rand.NextDouble() / 2 - .25f;
            p.Physics.Thrust((float)_rand.NextDouble() + .1f);
            p.Render.Layer = .5f;
            p.Render.Color = Colors[_rand.Next(0, Colors.Count)];
            return p;
        }
    }
}
