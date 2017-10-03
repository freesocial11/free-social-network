namespace mobSocial.Core
{
    /// <summary>
    /// Specifies that a particular entity is application specific
    /// </summary>
    public interface IPerApplicationEntity
    {
        int ApplicationId { get; set; }
    }
}