using Bouchonnois.Domain.Terminer;
using Bouchonnois.Tests.Builders;
using TerminerLaPartie = Bouchonnois.UseCases.TerminerLaPartie;

namespace Bouchonnois.Tests.Unit
{
    public class TerminerLaPartieDeChasse : UseCaseTest<TerminerLaPartie, string>
    {
        public TerminerLaPartieDeChasse() : base((r, p) => new TerminerLaPartie(r))
        {
        }

        [Fact]
        public async Task QuandLaPartieEstEnCoursEt1SeulChasseurGagne()
        {
            Given(
                await UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes()
                        .Avec(Dédé(), Bernard(), Robert().AyantTué(2))
                ));

            await When(id => UseCase.Handle(new Domain.Terminer.TerminerLaPartie(id)));

            Then((winner, partieDeChasse) =>
            {
                partieDeChasse
                    .Should()
                    .HaveEmittedEvent(Repository, new PartieTerminée(partieDeChasse!.Id, Now, Data.Robert, 2));

                winner.Should().Be(Data.Robert);
            });
        }

        [Fact]
        public async Task QuandLaPartieEstEnCoursEt1SeulChasseurDansLaPartie()
        {
            Given(
                await UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes()
                        .Avec(Robert().AyantTué(2))
                )
            );

            await When(id => UseCase.Handle(new Domain.Terminer.TerminerLaPartie(id)));

            Then((winner, partieDeChasse) =>
            {
                partieDeChasse
                    .Should()
                    .HaveEmittedEvent(Repository, new PartieTerminée(partieDeChasse!.Id, Now, Data.Robert, 2));

                winner.Should().Be(Data.Robert);
            });
        }

        [Fact]
        public async Task QuandLaPartieEstEnCoursEt2ChasseursExAequo()
        {
            Given(
                await UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes(4)
                        .Avec(Dédé().AyantTué(2), Bernard().AyantTué(2), Robert())
                )
            );

            await When(id => UseCase.Handle(new Domain.Terminer.TerminerLaPartie(id)));

            Then((winner, partieDeChasse) =>
            {
                partieDeChasse
                    .Should()
                    .HaveEmittedEvent(Repository,
                        new PartieTerminée(partieDeChasse!.Id, Now, 2, Data.Dédé, Data.Bernard));

                winner.Should().Be("Dédé, Bernard");
            });
        }

        [Fact]
        public async Task QuandLaPartieEstEnCoursEtToutLeMondeBrocouille()
        {
            Given(
                await UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes()
                )
            );

            await When(id => UseCase.Handle(new Domain.Terminer.TerminerLaPartie(id)));

            Then((winner, partieDeChasse) =>
            {
                partieDeChasse
                    .Should()
                    .HaveEmittedEvent(Repository, new PartieTerminée(partieDeChasse!.Id, Now, "Brocouille", 0));

                winner.Should().Be("Brocouille");
            });
        }

        [Fact]
        public async Task QuandLesChasseursSontALaperoEtTousExAequo()
        {
            Given(
                await UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes(12)
                        .Avec(Dédé().AyantTué(3), Bernard().AyantTué(3), Robert().AyantTué(3))
                        .ALapéro()
                )
            );

            await When(id => UseCase.Handle(new Domain.Terminer.TerminerLaPartie(id)));

            Then((winner, partieDeChasse) =>
            {
                var partieExAequoTerminée =
                    new PartieTerminée(partieDeChasse!.Id, Now, 3, Data.Dédé, Data.Bernard, Data.Robert);

                partieDeChasse
                    .Should()
                    .HaveEmittedEvent(Repository, partieExAequoTerminée);

                partieExAequoTerminée
                    .ToString()
                    .Should()
                    .BeEquivalentTo(
                        "La partie de chasse est terminée, vainqueur : Dédé, Bernard, Robert - 3 galinettes");

                winner.Should().Be("Dédé, Bernard, Robert");
            });
        }

        public class Echoue : UseCaseTest<TerminerLaPartie, string>
        {
            public Echoue() : base((r, p) => new TerminerLaPartie(r))
            {
            }

            [Fact]
            public async Task SiLaPartieDeChasseEstDéjàTerminée()
            {
                Given(
                    await UnePartieDeChasseExistante(
                        SurUnTerrainRicheEnGalinettes()
                            .Terminée())
                );

                await When(id => UseCase.Handle(new Domain.Terminer.TerminerLaPartie(id)));

                ThenFailWith("Quand c'est fini, c'est fini");
            }
        }
    }
}