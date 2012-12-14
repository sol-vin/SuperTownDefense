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
using SuperTownDefense.Objects;

namespace SuperTownDefense
{
    public class Soldier : Enemy
    {
        private Random _rand = new Random(DateTime.Now.Millisecond);
        private Animation _soldieranim;
        private GibEmitter _ge;
        public bool IsAttacking { get; protected set; }
        private Timer _attacktimer;
        private Sound _hitsound;
        private Sound _attacksound;

        public Soldier(EntityState es) : base(es)
        {
            Points = 10;

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

            Health = new Health(this, 1);
            Health.DiedEvent += OnDeath;
            Components.Add(Health);

            _attacktimer = new Timer(this);
            _attacktimer.Milliseconds = 500;
            _attacktimer.LastEvent += OnAttackTimer;
            Components.Add(_attacktimer);

            _ge = new GibEmitter(this);
            Components.Add(_ge);

            _hitsound = new Sound(this, StateRef.GameRef.Game.Content.Load<SoundEffect>(@"game/sounds/hit"));
            Components.Add(_hitsound);

            _attacksound = new Sound(this, StateRef.GameRef.Game.Content.Load<SoundEffect>(@"game/sounds/shoot"));
            _attacksound.Volume = .3f;
            Components.Add(_attacksound);
        }

        public override void Update()
        {
            base.Update();
            //Stop the soldier at a point around the city.
            if(!IsAttacking)
            {
                float leftstop = StateRef.GameRef.Viewport.Width/2 - 60 - _rand.Next(0, 40);
                float rightstop = StateRef.GameRef.Viewport.Width / 2 + 60 + _rand.Next(0, 40);

                //If we are facing left
                if (Render.Flip == SpriteEffects.None)
                {
                    if (Body.Position.X < rightstop)
                    {
                        Physics.Velocity = Vector2.Zero;
                        IsAttacking = true;
                    }
                    
                }
                    //If we are facing right
                else
                {
                    if (Body.Position.X > leftstop)
                    {
                        Physics.Velocity = Vector2.Zero;
                        IsAttacking = true;
                    }
                }
                if (IsAttacking)
                {
                    _attacktimer.Start();
                }
            }
        }

        public void OnAttackTimer()
        {
            foreach (var enemy in Enemies)
            {
                if (enemy.Health.Alive)
                {
                    enemy.Health.Hurt(1);
                    _attacksound.Pitch = (float) _rand.NextDouble()*_rand.Next(-1, 2);
                    _attacksound.Play();
                }
            }
            
        }

        public void OnDeath(Entity e = null)
        {
            KilledByPlayer = true;
            _ge.Emit(20);
            _hitsound.Play();
            Destroy();
        }


    }

    class GibEmitter : Emitter
    {
        private List<Color> colors;

        Random _rand = new Random(DateTime.Now.Millisecond);
        public GibEmitter(Entity e) : base(e, e.StateRef.GameRef.Game.Content.Load<Texture2D>(@"game/soldiergibs"), Vector2.One)
        {
            colors = new List<Color>
                {
                    new Color(0xCC, 0x29, 0x29),
                    new Color(0xED, 0x6B, 0x6B),
                    Color.Red,
                    Color.DarkRed
                };
        }
        

        protected override Particle GenerateNewParticle()
        {
            int index = 0;
            int ttl = _rand.Next(50, 80);

            var p = new ExplosionParticle(index, Entity.Body.Position + Entity.Render.Origin * Entity.Render.Scale, ttl, this);
            p.Body.Angle = (float)_rand.NextDouble() - .5f*1.5f;
            p.Physics.Thrust(((float)_rand.NextDouble() + 1f) * 2.5f);
            p.Render.Layer = .5f;
            p.Render.Scale = 2f;
            p.Render.Color = colors[_rand.Next(0, colors.Count)];
            return p;
        }
    }
}
