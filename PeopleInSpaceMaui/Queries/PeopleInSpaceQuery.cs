using PeopleInSpaceMaui.Apis;
using PeopleInSpaceMaui.Models;
using PeopleInSpaceMaui.Reactive;

namespace PeopleInSpaceMaui.Queries;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

public interface IPeopleInSpaceQuery
    {
        bool IsBusy { get; set; }

        IObservable<ICollection<CrewModel>> GetCrew(bool forceRefresh = false);
    }

    public class PeopleInSpaceQuery : ReactiveObject, IPeopleInSpaceQuery
    {
        readonly ISchedulerProvider _schedulerProvider;
        readonly ISpaceXApi _spaceXApi;

        [Reactive]
        public bool IsBusy { get; set; }

        List<CrewModel> _crew = new List<CrewModel>();

        public PeopleInSpaceQuery(ISchedulerProvider schedulerProvider,
            ISpaceXApi spaceXApi)
        {
            _schedulerProvider = schedulerProvider;
            _spaceXApi = spaceXApi;
        }

        public IObservable<ICollection<CrewModel>> GetCrew(bool forceRefresh = false)
        {
            return Observable.Create<ICollection<CrewModel>>(async observer =>
            {
                var results = await GetCrewAsync(forceRefresh).ConfigureAwait(false);

                observer.OnNext(results);

            }).SubscribeOn(_schedulerProvider.MainThread);
        }

        async Task<ICollection<CrewModel>> GetCrewAsync(bool forceRefresh = false)
        {
            IsBusy = true;

            var crewJson = await  _spaceXApi.GetAllCrew().ConfigureAwait(false);

            _crew = CrewModel.FromJson(crewJson).ToList();
                        
            IsBusy = false;

            return _crew;
        }
    }