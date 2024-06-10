namespace PeopleInSpaceMaui.Apis;

using Refit;
using System.Threading.Tasks;

public interface ISpaceXApi
{        
    [Get("/crew")]
    Task<string> GetAllCrew();
}