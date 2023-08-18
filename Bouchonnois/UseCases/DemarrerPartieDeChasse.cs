using Bouchonnois.Domain;
using LanguageExt;

namespace Bouchonnois.UseCases
{
    public sealed class DemarrerPartieDeChasse : IUseCase<Domain.Commands.DemarrerPartieDeChasse, Guid>
    {
        private readonly IPartieDeChasseRepository _repository;
        private readonly Func<DateTime> _timeProvider;

        public DemarrerPartieDeChasse(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public Either<Error, Guid> Handle(Domain.Commands.DemarrerPartieDeChasse demarrerPartieDeChasse)
            => PartieDeChasse.Create(_timeProvider, demarrerPartieDeChasse)
                .Do(p => _repository.Save(p))
                .Map(p => p.Id);
    }
}