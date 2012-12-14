using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components;
using EntityEngine.Data;
using EntityEngine.Engine;
using EntityEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperTownDefense.Objects.Components;

namespace SuperTownDefense.Objects
{
    public class Town : Entity
    {
        private DoubleInput _firekey = new DoubleInput(Keys.Enter, Buttons.A, PlayerIndex.One);
        public Gun Gun;
        public Animation DeadCityAnim;
        public TileRender TileRender;
        public Cursor Cursor;

        private Sound _fire;

        public Town(EntityState es) : base(es)
        {
            Body = new Body(this, Vector2.Zero, new Vector2(20,15));
            Components.Add(Body);

            TileRender = new TileRender(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/town"), new Vector2(20, 15));
            Render = TileRender;
            Render.Scale = 6f;
            Render.Layer = 1f;
            Components.Add(Render);

            Body.Position = new Vector2(StateRef.GameRef.Viewport.Width/2 - Render.DrawRect.Width / 2, 450);
            DeadCityAnim = new Animation(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/deadcity"), new Vector2(20, 15), 4, "deadtown");
            DeadCityAnim.Layer = 1f;
            DeadCityAnim.Scale = 6.0f;

            Health = new Health(this, 100);
            Health.HurtEvent += ChangeColor;

            TileRender.Index = 2;
            Components.Add(Health);

            Gun = new Gun(this);
            Components.Add(Gun);

            Cursor = new Cursor(StateRef, this);
            StateRef.AddEntity(Cursor);

            Collision = new Collision(this);
            Components.Add(Collision);

            _fire = new Sound(this, es.GameRef.Game.Content.Load<SoundEffect>(@"game/sounds/bombfire"));
            Components.Add(_fire);
        }

        public override void Update()
        {
            base.Update();
            if (Health.Alive)
            {
                switch ((int)Health.HitPoints)
                {
                    case 70:
                        TileRender.Index = 1;
                        break;
                    case 30:
                        TileRender.Index = 0;
                        break;
                }
            }
            if (Health.HitPoints <= 0 && Render != DeadCityAnim)
            {
                Components.Remove(Render);
                Render = DeadCityAnim;
                DeadCityAnim.Start();
                Components.Add(Render);
            }

            if (_firekey.RapidFire(1250) && Health.Alive && Cursor.Render.Color != Color.Red)
            {
                Gun.Fire(Cursor.Angle);
                _fire.Play();
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            Render.Color = Color.White;
        }

        public void ChangeColor(Entity e = null)
        {
            Render.Color = Color.Red;
        }

    }
}
