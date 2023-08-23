namespace Bouchonnois.Domain.Tirer
{
    public record ChasseurAVouluTiréPendantLApéro
        (Guid Id, DateTime Date, string Chasseur) : global::Domain.Core.Event(Id, 1, Date)
    {
        public override string ToString() => $"{Chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!";
    }
}