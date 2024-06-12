namespace PeopleInSpaceMaui.Navigation;

public interface INavigationService
{
    Task NavigateAsync(string route);
}

public class NavigationService : INavigationService
{
    public async Task NavigateAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }
}