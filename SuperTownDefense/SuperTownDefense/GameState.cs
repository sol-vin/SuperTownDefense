using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Engine;
using EntityEngine.GUI;
using EntityEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperTownDefense.Objects;

namespace SuperTownDefense
{
    public class GameState : EntityState
    {
        private Image _bgimage;
        private Town _town;
        private bool _alreadystarted;
        private Soldier _s;
        private DoubleInput _restartkey = new DoubleInput(Keys.Escape, Buttons.Start, PlayerIndex.One);
        private EnemySpawner _es;
        public GameState(SuperTownGame game) : base(game, "game")
        {
            
        }

        public new void Start()
        {
            if (_alreadystarted) return;

            _alreadystarted = true;

            GameRef.BGColor = Color.BlueViolet;
            _bgimage = new Image(this, GameRef.Game.Content.Load<Texture2D>(@"game/background"), Vector2.Zero)
                {
                    Render = {Scale = 6f, Layer = 0f}
                };
            AddEntity(_bgimage);

            _town = new Town(this);
            AddEntity(_town);

            _es = new EnemySpawner(this);
            AddEntity(_es);

            _town.Collision.NewPartners = _es.Enemies;
        }

        public override void Reset()
        {
            base.Reset();
            _alreadystarted = false;
        }

        public override void Show(string tag)
        {
            base.Show(tag);
            Start();
        }

        public override void Update()
        {
            base.Update();
            if (_restartkey.Pressed())
            {
                Reset();
                Start();
            }
        }
    }
}
