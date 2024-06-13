using DynamicData;
using DynamicData.Binding;
using PeopleInSpaceMaui.Models;
using PeopleInSpaceMaui.Reactive;
using PeopleInSpaceMaui.Repositories;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PeopleInSpaceMaui.Extensions;
using PeopleInSpaceMaui.Navigation;

namespace PeopleInSpaceMaui.ViewModels;

public class MainPageViewModel : ReactiveObject, IActivatableViewModel
{
    readonly ISchedulerProvider _schedulerProvider;
    private readonly ICrewRepository _crewRepository;
    private readonly INavigationService _navigationService;
    
    [Reactive]
    public string PageTitle { get; set; }
    
    [ObservableAsProperty]
    public bool IsRefreshing { get; }
    
    public ReactiveCommand<bool, ICollection<CrewModel>?> LoadCommand { get; private set; }
    
    public ReactiveCommand<CrewModel, Unit> NavigateToDetailCommand { get; private set; }
    
    private ReadOnlyObservableCollection<CrewModel> _crew;

    public ReadOnlyObservableCollection<CrewModel> Crew
    {
        get => _crew;
        set => this.RaiseAndSetIfChanged(ref _crew, value);
    }

    private static readonly Func<CrewModel, string> KeySelector = crew => crew.Name;
    readonly SourceCache<CrewModel, string> _crewCache = new(KeySelector);
        
    public ViewModelActivator Activator { get; } = new();
    
    public MainPageViewModel(ISchedulerProvider schedulerProvider,
        ICrewRepository crewRepository,
        INavigationService navigationService)
    {
        _schedulerProvider = schedulerProvider;
        _crewRepository = crewRepository;
        _navigationService = navigationService;
        
        PageTitle = "People In Space Maui";
        
        var crewSort = SortExpressionComparer<CrewModel>
            .Ascending(c => c.Name);

        var crewSubscription = _crewCache.Connect()
            .Sort(crewSort)
            .Bind(out _crew)
            .ObserveOn(_schedulerProvider.MainThread)        
            .DisposeMany()                              
            .Subscribe();
        
        LoadCommand = ReactiveCommand.CreateFromObservable<bool, ICollection<CrewModel>?>(
            forceRefresh =>  _crewRepository.GetCrew(forceRefresh),
            this.WhenAnyValue(x => x.IsRefreshing).Select(x => !x), 
            outputScheduler: _schedulerProvider.MainThread); 
        LoadCommand.ThrownExceptions.Subscribe(Crew_OnError);
        LoadCommand.Subscribe(Crew_OnNext);
        
        NavigateToDetailCommand = ReactiveCommand.Create<CrewModel>(NavigateToDetail);
        
        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(x => x._crewRepository.IsBusy)
                .ObserveOn(_schedulerProvider.MainThread)
                .ToPropertyEx(this, x => x.IsRefreshing, scheduler: _schedulerProvider.MainThread)
                .DisposeWith(disposables);
            
            disposables.Add(crewSubscription);
        });
    }
    
    private void Crew_OnNext(ICollection<CrewModel>? crew)
    {
        try
        {
            if (crew != null) UpdateCrew(crew);
        }
        catch (Exception exception)
        {
            Crew_OnError(exception);
        }
    }
    
    private void UpdateCrew(ICollection<CrewModel> crew)
    {
        _crewCache.Edit(innerCache =>
        {
            innerCache.AddOrUpdate(crew);
        });
    }

    private void Crew_OnError(Exception e)
    {
        _navigationService.DisplayToast(e.Message).FireAndForgetSafeAsync();
    }
    
    private void NavigateToDetail(CrewModel crewMember)
    {
        var name = Uri.EscapeDataString(crewMember.Name);
        var image = Uri.EscapeDataString(crewMember.Image.ToString());
        var wikipedia = Uri.EscapeDataString(crewMember.Wikipedia.ToString());
        
        var route = $"{Routes.DetailPage}?name={name}&image={image}&wikipedia={wikipedia}";
        _navigationService.NavigateAsync(route);
    }
}   