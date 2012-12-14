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
    public class Helicopter : Enemy
    {
        private Random _rand  = new Random(DateTime.Now.Millisecond);
        private Animation _helicopteranim;
        private HeliGibEmitter _hge;
        private Animation _explodeanim;
        private bool _hasdroppedbomb;

        public Helicopter(EntityState es) : base(es)
        {
            Points = 50;

            Body = new Body(this, Vector2.Zero);
            Components.Add(Body);

            _helicopteranim = new Animation(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/helicopter"), new Vector2(20, 16), 30,
                                   "helicopter");
            Render = _helicopteranim;
            Render.Flip = (_rand.RandomBool()) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Render.Layer = .5f;
            _helicopteranim.Start();
            Components.Add(Render);

            Body.Position.Y = 300 - _rand.Next(-10, 10);
            Body.Position.X = (Render.Flip == SpriteEffects.None) ? es.GameRef.Viewport.Right + 10 : -10;

            Collision = new Collision(this);
            Components.Add(Collision);

            Physics = new Physics(this);
            Physics.Velocity.X = (Render.Flip == SpriteEffects.None) ? -.4f : .4f;
            Components.Add(Physics);

            Health = new Health(this, 1);
            Health.DiedEvent += OnDeath;
            Components.Add(Health);

            _hge = new HeliGibEmitter(this);
            Components.Add(_hge);

            _explodeanim = new Animation(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/explosion"),
                new Vector2(16, 16), 30, "explode");
            _explodeanim.Layer = 0.2f;
            _explodeanim.Scale = 1.5f;
            _explodeanim.LastFrameEvent += Destroy;
        }

        public override void Update()
        {
            base.Update();
            if (!_hasdroppedbomb && Body.BoundingBox.Right > StateRef.GameRef.Viewport.Width/2 - 25 &&
                Body.BoundingBox.Left < StateRef.GameRef.Viewport.Width/2 + 25)
            {
                _hasdroppedbomb = true;
                Bomb b = new Bomb(StateRef, Body.Position + Render.Origin * Render.Scale, MathHelper.Pi, 0);
                b.Collision.NewPartners = Enemies;
                b.Damage = .5f;
                AddEntity(b);
            }
            //Destroy when leaves screen.
        }

        public void OnDeath(Entity e = null)
        {
            KilledByPlayer = true;
            _hge.Emit(40);
            Components.Remove(Render);
            Render = _explodeanim;
            Components.Add(Render);
            Body.Position -= Render.Origin * Render.Scale;

            _explodeanim.Start();
        }

        public void Destroy()
        {
            Destroy(null);
        }
    }

    class HeliGibEmitter : Emitter
    {
        Random _rand = new Random(DateTime.Now.Millisecond);
        private List<Color> colors;

        public HeliGibEmitter(Entity e)
            : base(e, e.StateRef.GameRef.Game.Content.Load<Texture2D>(@"game/soldiergibs"), Vector2.One)
        {
            colors = new List<Color>
                {
                    Color.Gray,
                    Color.SlateGray,
                    Color.DarkGray,
                    Color.Brown
                };
        }


        protected override Particle GenerateNewParticle()
        {
            int index = 0;
            int ttl = _rand.Next(50, 80);

            ExplosionParticle p = new ExplosionParticle(index, Entity.Body.Position + Entity.Render.Origin * Entity.Render.Scale, ttl, this);
            p.Body.Angle = (float)_rand.NextDouble() * MathHelper.TwoPi;
            p.Physics.Thrust(((float)_rand.NextDouble() + 1f) * 2.5f);
            p.Render.Layer = .5f;
            p.Render.Scale = 2f;
            p.Render.Color = colors[_rand.Next(0, colors.Count)];
            return p;
        }
    }
}
