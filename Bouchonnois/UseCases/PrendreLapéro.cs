using Bouchonnois.Domain;
using static Bouchonnois.UseCases.VoidResponse;

namespace Bouchonnois.UseCases
{
    public sealed class PrendreLapéro : PartieDeChasseUseCase<Domain.Commands.PrendreLapéro, VoidResponse>
    {
        public PrendreLapéro(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
            : base(repository,
                (partieDeChasse, _) =>
                {
                    partieDeChasse.PrendreLapéro(timeProvider);
                    return Empty;
                })
        {
        }
    }
}