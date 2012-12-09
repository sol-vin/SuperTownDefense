using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components;
using EntityEngine.Data;
using EntityEngine.Engine;
using EntityEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperTownDefense.Objects.Components;

namespace SuperTownDefense.Objects
{
    public class Town : Entity
    {
        private DoubleInput _firekey = new DoubleInput(Keys.Enter, Buttons.A, PlayerIndex.One);

        public Gun Gun;

        public AnimationRender AnimationRender;
        public Animation DeadCityAnim;

        public TileRender TileRender;

        public Cursor Cursor;

        public Town(EntityState es) : base(es)
        {
            Body = new Body(this, Vector2.Zero, new Vector2(20,15));
            Components.Add(Body);

            DeadCityAnim = new Animation(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/deadcity"), new Vector2(20,15), 4, "deadtown");

            TileRender = new TileRender(this, es.GameRef.Game.Content.Load<Texture2D>(@"game/town"), new Vector2(20, 15));
            Render = TileRender;
            Render.Scale = 6f;
            Render.Layer = 1f;
            Components.Add(Render);

            Body.Position = new Vector2(StateRef.GameRef.Viewport.Width/2 - Body.BoundingBox.Width / 2, 450);

            Health = new Health(this, 3);
            TileRender.Index = Health.HitPoints - 1;
            Components.Add(Health);

            Gun = new Gun(this);
            Components.Add(Gun);

            Cursor = new Cursor(StateRef, this);
            StateRef.AddEntity(Cursor);
        }

        public override void Update()
        {
            base.Update();
            TileRender.Index = Health.HitPoints - 1;

            if (Cursor.Render.Color != Color.Red && _firekey.RapidFire(1500))
            {
                Gun.Fire(Cursor.Angle);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

    }
}
