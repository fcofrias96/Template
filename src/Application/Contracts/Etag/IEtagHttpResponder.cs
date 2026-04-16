public interface IEtagHttpResponder
{
    /// <summary>
    /// Lee el If-None-Match del request actual (si existe).
    /// </summary>
    string? GetClientEtag();

    /// <summary>
    /// Aplica ETag header y, si coincide con clientEtag, pone StatusCode=304.
    /// Retorna true si fue NotModified.
    /// </summary>
    bool Apply(string etag, string? clientEtag = null);
}
