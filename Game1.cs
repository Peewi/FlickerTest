using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FlickerTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        List<Vector2> positions = new List<Vector2>(100);
        bool mouseDown = false;
        ShapeDrawer shapeDrawer;

        public Game1()
        {
            var check = MonoGame.Framework.Utilities.PlatformInfo.MonoGamePlatform;
            _graphics = new GraphicsDeviceManager(this)
			{
                GraphicsProfile = GraphicsProfile.HiDef
            };
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            positions.Add(new Vector2(20));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            shapeDrawer = new ShapeDrawer(Content.Load<Effect>("Shapes"), _graphics, Window);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var mouseState = Mouse.GetState();
            bool newMouseDown = mouseState.LeftButton == ButtonState.Pressed;
            if (newMouseDown && !mouseDown)
            {
                positions.Add(mouseState.Position.ToVector2());
            }
            mouseDown = newMouseDown;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            shapeDrawer.Begin();
			foreach (var item in positions)
			{
                shapeDrawer.Draw(item, 32);
			}
            shapeDrawer.End();
            base.Draw(gameTime);
        }
    }
}
