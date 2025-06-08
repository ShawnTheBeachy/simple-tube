using System.Net.Mime;

namespace SimpleTube.RestApi.Infrastructure.Images;

internal static class ImageTypeHelpers
{
    // From https://stackoverflow.com/questions/9354747/how-can-i-determine-if-a-file-is-an-image-file-in-net
    public static string? GetImageTypeFromHeader(this ReadOnlyMemory<byte> buffer)
    {
        const int mostBytesNeeded = 11; //For JPEG

        if (buffer.Length < mostBytesNeeded)
            return null;

        var headerBuffer = buffer[..mostBytesNeeded].Span;

        //Sources:
        //http://stackoverflow.com/questions/9354747
        //http://en.wikipedia.org/wiki/Magic_number_%28programming%29#Magic_numbers_in_files
        //http://www.mikekunz.com/image_file_header.html

        if (headerBuffer.IsJpeg())
            return MediaTypeNames.Image.Jpeg;

        if (headerBuffer.IsPng())
            return MediaTypeNames.Image.Png;

        if (headerBuffer.IsGif())
            return MediaTypeNames.Image.Gif;

        if (headerBuffer.IsBmp())
            return MediaTypeNames.Image.Bmp;

        if (headerBuffer.IsTiff())
            return MediaTypeNames.Image.Tiff;

        return null;
    }

    private static bool IsBmp(this ReadOnlySpan<byte> headerBuffer) =>
        headerBuffer[0] == 0x42
        && //42 4D
        headerBuffer[1] == 0x4D;

    private static bool IsGif(this ReadOnlySpan<byte> headerBuffer) =>
        headerBuffer[0] == 0x47
        && //'GIF'
        headerBuffer[1] == 0x49
        && headerBuffer[2] == 0x46;

    private static bool IsJpeg(this ReadOnlySpan<byte> headerBuffer) =>
        headerBuffer[0] == 0xFF
        && //FF D8
        headerBuffer[1] == 0xD8
        && (
            (
                headerBuffer[6] == 0x4A
                && //'JFIF'
                headerBuffer[7] == 0x46
                && headerBuffer[8] == 0x49
                && headerBuffer[9] == 0x46
            )
            || (
                headerBuffer[6] == 0x45
                && //'EXIF'
                headerBuffer[7] == 0x78
                && headerBuffer[8] == 0x69
                && headerBuffer[9] == 0x66
            )
        )
        && headerBuffer[10] == 00;

    private static bool IsPng(this ReadOnlySpan<byte> headerBuffer) =>
        headerBuffer[0] == 0x89
        && //89 50 4E 47 0D 0A 1A 0A
        headerBuffer[1] == 0x50
        && headerBuffer[2] == 0x4E
        && headerBuffer[3] == 0x47
        && headerBuffer[4] == 0x0D
        && headerBuffer[5] == 0x0A
        && headerBuffer[6] == 0x1A
        && headerBuffer[7] == 0x0A;

    private static bool IsTiff(this ReadOnlySpan<byte> headerBuffer) =>
        (
            headerBuffer[0] == 0x49
            && //49 49 2A 00
            headerBuffer[1] == 0x49
            && headerBuffer[2] == 0x2A
            && headerBuffer[3] == 0x00
        )
        || (
            headerBuffer[0] == 0x4D
            && //4D 4D 00 2A
            headerBuffer[1] == 0x4D
            && headerBuffer[2] == 0x00
            && headerBuffer[3] == 0x2A
        );
}
