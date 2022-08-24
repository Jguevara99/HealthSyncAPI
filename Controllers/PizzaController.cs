using System.Net.Mime;
using ContosoPizza.Models;
using ContosoPizza.Services;
using ContosoPizza.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class PizzaController : ControllerBase
{
    public PizzaController() { }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult<List<Pizza>> GetAll() => PizzaService.GetAll();

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pizza))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseHttpResponse))]
    public IActionResult Get(int id)
    {
        Pizza? pizza = PizzaService.Get(id);
        return pizza == null ? throw new HttpException("Pizza not found!", StatusCodes.Status404NotFound) : Ok(pizza);
    }

    [HttpPost]
    [Authorize(Permissions.Pizzas.Create)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseHttpResponse<Pizza>))]
    public IActionResult Create(Pizza pizza)
    {
        Pizza addedPizza = PizzaService.Add(pizza);
        return CreatedAtAction(nameof(Create), new { id = pizza.Id }, new Created<Pizza>("pizza added successfully!", addedPizza));
    }

    [HttpPut("{id}")]
    [Authorize(Permissions.Pizzas.Update)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseHttpResponse<Pizza>))]
    public IActionResult Update(int id, Pizza pizza)
    {
        pizza.Id = id;
        Pizza updatedPizza = PizzaService.Update(pizza);
        return Ok(new Ok("Pizza updated successfully!", pizza));
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions.Pizzas.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseHttpResponse<Pizza>))]
    public IActionResult Delete(int id)
    {
        return Ok(new Ok("Pizza deleted successfully!", PizzaService.Delete(id)));
    }
}