using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace OvulaeApp.Helpers.UI
{
    public class GifLoaderView : SKCanvasView
    {
        private SKCodec codec;
        private int currentFrame = 0;
        private SKBitmap bitmap;
        private bool isAnimating = true;
        private int frameDelay = 100; // fallback delay in ms
        private DateTime lastFrameTime = DateTime.MinValue;

        public GifLoaderView()
        {
            EnableTouchEvents = false;
            PaintSurface += OnPaintSurface;
            StartAnimation();
        }

        private void StartAnimation()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
            {
                if (!isAnimating)
                    return false;

                InvalidateSurface();
                return true;
            });
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            if (codec == null)
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("Resources/Images/app_loader.gif").Result;
                codec = SKCodec.Create(stream);

                if (codec == null || codec.FrameCount == 0)
                {
                    Console.WriteLine("Failed to load GIF or GIF has zero frames.");
                    return;
                }

                bitmap = new SKBitmap(codec.Info.Width, codec.Info.Height);
            }

            if (codec.FrameCount == 0)
            {
                Console.WriteLine("GIF FrameCount is zero, skipping drawing.");
                return;
            }

            // Handle frame delays
            var frameInfo = codec.FrameInfo[currentFrame];
            var now = DateTime.Now;
            if ((now - lastFrameTime).TotalMilliseconds < frameInfo.Duration)
                return; // skip until next frame time

            lastFrameTime = now;

            var options = new SKCodecOptions(currentFrame);
            codec.GetPixels(bitmap.Info, bitmap.GetPixels(), options);

            canvas.DrawBitmap(bitmap, new SKRect(0, 0, e.Info.Width, e.Info.Height));

            currentFrame = (currentFrame + 1) % codec.FrameCount;
        }

        public void StopAnimation()
        {
            isAnimating = false;
        }

        public void StartAnimationManually()
        {
            if (!isAnimating)
            {
                isAnimating = true;
                StartAnimation();
            }
        }
    }
}