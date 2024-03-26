using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems(
        string searchTerm,
        int pageNumber = 1,
        int pageSize = 4
    )
    {
        var query = DB.PagedSearch<Item>();

        query.Sort(x => x.Ascending(a => a.Make));

        query.PageNumber(pageNumber);
        query.PageSize(pageSize);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query.Match(Search.Full, searchTerm).SortByTextScore();
        }

        var results = await query.ExecuteAsync();

        return Ok(
            new
            {
                results = results.Results,
                pageCount = results.PageCount,
                totalCount = results.TotalCount
            }
        );
    }
}
