using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using DotnetBase.Infrastructure.Mvc.Filters;
using DotnetBase.Application.Queries.Categories.Request;
using DotnetBase.Application.Queries.Request;

namespace DotnetBase.Api.Controllers
{
    [Route("api/categories")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Authorize(Policy = Policy.ADMIN_ACCESS)]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CategoryController" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///   Get a page of Category.
        /// </summary>
        /// <param name="request">The request for Category, with paging.</param>
        /// <param name="cancellationToken">The cancellation token to abort execution.</param>
        /// <returns>Returns the page.</returns>
        [HttpGet]
        public async Task<dynamic> GetAsync([FromQuery] CategoryPageListRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }

        /// <summary>
        ///   Get a Category.
        /// </summary>
        /// <param name="id">The id of the Category.</param>
        /// <param name="cancellationToken">The cancellation token to abort execution.</param>
        /// <returns>Returns the Category.</returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var request = new CategoryGetByIdRequest { Id = id };
            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }

        /// <summary>
        ///   Create a new Category.
        /// </summary>
        /// <param name="request">The request body when create a new Category.</param>
        /// <param name="cancellationToken">The cancellation token to abort execution.</param>
        /// <returns>Returns.</returns>
        [HttpPost]
        public async Task<dynamic> PostAsync([FromBody] CategoryCreateRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }

        /// <summary>
        ///   Create a new Category.
        /// </summary>
        /// <param name="request">The request body when create a new Category.</param>
        /// <param name="cancellationToken">The cancellation token to abort execution.</param>
        /// <returns>Returns.</returns>
        [HttpPut("{id}")]
        public async Task<dynamic> PutAsync(Guid id, [FromBody] CategoryEditRequest request, CancellationToken cancellationToken)
        {
            request.Id = id;
            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<dynamic> PatchAsync(Guid id, [FromBody] JObject packageObj, CancellationToken cancellationToken)
        {
            var request = new CategoryPatchRequest()
            {
                Id = id,
                CategoryPatchModel = packageObj
            };

            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }

        /// <summary>
        ///   Delete a Category.
        /// </summary>
        /// <param name="id">The id of the Category.</param>
        /// <param name="cancellationToken">The cancellation token to abort execution.</param>
        /// <returns>Returns.</returns>
        [HttpDelete("{id}")]
        public async Task<dynamic> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var request = new CategoryDeleteRequest()
            {
                Id = id
            };
            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }
    }
}
