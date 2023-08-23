using Bouchonnois.Domain;
using LanguageExt;

namespace Bouchonnois.UseCases
{
    public sealed class DemarrerPartieDeChasse : IUseCase<Domain.Démarrer.DemarrerPartieDeChasse, Guid>
    {
        private readonly IPartieDeChasseRepository _repository;
        private readonly Func<DateTime> _timeProvider;

        public DemarrerPartieDeChasse(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public EitherAsync<Error, Guid> Handle(Domain.Démarrer.DemarrerPartieDeChasse demarrerPartieDeChasse)
            => PartieDeChasse.Create(_timeProvider, demarrerPartieDeChasse)
                .MapAsync(async p => await _repository.Save(p))
                .MapAsync(p => p.Id)
                .ToAsync();
    }
}