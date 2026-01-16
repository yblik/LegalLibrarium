public sealed class EvidenceItem
{
    public int Id { get; }
    public string Title { get; }

    public EvidenceItem(int id, string title)
    {
        Id = id;
        Title = title;
    }
}
