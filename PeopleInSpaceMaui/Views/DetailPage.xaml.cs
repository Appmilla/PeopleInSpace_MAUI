using PeopleInSpaceMaui.ViewModels;

namespace PeopleInSpaceMaui.Views;

public partial class DetailPage : ReactiveUI.Maui.ReactiveContentPage<DetailPageViewModel>
{
    public DetailPage(DetailPageViewModel viewModel)
    {
        ViewModel = viewModel;
        
        InitializeComponent();
    }
}