using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components;
using EntityEngine.Data;
using EntityEngine.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperTownDefense.Objects
{
    public class Bomb : Entity
    {
        private const float Gravity = .1f;
        private Animation _explodeanim;
        private bool _isexploding;

        public Bomb(EntityState es, Vector2 position, float angle) : base(es)
        {
            Body = new Body(this, position, new Vector2(5,10)) {Angle = angle};
            Components.Add(Body);

            Physics = new Physics(this);
            Physics.Thrust(-8.5f);
            Components.Add(Physics);

            
            _explodeanim = new Animation(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/explosion"), 
                new Vector2(16,16), 18, "explode");
            _explodeanim.FrameTimer.LastEvent += _explodeanim.Stop;
            _explodeanim.Layer = 0.2f;
            _explodeanim.Scale = 2.0f;
            _explodeanim.LastFrameEvent += Destroy;

            Render = new Render(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/bomb"));
            Render.Layer = 0.1f;
            Render.Scale = 1.5f;
            Components.Add(Render);
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
                    Components.Add(_explodeanim);
                    _explodeanim.Start();
                }
            }
        }

        public void Destroy()
        {
            Destroy(null);
        }
    }
}
