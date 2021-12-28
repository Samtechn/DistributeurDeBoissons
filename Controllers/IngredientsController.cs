using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DistributeurBoisson.Models;
using System.IO;
using Newtonsoft.Json;

namespace DistributeurBoisson.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        //Get and Set ingredients to text file
        public static List<Ingredient> LIngredients
        {
            get
            {
                using (StreamReader streamReader = new StreamReader(@".\Text Files\Ingredients.txt", System.Text.Encoding.UTF8))
                {
                    return JsonConvert.DeserializeObject<List<Ingredient>>(streamReader.ReadToEnd(),new JsonSerializerSettings { });                    
                }
            }

            set
            {
                using (StreamWriter streamWriter = new StreamWriter(@".\Text Files\Ingredients.txt", false, System.Text.Encoding.UTF8))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(value));                    
                }
            }
        }


        // GET: api/Ingredient
        /// <summary>Get all the available ingredients</summary>
        /// <response code="200">Ingredients selected</response>
        /// <response code="204">No ingredients found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Ingredient>), 200)]
        public IActionResult GetAllIngredients()
        {
            try
            {
                var ParmListIngred = LIngredients;

                if (ParmListIngred == null)
                {
                    return NoContent();
                }
                else
                {
                    return new ObjectResult(ParmListIngred);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }         
        }

        // GET: api/Ingredient/5
        /// <summary>Select an ingredient by it's name</summary>
        /// <param name="name">Ingredient's name</param>
        /// <response code="200">Ingredient selected</response>
        /// <response code="204">Ingredient not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{name}")]
        [ProducesResponseType(typeof(Ingredient), 200)]
        public IActionResult GetIngredientByName(string name)
        {
            try
            {
                List<Ingredient> ParmListIngred = LIngredients;                

                if (ParmListIngred == null || LIngredients.Find(x => x.Name == name) == null)
                {
                    return NoContent();
                }
                else
                {
                    Ingredient TargetIngredient = LIngredients.Find(x => x.Name == name);
                    return new ObjectResult(TargetIngredient);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        // POST: api/Ingredient        
        /// <summary>Add a new ingredient to the list of ingredients</summary>
        /// <param name="value">Ingredient's properties</param>
        /// <response code="201">Ingredient added</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), 201)]
        public IActionResult CreateNewIngredient([FromBody] Ingredient value)
        {
            try
            {
                List<Ingredient> ParmListIngred = LIngredients;

                if (ParmListIngred == null) { ParmListIngred = new List<Ingredient>(); }

                ParmListIngred.Add(value);

                LIngredients = ParmListIngred;

                return CreatedAtAction("CreateNewIngredient", "Ingredient '" + value.Name + "' was created.");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        // PUT: api/Ingredient/5
        /// <summary>Modify an ingredient's price</summary>
        /// <param name="name">Ingredient's name</param>
        /// <param name="price">New price</param>
        /// <response code="201">Price modified</response>
        /// <response code="204">Ingredient not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [ProducesResponseType(typeof(string), 201)]
        public IActionResult ModifyIngredientPrice(string name, double price)
        {
            try
            {
                List<Ingredient> ParmListIngred = LIngredients;                

                if (ParmListIngred == null || ParmListIngred.Find(x => x.Name == name) == null)
                {
                    return NoContent();
                }
                else
                {
                    Ingredient TargetIngredient = ParmListIngred.Find(x => x.Name == name);
                    ParmListIngred.Remove(TargetIngredient);
                    ParmListIngred.Add(new Ingredient {Name = name, Price = price});
                    LIngredients = ParmListIngred;

                    return CreatedAtAction("ModifyIngredientPrice", "Price of ingredient '" + name + "' was Modified.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        // DELETE: api/ApiWithActions/5
        /// <summary>Delete an ingredient</summary>
        /// <param name="name">Ingredient's name</param>
        /// <response code="201">Ingredient deleted</response>
        /// <response code="204">Ingredient not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{name}")]
        [ProducesResponseType(typeof(string), 201)]
        public IActionResult DeleteIngredient(string name)
        {
            try
            {
                //Avant de supprimer l'ingredient verifier s'il ne compose pas une recette 
                List<Recipe> ParmListRecipe = RecipesController.LRecipes;
                if (ParmListRecipe != null)
                {
                    foreach (Recipe r in ParmListRecipe)
                    {
                        if (r.DicComposition.Keys.Contains(name))
                        {
                            throw new Exception("Cannot delete the ingredient " + name + " : The recipe " + r.Name + " contains this ingredient.");
                        }
                    }
                }                

                //Proceder à la suppression de l'ingredient
                List<Ingredient> ParmListIngred = LIngredients;
                if (ParmListIngred == null || ParmListIngred.Find(x => x.Name == name) == null)
                {
                    return NoContent();
                }
                else
                {
                    Ingredient TargetIngredient = ParmListIngred.Find(x => x.Name == name);
                    ParmListIngred.Remove(TargetIngredient);
                    LIngredients = ParmListIngred;

                    return CreatedAtAction("DeleteIngredient", "Ingredient '" + name + "' was deleted.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
    }
}
