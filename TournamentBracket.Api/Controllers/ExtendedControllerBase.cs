using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Api.Controllers;

public abstract class ExtendedControllerBase : ControllerBase
{
    protected IActionResult ToActionResult(Result result, int okCode = 200)
    {
        return result.IsSuccess
            ? OkResultByCode(okCode)
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }

    protected IActionResult ToActionResult<TItem>(Result<TItem> result, int okCode = 200)
    {
        return result.IsSuccess
            ? OkResultByCode(result.Item, okCode)
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }

    private ActionResult OkResultByCode(int okCode)
    {
        return okCode switch
        {
            200 => Ok(),
            204 => Created(),
            _ => Ok()
        };
    }

    private ActionResult OkResultByCode<TItem>(TItem item, int okCode)
    {
        return okCode switch
        {
            200 => Ok(item),
            204 => Created(),
            _ => Ok()
        };
    }
}