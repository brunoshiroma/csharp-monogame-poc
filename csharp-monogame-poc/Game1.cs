using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameCore;
using System.Linq;

namespace csharp_monogame_poc;

public class Game1 : Core
{
    private Texture2D nave;

    private Vector2 spritePosition = Vector2.Zero;
    protected float spriteScale = 0.5f;

    public Game1() : base("Galaxia", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        nave = Content.Load<Texture2D>("images/nave_01");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || Keyboard.GetState().GetPressedKeys().Contains(Keys.Up))
        {
            if (spriteScale <= 1f)
            {
                spriteScale += 0.1f;

            }
        }
        if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || Keyboard.GetState().GetPressedKeys().Contains(Keys.Down))
        {
            if (spriteScale >= 0f)
            {
                spriteScale -= 0.1f;

            }
        }
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.MonoGameOrange);

        // TODO: Add your drawing code here

        base.Draw(gameTime);


        SpriteBatch.Begin();
        SpriteBatch.Draw(nave, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
        SpriteBatch.End();
    }
}
