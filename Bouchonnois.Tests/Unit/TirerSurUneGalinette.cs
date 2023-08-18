using Bouchonnois.Domain.Exceptions;
using Bouchonnois.Tests.Builders;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.Tests.Unit
{
    public class TirerSurUneGalinette : UseCaseTest<UseCases.TirerSurUneGalinette>
    {
        public TirerSurUneGalinette() : base((r, p) => new UseCases.TirerSurUneGalinette(r, p))
        {
        }

        [Fact]
        public void AvecUnChasseurAyantDesBallesEtAssezDeGalinettesSurLeTerrain()
        {
            Given(
                UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes()
                ));

            WhenWithException(id => _useCase.Handle(new Domain.Commands.TirerSurUneGalinette(id, Data.Bernard)));

            Then(savedPartieDeChasse =>
                savedPartieDeChasse
                    .Should()
                    .HaveEmittedEvent(Now, "Bernard tire sur une galinette").And
                    .ChasseurATiréSurUneGalinette(Data.Bernard, ballesRestantes: 7, galinettes: 1).And
                    .GalinettesSurLeTerrain(2)
            );
        }

        public class Echoue : UseCaseTest<UseCases.TirerSurUneGalinette>
        {
            public Echoue() : base((r, p) => new UseCases.TirerSurUneGalinette(r, p))
            {
            }

            [Fact]
            public void CarPartieNexistePas()
            {
                Given(UnePartieDeChasseInexistante());

                WhenWithException(id => _useCase.Handle(new Domain.Commands.TirerSurUneGalinette(id, Data.Bernard)));

                ThenThrow<LaPartieDeChasseNexistePas>(savedPartieDeChasse => savedPartieDeChasse.Should().BeNull());
            }

            [Fact]
            public void AvecUnChasseurNayantPlusDeBalles()
            {
                Given(UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes()
                        .Avec(Dédé(), Bernard().SansBalles(), Robert())
                ));

                WhenWithException(id => _useCase.Handle(new Domain.Commands.TirerSurUneGalinette(id, Data.Bernard)));

                ThenThrow<TasPlusDeBallesMonVieuxChasseALaMain>(savedPartieDeChasse =>
                    savedPartieDeChasse.Should()
                        .HaveEmittedEvent(Now,
                            "Bernard veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main")
                );
            }

            [Fact]
            public void CarPasDeGalinetteSurLeTerrain()
            {
                Given(UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes(1)
                        .Avec(Dédé().AyantTué(1), Robert())
                ));

                WhenWithException(id => _useCase.Handle(new Domain.Commands.TirerSurUneGalinette(id, Data.Bernard)));

                ThenThrow<TasTropPicoléMonVieuxTasRienTouché>(savedPartieDeChasse =>
                    savedPartieDeChasse.Should().BeNull()
                );
            }

            [Fact]
            public void CarLeChasseurNestPasDansLaPartie()
            {
                Given(UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes()
                        .Avec(Dédé(), Robert())
                ));

                WhenWithException(id =>
                    _useCase.Handle(new Domain.Commands.TirerSurUneGalinette(id, Data.ChasseurInconnu)));

                ThenThrow<ChasseurInconnu>(savedPartieDeChasse =>
                        savedPartieDeChasse.Should().BeNull(),
                    expectedMessage: "Chasseur inconnu Chasseur inconnu"
                );
            }

            [Fact]
            public void SiLesChasseursSontEnApero()
            {
                Given(UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes()
                        .Avec(Dédé(), Robert())
                        .ALapéro()
                ));

                WhenWithException(id =>
                    _useCase.Handle(new Domain.Commands.TirerSurUneGalinette(id, Data.ChasseurInconnu)));

                ThenThrow<OnTirePasPendantLapéroCestSacré>(savedPartieDeChasse =>
                    savedPartieDeChasse.Should()
                        .HaveEmittedEvent(Now,
                            "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!")
                );
            }

            [Fact]
            public void SiLaPartieDeChasseEstTerminée()
            {
                Given(UnePartieDeChasseExistante(
                    SurUnTerrainRicheEnGalinettes()
                        .Avec(Dédé(), Robert())
                        .Terminée()
                ));

                WhenWithException(id =>
                    _useCase.Handle(new Domain.Commands.TirerSurUneGalinette(id, Data.ChasseurInconnu)));

                ThenThrow<OnTirePasQuandLaPartieEstTerminée>(savedPartieDeChasse =>
                    savedPartieDeChasse.Should().HaveEmittedEvent(Now,
                        "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée")
                );
            }
        }
    }
}