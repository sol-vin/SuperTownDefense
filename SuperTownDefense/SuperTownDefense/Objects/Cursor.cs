using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components;
using EntityEngine.Engine;
using EntityEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperTownDefense.Objects
{
    public class Cursor : Entity
    {
        public Town Town;
        private DoubleInput _aimleftkey;
        private DoubleInput _aimrightkey;
        private DoubleInput _quickaimkey;

        public float Angle;

        private const float AngleConstrant = .2f;

        public Cursor(EntityState es, Town town) : base(es)
        {
            Town = town;

            _aimleftkey = new DoubleInput(Keys.A, Buttons.DPadLeft, PlayerIndex.One);
            _aimrightkey = new DoubleInput(Keys.D, Buttons.DPadRight, PlayerIndex.One);
            _quickaimkey = new DoubleInput(Keys.W, Buttons.RightShoulder, PlayerIndex.One);

            Physics = new Physics(this);
            Components.Add(Physics);

            Render = new Render(this, StateRef.GameRef.Game.Content.Load<Texture2D>(@"game/cursor"))
                {
                    Layer = 1f,
                    Scale = 6f
                };
            Components.Add(Render);

            Body = new Body(this, town.Body.Position + (town.Render.Origin - Vector2.UnitY * 40 - Render.Origin) * Render.Scale, Vector2.One * 5);
            Components.Add(Body);
        }

        public override void Update()
        {
            base.Update();
            float anglespeedmodifer = _quickaimkey.Down() ? 2 : 1;

            if (_aimleftkey.Down())
                Angle -= .001f * anglespeedmodifer;
            else if (_aimrightkey.Down())
                Angle += .001f * anglespeedmodifer;

            if (Angle > AngleConstrant)
                Angle = AngleConstrant;
            else if (Angle < -AngleConstrant)
                Angle = -AngleConstrant;

            if (Math.Abs(Angle) <= 0.055)
                Render.Color = Color.Red;
            else
                Render.Color = Color.White;
            

            var origin = Town.Body.Position + Town.Render.Origin * Town.Render.Scale;
            var unrotatedposition = Town.Body.Position + (Town.Render.Origin - Vector2.UnitY * 40 - Render.Origin) * Render.Scale;
            
            Body.Position = new Vector2(
                (float)(Math.Cos(Angle) * (unrotatedposition.X - origin.X) - Math.Sin(Angle) * (unrotatedposition.Y - origin.Y) + origin.X),
                (float)(Math.Sin(Angle) * (unrotatedposition.X - origin.X) + Math.Cos(Angle) * (unrotatedposition.Y - origin.Y) + origin.Y)
            );
        }
    }
}
