using System.Reactive.Linq;
using Akavache;
using PeopleInSpaceMaui.Apis;
using PeopleInSpaceMaui.Models;
using PeopleInSpaceMaui.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PeopleInSpaceMaui.Repositories;

public interface ICrewRepository
{
    bool IsBusy { get; set; }
    IObservable<ICollection<CrewModel>?> GetCrew(bool forceRefresh = false);
}

public class CrewRepository(
    ISchedulerProvider schedulerProvider,
    ISpaceXApi spaceXApi,
    IBlobCache cache)
    : ReactiveObject, ICrewRepository
{
    private const string CrewCacheKey = "crew_cache_key";
    private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(2);

    [Reactive]
    public bool IsBusy { get; set; }

    public IObservable<ICollection<CrewModel>?> GetCrew(bool forceRefresh = false)
    {
        return Observable.Defer(() =>
        {
            IsBusy = true;
            if (forceRefresh)
            {
                return FetchAndCacheCrew();
            }

            DateTimeOffset? expiration = DateTimeOffset.Now + _cacheLifetime;
            return cache.GetOrFetchObject(CrewCacheKey,
                    fetchFunc: FetchAndCacheCrew,
                    absoluteExpiration: expiration)
                .Do(_ => IsBusy = false);
        }).SubscribeOn(schedulerProvider.ThreadPool);
    }

    private IObservable<ICollection<CrewModel>> FetchAndCacheCrew()
    {
        return Observable.Create<ICollection<CrewModel>>(async observer =>
        {
            try
            {
                var crewJson = await spaceXApi.GetAllCrew().ConfigureAwait(false);
                var crew = CrewModel.FromJson(crewJson).ToList();
                await cache.InsertObject(CrewCacheKey, crew, DateTimeOffset.Now + _cacheLifetime);
                observer.OnNext(crew);
                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }).SubscribeOn(schedulerProvider.ThreadPool);
    }
}