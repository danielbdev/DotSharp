namespace DotSharp.Primitives.Constants;

/// <summary>
/// Provides access to the current header values, including request and response headers.
/// Implements the Singleton pattern to ensure only one instance exists.
/// </summary>
public class HeadersValuesProvider
{
    private static HeadersValuesProvider _current;

    /// <summary>
    /// Gets the current instance of <see cref="HeadersValuesProvider"/>.
    /// </summary>
    public static HeadersValuesProvider Current
    {
        get
        {
            _current ??= new HeadersValuesProvider();

            return _current;
        }
    }

    /// <summary>
    /// Request-specific header values, such as pagination details.
    /// </summary>
    public RequestHeaders Request { get; set; }

    /// <summary>
    /// Response-specific header values, such as total record count.
    /// </summary>
    public ResponseHeaders Response { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeadersValuesProvider"/> class with default values.
    /// </summary>
    public HeadersValuesProvider()
    {
        Request = new RequestHeaders();
        Response = new ResponseHeaders();
    }

    /// <summary>
    /// Initializes the current header values with the provided <see cref="HeadersValuesProvider"/>.
    /// </summary>
    /// <param name="headersValues">The instance of <see cref="HeadersValuesProvider"/> to initialize from.</param>
    public void Initialize(HeadersValuesProvider headersValues)
    {
        Request = headersValues.Request;
        Response = headersValues.Response;
    }
}

/// <summary>
/// Contains request header values, such as page number and page size for pagination.
/// </summary>
public class RequestHeaders
{
    /// <summary>
    /// The current page number for pagination.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page for pagination.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestHeaders"/> class with default values.
    /// </summary>
    public RequestHeaders()
    {
        PageNumber = 1;
        PageSize = 25;
    }
}

/// <summary>
/// Contains response header values, such as the total number of records.
/// </summary>
public class ResponseHeaders
{
    /// <summary>
    /// The total number of records in the response.
    /// </summary>
    public string? TotalRecords { get; set; }
}