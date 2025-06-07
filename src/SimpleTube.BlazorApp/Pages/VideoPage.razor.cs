using Microsoft.AspNetCore.Components;

namespace SimpleTube.BlazorApp.Pages;

public sealed partial class VideoPage
{
    [Parameter]
    public string VideoId { get; set; } = "";
}
