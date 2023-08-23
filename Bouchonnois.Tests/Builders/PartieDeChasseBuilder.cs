using ArchUnitNET.Domain.Extensions;
using Bouchonnois.Domain;
using Bouchonnois.Domain.Démarrer;
using static Bouchonnois.Domain.PartieStatus;
using static Bouchonnois.Tests.Builders.Functions;
using Chasseur = Bouchonnois.Domain.Chasseur;

namespace Bouchonnois.Tests.Builders
{
    public class PartieDeChasseBuilder
    {
        private readonly int _nbGalinettes;
        private ChasseurBuilder[] _chasseurs = {Dédé(), Bernard(), Robert()};
        private readonly List<PartieStatus> _status = new();

        private PartieDeChasseBuilder(int nbGalinettes) => _nbGalinettes = nbGalinettes;

        public static PartieDeChasseBuilder SurUnTerrainRicheEnGalinettes(int nbGalinettes = 3) => new(nbGalinettes);

        public static Guid UnePartieDeChasseInexistante() => Guid.NewGuid();

        public PartieDeChasseBuilder Avec(params ChasseurBuilder[] chasseurs)
        {
            _chasseurs = chasseurs;
            return this;
        }

        public PartieDeChasseBuilder ALapéro()
        {
            _status.Add(Apéro);
            return this;
        }

        public PartieDeChasseBuilder Terminée()
        {
            _status.Add(PartieStatus.Terminée);
            return this;
        }

        public PartieDeChasse Build(Func<DateTime> timeProvider)
        {
            var builtChasseurs = _chasseurs.Select(c => c.Build()).ToArray();
            var chasseursSansBalles = builtChasseurs.Where(c => c.BallesRestantes == 0).Select(c => c.Nom);

            var partieDeChasse = PartieDeChasse.Create(
                timeProvider,
                new DemarrerPartieDeChasse(
                    new TerrainDeChasse("Pitibon sur Sauldre", _nbGalinettes),
                    builtChasseurs
                        .Select(c => new Domain.Démarrer.Chasseur(c.Nom, c.BallesRestantes > 0 ? c.BallesRestantes : 1))
                        .ToList()
                )
            ).RightUnsafe();

            TirerSurLesGalinettes(partieDeChasse, builtChasseurs);
            TirerDansLeVide(partieDeChasse, timeProvider, chasseursSansBalles);

            ChangeStatus(partieDeChasse);

            return partieDeChasse;
        }

        private static void TirerDansLeVide(
            PartieDeChasse partieDeChasse,
            Func<DateTime> timeProvider,
            IEnumerable<string> chasseursSansBalles) =>
            chasseursSansBalles.ForEach(c => partieDeChasse.Tirer(c));

        private static void TirerSurLesGalinettes(
            PartieDeChasse partieDeChasse,
            IEnumerable<Chasseur> builtChasseurs) =>
            builtChasseurs
                .ToList()
                .ForEach(c => Repeat(c.NbGalinettes, () => partieDeChasse.TirerSurUneGalinette(c.Nom)));

        private void ChangeStatus(PartieDeChasse partieDeChasse) =>
            _status.ForEach(status => ChangeStatus(partieDeChasse, status));

        private static void ChangeStatus(PartieDeChasse partieDeChasse, PartieStatus status)
        {
            if (status == PartieStatus.Terminée) partieDeChasse.Terminer();
            else if (status == Apéro) partieDeChasse.PrendreLapéro();
        }
    }
}