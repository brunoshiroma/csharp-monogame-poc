using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameCore;

namespace PongGame;

/// <summary>
/// Two-player Pong. Left paddle: W/S. Right paddle: Up/Down.
/// First to 7 points wins. Press Enter to restart after a win.
/// </summary>
public class PongGame : Core
{
    private const int ScreenWidth  = 800;
    private const int ScreenHeight = 600;
    private const int WinScore     = 7;

    // Gameplay objects
    private Ball   _ball   = null!;
    private Paddle _left   = null!;
    private Paddle _right  = null!;

    // Scores
    private int _scoreLeft;
    private int _scoreRight;

    // Drawing primitives (generated at runtime — no asset files needed)
    private Texture2D _pixel = null!;
    private SpriteFont? _font;          // null if no font asset; falls back to no text

    private bool _gameOver;
    private string _winMessage = string.Empty;

    public PongGame() : base("Pong", ScreenWidth, ScreenHeight, false) { }

    protected override void Initialize()
    {
        base.Initialize();
        NewRound(1);
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        // 1×1 white pixel used to draw all rectangles
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        // Attempt to load a font if one is compiled into content; not required
        try { _font = Content.Load<SpriteFont>("fonts/pong"); }
        catch { _font = null; }
    }

    private void NewRound(int serveDirection)
    {
        _ball  = new Ball(ScreenWidth, ScreenHeight);
        _ball.Reset(serveDirection);
        _left  = new Paddle(PaddleSide.Left,  ScreenWidth, ScreenHeight);
        _right = new Paddle(PaddleSide.Right, ScreenWidth, ScreenHeight);
    }

    private void RestartGame()
    {
        _scoreLeft  = 0;
        _scoreRight = 0;
        _gameOver   = false;
        NewRound(1);
    }

    protected override void Update(GameTime gameTime)
    {
        var kb = Keyboard.GetState();
        if (kb.IsKeyDown(Keys.Escape))
            Exit();

        if (_gameOver)
        {
            if (kb.IsKeyDown(Keys.Enter))
                RestartGame();
            base.Update(gameTime);
            return;
        }

        _left.Update(gameTime, _ball);
        _right.Update(gameTime, _ball);
        _ball.Update(gameTime);

        HandleCollisions();
        CheckScoring();

        base.Update(gameTime);
    }

    private void HandleCollisions()
    {
        Rectangle ballBounds = _ball.Bounds;

        if (ballBounds.Intersects(_left.Bounds) && _ball.Velocity.X < 0)
            _ball.BounceOffPaddle(_left.Bounds, isLeftPaddle: true);

        if (ballBounds.Intersects(_right.Bounds) && _ball.Velocity.X > 0)
            _ball.BounceOffPaddle(_right.Bounds, isLeftPaddle: false);
    }

    private void CheckScoring()
    {
        if (_ball.Position.X + Ball.Size < 0)
        {
            _scoreRight++;
            CheckWin();
            if (!_gameOver) NewRound(1);
        }
        else if (_ball.Position.X > ScreenWidth)
        {
            _scoreLeft++;
            CheckWin();
            if (!_gameOver) NewRound(-1);
        }
    }

    private void CheckWin()
    {
        if (_scoreLeft >= WinScore)
        {
            _gameOver   = true;
            _winMessage = "Left Player Wins!";
        }
        else if (_scoreRight >= WinScore)
        {
            _gameOver   = true;
            _winMessage = "Right Player Wins!";
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        base.Draw(gameTime);

        SpriteBatch.Begin();

        DrawCentreNet();
        DrawRect(_left.Bounds,  Color.White);
        DrawRect(_right.Bounds, Color.White);
        DrawRect(_ball.Bounds,  Color.White);

        if (_font != null)
        {
            // Scores
            string leftScore  = _scoreLeft.ToString();
            string rightScore = _scoreRight.ToString();
            SpriteBatch.DrawString(_font, leftScore,  new Vector2(ScreenWidth / 4f,  20), Color.White);
            SpriteBatch.DrawString(_font, rightScore, new Vector2(3 * ScreenWidth / 4f, 20), Color.White);

            if (_gameOver)
            {
                Vector2 size   = _font.MeasureString(_winMessage);
                Vector2 origin = new Vector2(ScreenWidth / 2f - size.X / 2f, ScreenHeight / 2f - size.Y / 2f);
                SpriteBatch.DrawString(_font, _winMessage, origin, Color.Yellow);

                const string restart = "Press Enter to restart";
                Vector2 rSize = _font.MeasureString(restart);
                SpriteBatch.DrawString(_font, restart,
                    new Vector2(ScreenWidth / 2f - rSize.X / 2f, origin.Y + size.Y + 10),
                    Color.Gray);
            }
        }
        else
        {
            // Fallback score display using coloured rectangles (no font required)
            DrawScorePips(_scoreLeft,  ScreenWidth / 4,       40);
            DrawScorePips(_scoreRight, 3 * ScreenWidth / 4,   40);
        }

        SpriteBatch.End();
    }

    private void DrawCentreNet()
    {
        const int dashHeight = 12;
        const int dashGap    = 8;
        int x = ScreenWidth / 2 - 1;

        for (int y = 0; y < ScreenHeight; y += dashHeight + dashGap)
            DrawRect(new Rectangle(x, y, 2, dashHeight), new Color(80, 80, 80));
    }

    /// <summary>Draws score as a row of small square pips (no font needed).</summary>
    private void DrawScorePips(int score, int centreX, int y)
    {
        const int pip  = 8;
        const int gap  = 4;
        int totalWidth = WinScore * (pip + gap) - gap;
        int startX     = centreX - totalWidth / 2;

        for (int i = 0; i < WinScore; i++)
        {
            Color c = i < score ? Color.White : new Color(60, 60, 60);
            DrawRect(new Rectangle(startX + i * (pip + gap), y, pip, pip), c);
        }
    }

    private void DrawRect(Rectangle rect, Color color) =>
        SpriteBatch.Draw(_pixel, rect, color);
}
