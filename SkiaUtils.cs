using SkiaSharp;

namespace DalleImageBase64;
public static class SkiaUtils {
           public static async Task<string> SaveImageToFile(string url, int width, int height, string filename = "image.png") {

        SKImageInfo info = new SKImageInfo(width, height);
        SKSurface surface = SKSurface.Create(info);
        SKCanvas canvas = surface.Canvas;
        canvas.Clear(SKColors.White);
        var httpClient = new HttpClient();
        using (Stream stream = await httpClient.GetStreamAsync(url))
        using (MemoryStream memStream = new MemoryStream()) {
            await stream.CopyToAsync(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            SKBitmap webBitmap = SKBitmap.Decode(memStream);
            canvas.DrawBitmap(webBitmap, 0, 0, null);
            surface.Draw(canvas, 0, 0, null);
        };
        surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo(new FileStream(filename, FileMode.Create));
        return filename;
    }
            public static async Task<string> GetImageToBase64String(string url, int width, int height) {

        SKImageInfo info = new SKImageInfo(width, height);
        SKSurface surface = SKSurface.Create(info);
        SKCanvas canvas = surface.Canvas;
        canvas.Clear(SKColors.White);
        var httpClient = new HttpClient();
        using (Stream stream = await httpClient.GetStreamAsync(url))
        using (MemoryStream memStream = new MemoryStream())  {
            await stream.CopyToAsync(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            SKBitmap webBitmap = SKBitmap.Decode(memStream);
            canvas.DrawBitmap(webBitmap, 0, 0, null);
            surface.Draw(canvas, 0, 0, null);
        };
        using (MemoryStream memStream = new MemoryStream()) {
            surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo(memStream);
            byte[] imageBytes = memStream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }
}