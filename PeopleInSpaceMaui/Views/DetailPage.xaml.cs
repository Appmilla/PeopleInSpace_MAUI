using PeopleInSpaceMaui.ViewModels;
using ReactiveUI;

namespace PeopleInSpaceMaui.Views;

public partial class DetailPage : ReactiveUI.Maui.ReactiveContentPage<DetailPageViewModel>
{
    public DetailPage(DetailPageViewModel viewModel)
    {
        BindingContext = viewModel;
        ViewModel = viewModel;
        
        InitializeComponent();
        
        this.WhenActivated(disposables =>
        {
        });
    }
}