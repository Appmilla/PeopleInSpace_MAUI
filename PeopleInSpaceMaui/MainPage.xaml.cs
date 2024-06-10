using PeopleInSpaceMaui.ViewModels;
using ReactiveUI;

namespace PeopleInSpaceMaui;


public partial class MainPage : ReactiveUI.Maui.ReactiveContentPage<MainPageViewModel>
{
    public MainPage(MainPageViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}