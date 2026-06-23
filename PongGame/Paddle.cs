using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PongGame;

public enum PaddleSide { Left, Right }

/// <summary>
/// Player-controlled or AI-controlled paddle.
/// </summary>
public class Paddle
{
    public const int Width = 14;
    public const int Height = 90;
    private const float Speed = 420f;
    private const int Margin = 20;

    public Vector2 Position;

    private readonly PaddleSide _side;
    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private readonly bool _isAI;

    public Paddle(PaddleSide side, int screenWidth, int screenHeight, bool isAI = false)
    {
        _side = side;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _isAI = isAI;

        float x = side == PaddleSide.Left ? Margin : screenWidth - Margin - Width;
        Position = new Vector2(x, screenHeight / 2f - Height / 2f);
    }

    public void Update(GameTime gameTime, Ball ball)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_isAI)
            UpdateAI(dt, ball);
        else
            UpdatePlayer(dt);

        // Clamp to screen
        Position.Y = MathHelper.Clamp(Position.Y, 0, _screenHeight - Height);
    }

    private void UpdatePlayer(float dt)
    {
        var kb = Keyboard.GetState();

        if (_side == PaddleSide.Left)
        {
            if (kb.IsKeyDown(Keys.W)) Position.Y -= Speed * dt;
            if (kb.IsKeyDown(Keys.S)) Position.Y += Speed * dt;
        }
        else
        {
            if (kb.IsKeyDown(Keys.Up))   Position.Y -= Speed * dt;
            if (kb.IsKeyDown(Keys.Down)) Position.Y += Speed * dt;
        }
    }

    private void UpdateAI(float dt, Ball ball)
    {
        // Simple AI: track ball centre with a speed cap
        float paddleCentre = Position.Y + Height / 2f;
        float ballCentre   = ball.Position.Y + Ball.Size / 2f;

        if (paddleCentre < ballCentre - 4)
            Position.Y += Speed * 0.85f * dt;
        else if (paddleCentre > ballCentre + 4)
            Position.Y -= Speed * 0.85f * dt;
    }

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);
}
