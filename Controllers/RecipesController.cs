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
    public class RecipesController : ControllerBase
    {
        //Charger la liste des recettes disponibles dans le Json
        public static List<Recipe> LRecipes
        {
            get
            {
                using (StreamReader streamReader = new StreamReader(@".\Text Files\Recipes.txt", System.Text.Encoding.UTF8))
                {
                    return JsonConvert.DeserializeObject<List<Recipe>>(streamReader.ReadToEnd(), new JsonSerializerSettings { });
                }
            }

            set
            {
                using (StreamWriter streamWriter = new StreamWriter(@".\Text Files\Recipes.txt", false, System.Text.Encoding.UTF8))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(value));
                }
            }
        }

        // GET: api/Recette
        /// <summary>Get all the available recipes</summary>
        /// <response code="200">Recipes selected</response>
        /// <response code="204">No recipe available</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Recipe>), 200)]
        public IActionResult GetAllRecipes()
        {
            try
            {
                var ParmLRecipes = LRecipes;

                if (ParmLRecipes == null)
                {
                    return NoContent();
                }
                else
                {
                    return new ObjectResult(ParmLRecipes);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        // GET: api/Recette/5
        /// <summary>Select recipe by name</summary>
        /// <param name="name">Recipe's name</param>
        /// <response code="200">Recipe selected</response>
        /// <response code="204">Recipe not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{name}")]
        [ProducesResponseType(typeof(Recipe), 200)]
        public IActionResult GetRecipeByName(string name)
        {
            try
            {
                List<Recipe> ParamListRecipe = LRecipes;                

                if (ParamListRecipe == null || LRecipes.Find(x => x.Name == name) == null)
                {
                    return NoContent();
                }
                else
                {
                    Recipe TargetRecipe = LRecipes.Find(x => x.Name == name);
                    return new ObjectResult(TargetRecipe);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }        
        
        /// <summary>Get recipe's price by name</summary>
        /// <param name="name">Recipe's name</param>
        /// <response code="200">Recipe's price calculated</response>
        /// <response code="204">Recipe not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{name}")]
        public IActionResult GetRecipePriceByName(string name)
        {
            try
            {
                List<Recipe> ParamListRecipe = LRecipes;

                if (ParamListRecipe == null || LRecipes.Find(x => x.Name == name) == null)
                {
                    return NoContent();
                }
                else
                {
                    //Calculer et retourner le prix
                    Recipe TargetRecipe = LRecipes.Find(x => x.Name == name);
                    List<Ingredient> ParamLIngredient = IngredientsController.LIngredients;
                    double Price = 0;
                    foreach(string i in TargetRecipe.DicComposition.Keys)
                    {
                        Price = Price + TargetRecipe.DicComposition[i] * ParamLIngredient.Find(x => x.Name == i).Price;
                    }

                    return new ObjectResult("The price of " + name + " is : " + Price * 1.3);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        // POST: api/Recette        
        /// <summary>Create a new recipe</summary>
        /// <response code="201">Recipe created</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), 201)]
        public IActionResult CreateNewRecipe([FromBody] Recipe value)
        {
            try
            {
                List<Ingredient> ParmListIngredient = IngredientsController.LIngredients;
                if (ParmListIngredient == null) { ParmListIngredient = new List<Ingredient>(); }
                // vérifier si la liste des ingredients séléctionnés figurent bien dans la liste des ingrédients
                foreach (var i in value.DicComposition.Keys)
                {
                    if (!ParmListIngredient.Exists(x => x.Name == i))
                    {
                        throw new Exception("The ingredient " + i + " does not exit in the list of ingredients.");
                    }
                }

                //Rajouter la nouvelle recette à la liste des recettes
                List<Recipe> ParmListRecipe = LRecipes;
                if (ParmListRecipe == null) { ParmListRecipe = new List<Recipe>(); }
                Recipe TargetRecipe = ParmListRecipe.Find(x => x.Name == value.Name);

                if (TargetRecipe != null)
                {
                    ParmListRecipe.Remove(TargetRecipe);
                }

                ParmListRecipe.Add(new Recipe { Name = value.Name, DicComposition = value.DicComposition });
                LRecipes = ParmListRecipe;

                return CreatedAtAction("CreateNewRecipe", "Recipe '" + value.Name + "' was Created.");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        // DELETE: api/ApiWithActions/5
        /// <summary>Delete recipe by name</summary>
        /// <param name="name">Recipe's name</param>
        /// <response code="201">Recipe deleted</response>
        /// <response code="204">Recipe not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{name}")]
        [ProducesResponseType(typeof(string), 201)]
        public IActionResult DeleteRecipe(string name)
        {
            try
            {
                List<Recipe> ParmListRecipe = LRecipes;                

                if (ParmListRecipe == null || ParmListRecipe.Find(x => x.Name == name) == null)
                {
                    return NoContent();
                }
                else
                {
                    Recipe TargetRecipe = ParmListRecipe.Find(x => x.Name == name);
                    ParmListRecipe.Remove(TargetRecipe);
                    LRecipes = ParmListRecipe;

                    return CreatedAtAction("DeleteRecipe", "Recipe '" + name + "' was deleted.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
    }
}
