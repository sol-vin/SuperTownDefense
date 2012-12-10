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
using SuperTownDefense.Objects;

namespace SuperTownDefense
{
    public class Soldier : Entity
    {
        Random _rand = new Random(DateTime.Now.Millisecond);
        private Animation _soldieranim;
        private GibEmitter _ge;
        public Soldier(EntityState es) : base(es)
        {
            Body = new Body(this, Vector2.Zero);
            Components.Add(Body);
            _soldieranim = new Animation(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/soldier"), new Vector2(5, 10), 4,
                                   "soldier");
            Render = _soldieranim;
            Render.Flip = (_rand.RandomBool()) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Render.Layer = .5f;
            _soldieranim.Start();
            Components.Add(Render);

            Body.Position.Y = 520 -  _rand.Next(-10,10);
            Body.Position.X = (Render.Flip == SpriteEffects.None) ? es.GameRef.Viewport.Right + 10 : -10;

            Collision = new Collision(this);
            Components.Add(Collision);

            Physics = new Physics(this);
            Physics.Velocity.X = (Render.Flip == SpriteEffects.None) ? -.25f : .25f;
            Components.Add(Physics);

            _ge = new GibEmitter(this);
            Components.Add(_ge);
        }

        public override void Update()
        {
            base.Update();
            //Stop the soldier at a point around the city.
            float leftstop = StateRef.GameRef.Viewport.Width/2 - 60 - _rand.Next(0, 40);
            float rightstop = StateRef.GameRef.Viewport.Width / 2 + 60 + _rand.Next(0, 40);

            //If we are facing left
            if (Render.Flip == SpriteEffects.None)
            {
                if (Body.Position.X < rightstop)
                {
                    Physics.Velocity = Vector2.Zero;
                }
            }
            //If we are facing right
            else
            {
                if (Body.Position.X > leftstop)
                {
                    Physics.Velocity = Vector2.Zero;
                }
            }
        }

        public override void Destroy(Entity e = null)
        {
            base.Destroy(e);
            _ge.Emit(20);
        }


    }

    class GibEmitter : Emitter
    {
        Random _rand = new Random(DateTime.Now.Millisecond);
        public GibEmitter(Entity e) : base(e, e.StateRef.GameRef.Game.Content.Load<Texture2D>(@"game/soldiergibs"), Vector2.One * 3)
        {
        }
        

        protected override Particle GenerateNewParticle()
        {
            int index = _rand.Next(0, 4);
            int ttl = _rand.Next(50, 80);

            ExplosionParticle p = new ExplosionParticle(index, Entity.Body.Position + Entity.Render.Origin * Entity.Render.Scale, ttl, this);
            p.Body.Angle = (float)_rand.NextDouble() - .5f*1.5f;
            p.Physics.Thrust(((float)_rand.NextDouble() + 1f) * 2.5f);
            p.Render.Layer = .5f;
            p.Render.Scale = 2f;
            return p;
        }
    }
}
