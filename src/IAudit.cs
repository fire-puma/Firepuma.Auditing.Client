namespace Firepuma.Auditing.Client
{
    public interface IAudit
    {
        string Action { get; }
        string EntityId { get; }
        string EntityDescription { get; }
        object OldValue { get; }
        object NewValue { get; }
    }
}
