using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Engine;
using EntityEngine.GUI;
using EntityEngine.Input;
using EntityEngine.Components;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperTownDefense
{
    public class MenuState : EntityState
    {
        private DoubleInput _startkey;
        private Text _starttext;
        private Image _bgimage;

        public MenuState(SuperTownGame game) : base(game, "menu")
        {
            ChangeState += game.GameState.Show;
            Start();
        }

        public new void Start()
        {
            _startkey = new DoubleInput(Keys.Enter, Buttons.Start, PlayerIndex.One);
            GameRef.BGColor = Color.Red;



            _bgimage = new Image(this, GameRef.Game.Content.Load<Texture2D>(@"menu/background"), Vector2.Zero)
            {
                Render = { Scale = 6f, Layer = 0f }
            };
            AddEntity(_bgimage);

            _starttext = new Text(this, new Vector2(100, 500), "Press Start or Enter to start",
                                 GameRef.Game.Content.Load<SpriteFont>("font")) { Render = { Layer = 1f } };
            AddEntity(_starttext);
        }
        

        public override void Update()
        {
            base.Update();
            if (_startkey.Pressed())
            {
                ChangeToState("game");
            }
        }
    }
}
