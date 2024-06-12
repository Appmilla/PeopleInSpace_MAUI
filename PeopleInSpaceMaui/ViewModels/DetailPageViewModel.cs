using PeopleInSpaceMaui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PeopleInSpaceMaui.ViewModels;

public class DetailPageViewModel : ReactiveObject, IQueryAttributable
{
    [Reactive]
    public string PageTitle { get; set; } = "Biography";

    [Reactive] 
    public CrewModel? CrewMember { get; private set; }
    
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var name = GetQueryValue(query, "name");
        var image = GetQueryValue(query, "image");
        var wikipedia = GetQueryValue(query, "wikipedia");

        if (!string.IsNullOrEmpty(name) && Uri.TryCreate(image, UriKind.Absolute, out var imageUri) && Uri.TryCreate(wikipedia, UriKind.Absolute, out var wikiUri))
        {
            CrewMember = new CrewModel
            {
                Name = name,
                Image = imageUri,
                Wikipedia = wikiUri
            };
        }
        else
        {
            // Handle the case where one or more parameters are invalid
            throw new ArgumentException("Invalid or missing query parameters.");
        }
    }

    private static string GetQueryValue(IDictionary<string, object> query, string key)
    {
        if (query.TryGetValue(key, out var value) && value is string stringValue)
        {
            return Uri.UnescapeDataString(stringValue);
        }
        return string.Empty;
    }
}