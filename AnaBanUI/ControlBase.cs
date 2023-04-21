using LiVerse.AnaBanUI.Containers;
using LiVerse.AnaBanUI.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Runtime.CompilerServices;

namespace LiVerse.AnaBanUI {
  public abstract class ControlBase {
    public ControlBase? ParentControl { get; set; }

    /// A disabled control doesn't accept user input, but its still rendered if visible
    public bool Enabled { get; set; } = true;
    /// A disabled control doesn't accept user input, but its still rendered if visible
    public bool DrawDebugLines { get; set; } = false;
    /// A invisible control is invisible and doesn't process any events
    public bool Visible { get; set; } = true;
    /// Backing field for ElementSize
    protected Vector2 _size { get; set; }

    /// <summary>
    /// Total Size, including padding/margins.<br></br>Don't change this value manually, this value should be determined by the container
    /// </summary>
    public Vector2 Size {
      get => _size;
      set {
        if (value == _size) {
          return;
        }

        _size = value;
        _abosoluteArea = new RectangleF(_absolutePosition, _size);
        ElementSizeChanged();
      }
    }

    public Vector2 ContentArea { get => new Vector2(Size.X - Margin.X * 2, Size.Y - Margin.Y * 2); }
    public Vector2 RenderOffset { get; set; } = Vector2.Zero;
    public Vector2 Margin { get; set; } = Vector2.Zero;

    Vector2 _minimumSize = Vector2.Zero;
    public Vector2 MinimumSize {
      get => _minimumSize;
      set {
        //if (value == _minimumSize) { return; }

        _minimumSize = new Vector2(value.X + Margin.X, value.Y + Margin.Y);
      }
    }

    public Vector2? MaximumSize { get; set; } = null;
    /// Absolute Position on Screen, Used for collision detection.
    Vector2 _absolutePosition = Vector2.Zero;
    public Vector2 AbsolutePosition {
      get => _absolutePosition;
      set {
        //if (value == _absolutePosition) { return; }

        _absolutePosition = value + Margin;
        _abosoluteArea = new RectangleF(_absolutePosition, _size);
      }
    }
    public Vector2 RelativePosition { get; set; }
    RectangleF _abosoluteArea = RectangleF.Empty;
    public RectangleF AbsoluteArea { get => _abosoluteArea; }

    Viewport oldViewport;

    protected virtual void ElementSizeChanged() { }

    /// <summary>
    /// Updates internal logic
    /// </summary>
    /// <param name="deltaTime"></param>
    public abstract void Update(double deltaTime);

    /// <summary>
    /// Called after drawing, updates Positions, Animations etc
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void UpdateUI(double deltaTime) { }

    /// <summary>
    /// Method called when the UIRoot decides this element should receive/process pointer events
    /// </summary>
    /// <returns>True if the event should be blocked</returns>
    public virtual bool InputUpdate(PointerEvent pointerEvent) { return false; }

    /// <summary>
    /// Method called when the UIRoot decides this element should receive/process keyboard events
    /// </summary>
    /// <returns>True if the event should be blocked</returns>
    public virtual bool InputUpdate(KeyboardEvent keyboardEvent) { return false; }

    /// <summary>
    /// Applies a translation matrix and calls <seealso cref="DrawElement"/>
    /// </summary>
    public virtual void Draw(SpriteBatch spriteBatch, double deltaTime) {
      if (!Visible) { return; }

      BeginDraw(spriteBatch);
      UpdateUI(deltaTime);
      DrawElement(spriteBatch, deltaTime);

      if (DrawDebugLines) {
        if (Margin != Vector2.Zero) {
          spriteBatch.DrawRectangle(new RectangleF(Vector2.Zero, ContentArea), Color.Magenta);
        }

        spriteBatch.End();
        spriteBatch.GraphicsDevice.Viewport = new Viewport((int)(AbsolutePosition.X - Margin.X), (int)(AbsolutePosition.Y - Margin.Y), (int)Size.X, (int)Size.Y);
        spriteBatch.Begin();

        spriteBatch.DrawRectangle(new RectangleF(Vector2.Zero, Size), Color.Blue);
      }

      EndDraw(spriteBatch);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginDraw(SpriteBatch spriteBatch) {
      oldViewport = spriteBatch.GraphicsDevice.Viewport;

      spriteBatch.End();
      Vector2 additionalOffset = ParentControl == null ? Vector2.Zero : ParentControl.RenderOffset;
      spriteBatch.GraphicsDevice.Viewport = new Viewport((int)(AbsolutePosition.X + RenderOffset.X + additionalOffset.X), (int)(AbsolutePosition.Y + RenderOffset.Y + additionalOffset.Y), (int)Size.X, (int)Size.Y);
      spriteBatch.Begin();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndDraw(SpriteBatch spriteBatch) {
      spriteBatch.End();
      spriteBatch.GraphicsDevice.Viewport = oldViewport;
      spriteBatch.Begin();
    }

    /// <summary>
    /// Make a element use all this control's space and also sets this as the elements parent control
    /// </summary>
    internal void FillElement(ControlBase element, bool overrideMinimumSize = true) {
      element.Size = ContentArea;
      element.RelativePosition = RelativePosition;
      element.AbsolutePosition = AbsolutePosition;
      element.ParentControl = this;

      if (overrideMinimumSize) MinimumSize = element.MinimumSize;
    }

    /// <summary>
    /// Draws the element without transformation
    /// </summary>
    public abstract void DrawElement(SpriteBatch spriteBatch, double deltaTime);
  }
}