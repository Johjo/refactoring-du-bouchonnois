using Bouchonnois.Domain.Commands;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;
using Bouchonnois.UseCases;
using FluentAssertions.Extensions;
using static Bouchonnois.Tests.Builders.CommandBuilder;
using ConsulterStatus = Bouchonnois.UseCases.ConsulterStatus;
using DemarrerPartieDeChasse = Bouchonnois.UseCases.DemarrerPartieDeChasse;
using PrendreLapéro = Bouchonnois.UseCases.PrendreLapéro;
using ReprendreLaPartie = Bouchonnois.UseCases.ReprendreLaPartie;
using TerminerLaPartie = Bouchonnois.UseCases.TerminerLaPartie;
using Tirer = Bouchonnois.UseCases.Tirer;
using TirerSurUneGalinette = Bouchonnois.Domain.Commands.TirerSurUneGalinette;

namespace Bouchonnois.Tests.Acceptance
{
    [UsesVerify]
    public class ScenarioTests
    {
        private DateTime _time = new(2024, 4, 25, 9, 0, 0);

        private readonly DemarrerPartieDeChasse _demarrerPartieDeChasse;
        private readonly Tirer _tirer;
        private readonly UseCases.TirerSurUneGalinette _tirerSurUneGalinette;
        private readonly PrendreLapéro _prendreLapéro;
        private readonly ReprendreLaPartie _reprendreLaPartie;
        private readonly TerminerLaPartie _terminerLaPartie;
        private readonly ConsulterStatus _consulterStatus;

        public ScenarioTests()
        {
            var repository = new PartieDeChasseRepositoryForTests();
            var timeProvider = () => _time;

            _demarrerPartieDeChasse = new DemarrerPartieDeChasse(repository, timeProvider);
            _tirer = new Tirer(repository, timeProvider);
            _tirerSurUneGalinette = new UseCases.TirerSurUneGalinette(repository, timeProvider);
            _prendreLapéro = new PrendreLapéro(repository, timeProvider);
            _reprendreLaPartie = new ReprendreLaPartie(repository, timeProvider);
            _terminerLaPartie = new TerminerLaPartie(repository, timeProvider);
            _consulterStatus = new ConsulterStatus(repository);
        }

        [Fact]
        public Task DéroulerUnePartie()
        {
            var command = DémarrerUnePartieDeChasse()
                .Avec((Data.Dédé, 20), (Data.Bernard, 8), (Data.Robert, 12))
                .SurUnTerrainRicheEnGalinettes(4);

            var id = _demarrerPartieDeChasse.Handle(command.Build());

            After(10.Minutes(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Dédé)));
            After(30.Minutes(), () => _tirerSurUneGalinette.Handle(new TirerSurUneGalinette(id, Data.Robert)));
            After(20.Minutes(), () => _prendreLapéro.Handle(new Domain.Commands.PrendreLapéro(id)));
            After(1.Hours(), () => _reprendreLaPartie.Handle(new Domain.Commands.ReprendreLaPartie(id)));
            After(2.Minutes(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Bernard)));
            After(1.Minutes(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Bernard)));
            After(1.Minutes(), () => _tirerSurUneGalinette.Handle(new TirerSurUneGalinette(id, Data.Dédé)));
            After(26.Minutes(), () => _tirerSurUneGalinette.Handle(new TirerSurUneGalinette(id, Data.Robert)));
            After(10.Minutes(), () => _prendreLapéro.Handle(new Domain.Commands.PrendreLapéro(id)));
            After(170.Minutes(), () => _reprendreLaPartie.Handle(new Domain.Commands.ReprendreLaPartie(id)));
            After(11.Minutes(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Bernard)));
            After(1.Seconds(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Bernard)));
            After(1.Seconds(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Bernard)));
            After(1.Seconds(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Bernard)));
            After(1.Seconds(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Bernard)));
            After(1.Seconds(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Bernard)));
            After(1.Seconds(), () => _tirer.Handle(new Domain.Commands.Tirer(id, Data.Bernard)));
            After(19.Minutes(), () => _tirerSurUneGalinette.Handle(new TirerSurUneGalinette(id, Data.Robert)));
            After(30.Minutes(), () => _terminerLaPartie.Handle(new Domain.Commands.TerminerLaPartie(id)));

            return Verify(_consulterStatus.Handle(new Domain.Commands.ConsulterStatus(id)));
        }

        private void After(TimeSpan time, Action act)
        {
            _time = _time.Add(time);
            try
            {
                act();
            }
            catch
            {
                // ignored
            }
        }
    }
}