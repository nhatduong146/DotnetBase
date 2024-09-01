using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotnetBase.Application.Exports.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBase.Api.Controllers
{
    [Route("api/exports")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ExportController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CategoryController" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public ExportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///   Get a page of Category.
        /// </summary>
        /// <param name="request">The request for Category, with paging.</param>
        /// <param name="cancellationToken">The cancellation token to abort execution.</param>
        /// <returns>Returns the page.</returns>
        [HttpGet("categories")]
        public async Task<dynamic> ExportCategoryAsync([FromQuery] ExportCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            var excelName = $"Category-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File((MemoryStream)result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
