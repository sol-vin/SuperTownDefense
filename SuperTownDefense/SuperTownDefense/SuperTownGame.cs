using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperTownDefense
{
    public class SuperTownGame : EntityGame
    {
        public readonly PausedState PausedState;
        public readonly GameState GameState;
        public  readonly MenuState MenuState;

        public SuperTownGame(Game game, GraphicsDeviceManager g, SpriteBatch sb)
            : base(game, g, new Rectangle(0, 0, 600, 600), sb)
        {
            //PauseState = new PauseState(this);
            GameState = new GameState(this);
            MenuState = new MenuState(this);
            MenuState.Show();
        }
    }
}
