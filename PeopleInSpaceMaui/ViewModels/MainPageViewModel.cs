using DynamicData;
using DynamicData.Binding;
using PeopleInSpaceMaui.Models;
using PeopleInSpaceMaui.Queries;
using PeopleInSpaceMaui.Reactive;

namespace PeopleInSpaceMaui.ViewModels;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

public class MainPageViewModel : ReactiveObject
    {
        ISchedulerProvider _schedulerProvider;

        IPeopleInSpaceQuery _peopleInSpaceQuery;

        [Reactive]
        public string Greeting { get; set; }

       
        [ObservableAsProperty]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public bool IsBusy { get; }

        public ReactiveCommand<Unit, ICollection<CrewModel>> RefreshCommand { get; set; }

        ReadOnlyObservableCollection<CrewModel> _crew;

        public ReadOnlyObservableCollection<CrewModel> Crew
        {
            get => _crew;
            set => this.RaiseAndSetIfChanged(ref _crew, value);
        }

        private static readonly Func<CrewModel, string> KeySelector = crew => crew.Name;
        readonly SourceCache<CrewModel, string> _crewCache = new SourceCache<CrewModel, string>(KeySelector);


        public MainPageViewModel(ISchedulerProvider schedulerProvider,
            IPeopleInSpaceQuery peopleInSpaceQuery)
        {
            _schedulerProvider = schedulerProvider;
            _peopleInSpaceQuery = peopleInSpaceQuery;

            Greeting = "People In Space";

            this.WhenAnyValue(x => x._peopleInSpaceQuery.IsBusy)
                .ObserveOn(_schedulerProvider.MainThread)
                .ToPropertyEx(this, x => x.IsBusy, scheduler: _schedulerProvider.MainThread);
           
            RefreshCommand = ReactiveCommand.CreateFromObservable(
                () => _peopleInSpaceQuery.GetCrew(false),
                //this.WhenAnyValue(x => x.IsBusy).Select(x => !x),
                outputScheduler: _schedulerProvider.MainThread); // If ThreadPool is used here you get a UIKit Main thread exception on iOS
            RefreshCommand.ThrownExceptions.Subscribe(Crew_OnError);
            RefreshCommand.Subscribe(Crew_OnNext);

            var crewSort = SortExpressionComparer<CrewModel>
                .Ascending(c => c.Name);

            _ = _crewCache.Connect()
                .Sort(crewSort)
                .Bind(out _crew)
                .ObserveOn(_schedulerProvider.MainThread)        //ensure operation is on the UI thread;
                .DisposeMany()                              //automatic disposal
                .Subscribe();
        }

        void Crew_OnNext(ICollection<CrewModel> crew)
        {
            try
            {
                UpdateCrew(crew);
            }
            catch (Exception exception)
            {
                Crew_OnError(exception);
            }
        }

        void UpdateCrew(ICollection<CrewModel> crew)
        {
            _schedulerProvider.MainThread.Schedule(_ =>
            {
                _crewCache.Edit(innerCache =>
                {
                    innerCache.Clear();
                    innerCache.AddOrUpdate(crew);
                });
            });
        }

        void Crew_OnError(Exception e)
        {
            
        }
    }   