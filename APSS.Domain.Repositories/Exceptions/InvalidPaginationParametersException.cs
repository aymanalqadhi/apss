namespace APSS.Domain.Repositories.Exceptions;

public class InvalidPaginationParametersException : Exception
{
    #region Private fields

    private readonly int _page;
    private readonly int _pageLength;

    #endregion Private fields

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="page">The page index</param>
    /// <param name="pageLength">The page length</param>
    public InvalidPaginationParametersException(int page, int pageLength)
        : base($"invalid pagination parameters ({page}, {pageLength})")
    {
        _page = page;
        _pageLength = pageLength;
    }

    #endregion Constructors

    #region Public properties

    /// <summary>
    /// Gets the page index
    /// </summary>
    public int Page => _page;

    /// <summary>
    /// Gets the page length
    /// </summary>
    public int PageLength => _pageLength;

    #endregion Public properties
}