using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceTraders.Api.Responses.ResponseData;

namespace SpaceTraders.Api.Responses;
internal class ErrorResponse<T> where T : IErrorResponseData
{
    public T Error { get; set; } = default!;
}