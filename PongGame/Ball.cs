using System;
using Microsoft.Xna.Framework;

namespace PongGame;

/// <summary>
/// Tracks ball position, velocity, and collision response.
/// </summary>
public class Ball
{
    public const int Size = 14;

    public Vector2 Position;
    public Vector2 Velocity;

    private readonly int _screenWidth;
    private readonly int _screenHeight;

    public Ball(int screenWidth, int screenHeight)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        Reset(1);
    }

    /// <summary>Resets ball to centre with a serve direction. direction: +1 = right, -1 = left.</summary>
    public void Reset(int direction)
    {
        Position = new Vector2(_screenWidth / 2f - Size / 2f, _screenHeight / 2f - Size / 2f);
        float angle = MathHelper.ToRadians(30f);
        Velocity = new Vector2(direction * 300f * MathF.Cos(angle), 300f * MathF.Sin(angle));
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;

        // Bounce off top/bottom walls
        if (Position.Y <= 0)
        {
            Position.Y = 0;
            Velocity.Y = MathF.Abs(Velocity.Y);
        }
        else if (Position.Y + Size >= _screenHeight)
        {
            Position.Y = _screenHeight - Size;
            Velocity.Y = -MathF.Abs(Velocity.Y);
        }
    }

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size, Size);

    /// <summary>
    /// Reflects the ball off a paddle. Adjusts Y velocity based on where the ball
    /// hits the paddle face to give the player some control over angle.
    /// </summary>
    public void BounceOffPaddle(Rectangle paddle, bool isLeftPaddle)
    {
        float paddleCentre = paddle.Y + paddle.Height / 2f;
        float ballCentre = Position.Y + Size / 2f;
        float relativeIntersect = (ballCentre - paddleCentre) / (paddle.Height / 2f); // -1..1

        float bounceAngle = relativeIntersect * MathHelper.ToRadians(60f);
        float speed = Velocity.Length() * 1.05f; // slight speed increase per hit
        speed = MathF.Min(speed, 700f);           // cap speed

        Velocity.X = (isLeftPaddle ? 1 : -1) * speed * MathF.Cos(bounceAngle);
        Velocity.Y = speed * MathF.Sin(bounceAngle);

        // Ensure ball is outside the paddle rect to prevent tunnelling
        if (isLeftPaddle)
            Position.X = paddle.Right;
        else
            Position.X = paddle.Left - Size;
    }
}
