using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("[controller]")]
public class PizzaController : ControllerBase
{
    public PizzaController() { }

    [HttpGet]
    public ActionResult<List<Pizza>> GetAll() => PizzaService.GetAll();

    [HttpGet("{id}")]
    public ActionResult<Pizza> Get(int id)
    {
        Pizza? pizza = PizzaService.Get(id);
        return pizza == null ? NotFound(new NotFound("pizza not found!")) : pizza;
    }

    [HttpPost]
    public IActionResult Create(Pizza pizza)
    {
        Pizza addedPizza = PizzaService.Add(pizza);
        return CreatedAtAction(nameof(Create), new { id = pizza.Id }, new Created("pizza added successfully!", addedPizza));
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Pizza pizza)
    {
        try
        {
            pizza.Id = id;
            Pizza updatedPizza = PizzaService.Update(pizza);
            return Ok(new Ok("Pizza updated successfully!", pizza));
        }
        catch (HttpException ex)
        {
            return StatusCode(((int)ex.Code), new BaseHttpResponse(ex.Message, ex.Code, false, ex.Body));
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id) 
    {
        return Ok(new Ok("Pizza deleted successfully!", PizzaService.Delete(id)));
    }
}