using System;
using System.Collections.Generic;
using System.Globalization;
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
        private DoubleInput _restartkey = new DoubleInput(Keys.Escape, Buttons.Start, PlayerIndex.One);
        private EnemySpawner _es;

        private Text _scoretext;
        private Text _healthtext;
        private Text _gameovertext;

        public static int Score;
        public static bool GameOver { get; private set; }

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
            EntityRemoved += _es.RemoveEnemy;
            _es.Targets.Add(_town);
            AddEntity(_es);

            _scoretext = new Text(this, Vector2.Zero, Score.ToString(), GameRef.Game.Content.Load<SpriteFont>(@"font")); 
            _scoretext.Body.Position = new Vector2(GameRef.Viewport.Width/2 - _scoretext.Render.DrawRectangle.Width/2, 10);
            _scoretext.Render.Layer = 1f;
            AddEntity(_scoretext);

            _healthtext = new Text(this, Vector2.Zero, _town.Health.HitPoints.ToString(), GameRef.Game.Content.Load<SpriteFont>(@"font"));
            _healthtext.Body.Position = new Vector2(GameRef.Viewport.Width / 2 - _healthtext.Render.DrawRectangle.Width / 2, 50);
            _healthtext.Render.Layer = 1f;
            _healthtext.Render.Color = Color.Red;
            AddEntity(_healthtext);

            _gameovertext = new Text(this, Vector2.Zero, "", GameRef.Game.Content.Load<SpriteFont>(@"font"));
            _gameovertext.Render.Layer = 1f;
            _gameovertext.Render.Text = "Game Over!\nPress Start or Enter to retry!";
            _gameovertext.Render.Color = Color.Black;

            _town.Collision.NewPartners = _es.Enemies;
        }

        public override void Reset()
        {
            base.Reset();
            _alreadystarted = false;
            GameOver = false;
        }

        public override void Show(string tag)
        {
            base.Show(tag);
            Start();
        }

        public override void Update()
        {
            base.Update();
            _scoretext.Render.Text = Score.ToString();
            _scoretext.Body.Position = new Vector2(GameRef.Viewport.Width / 2 - _scoretext.Render.DrawRectangle.Width / 2, 10);

            _healthtext.Render.Text = ((int)_town.Health.HitPoints).ToString();
            _healthtext.Body.Position = new Vector2(GameRef.Viewport.Width / 2 - _healthtext.Render.DrawRectangle.Width / 2, 50);
            
            //Game over logic
            if (!_town.Health.Alive)
                GameOver = true;

            if (GameOver)
            {
                OnGameOver();

            }
        }

        private void OnGameOver()
        {
            AddEntity(_gameovertext);
            _gameovertext.Body.Position = new Vector2(GameRef.Viewport.Width / 2 - _gameovertext.Render.DrawRectangle.Width / 2,GameRef.Viewport.Height / 2 - _gameovertext.Render.DrawRectangle.Height / 2);

            if (_restartkey.Pressed())
            {
                Reset();
                Start();
            }
        }

        public override void RemoveEntity(Entity entity)
        {
            base.RemoveEntity(entity);
        }

        public void EnemyAdded(Enemy e)
        {
            e.Enemies.Add(_town);
        }
    }
}
