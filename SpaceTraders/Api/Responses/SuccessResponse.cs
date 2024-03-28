using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Api.Responses;
internal class SuccessResponse<T>
{
    public T Data { get; set; } = default!;
    public Meta? Meta { get; set; } = null;

}
