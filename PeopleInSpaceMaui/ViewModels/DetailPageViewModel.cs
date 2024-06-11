using PeopleInSpaceMaui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PeopleInSpaceMaui.ViewModels;

public class DetailPageViewModel()
    : ReactiveObject, IQueryAttributable
{
    [Reactive] 
    public CrewModel? CrewMember { get; set; }
    
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("name") && query.ContainsKey("image") && query.ContainsKey("wikipedia"))
        {
            CrewMember = new CrewModel
            {
                Name = Uri.UnescapeDataString(query["name"].ToString()),
                Image = new Uri(Uri.UnescapeDataString(query["image"].ToString())),
                Wikipedia = new Uri(Uri.UnescapeDataString(query["wikipedia"].ToString()))
            };
        }
    }
}