using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.ApiModels.Responses;
internal class ErrorResponse<T>
{
    public ErrorResponseData<T> Error { get; set; } = null!;
}