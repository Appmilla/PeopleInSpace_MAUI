using DynamicData;
using DynamicData.Binding;
using PeopleInSpaceMaui.Models;
using PeopleInSpaceMaui.Reactive;
using PeopleInSpaceMaui.Repositories;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace PeopleInSpaceMaui.ViewModels;

public class MainPageViewModel : ReactiveObject, IActivatableViewModel
{
    readonly ISchedulerProvider _schedulerProvider;
    private readonly ICrewRepository _crewRepository;
    
    [Reactive]
    public string Greeting { get; set; }
    
    [ObservableAsProperty]
    public bool IsRefreshing { get; }
       
    public ReactiveCommand<Unit, ICollection<CrewModel>?> RefreshCommand { get; set; }

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
        ICrewRepository crewRepository)
    {
        _schedulerProvider = schedulerProvider;
        _crewRepository = crewRepository;

        Greeting = "People In Space Maui";

        var crewSort = SortExpressionComparer<CrewModel>
            .Ascending(c => c.Name);

        var crewSubscription = _crewCache.Connect()
            .Sort(crewSort)
            .Bind(out _crew)
            .ObserveOn(_schedulerProvider.MainThread)        
            .DisposeMany()                              
            .Subscribe();
        
        var isRefreshingSubscription = this.WhenAnyValue(x => x._crewRepository.IsBusy)
            .ObserveOn(_schedulerProvider.MainThread)
            .ToPropertyEx(this, x => x.IsRefreshing, scheduler: _schedulerProvider.MainThread);
        
        this.WhenActivated(disposables =>
        {
            disposables.Add(crewSubscription);
            disposables.Add(isRefreshingSubscription);
        });
        
        RefreshCommand = ReactiveCommand.CreateFromObservable(
            () => _crewRepository.GetCrew(false),
            this.WhenAnyValue(x => x.IsRefreshing).Select(x => !x), 
            outputScheduler: _schedulerProvider.MainThread); 
        RefreshCommand.ThrownExceptions.Subscribe(Crew_OnError);
        RefreshCommand.Subscribe(Crew_OnNext);
        
        NavigateToDetailCommand = ReactiveCommand.Create<CrewModel>(NavigateToDetail);
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
        _schedulerProvider.MainThread.Schedule(_ =>
        {
            _crewCache.Edit(innerCache =>
            {
                innerCache.AddOrUpdate(crew);
            });
        });
    }

    private void Crew_OnError(Exception e)
    {
            
    }
    
    private void NavigateToDetail(CrewModel crewMember)
    {
        var name = Uri.EscapeDataString(crewMember.Name);
        var image = Uri.EscapeDataString(crewMember.Image.ToString());
        var wikipedia = Uri.EscapeDataString(crewMember.Wikipedia.ToString());

        var route = $"DetailPage?name={name}&image={image}&wikipedia={wikipedia}";
        Shell.Current.GoToAsync(route);
    }
}   