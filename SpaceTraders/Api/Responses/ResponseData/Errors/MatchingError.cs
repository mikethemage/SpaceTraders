using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Api.Responses.ResponseData.Errors;
public class MatchingError : IErrorData
{
    public string Expected { get; set; } = string.Empty;
    public string Actual { get; set; } = string.Empty;

    public string GetErrorAsText()
    {
        return $"Expected: {Expected}, Actual: {Actual}";
    }
}