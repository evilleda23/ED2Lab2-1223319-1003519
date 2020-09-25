using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        public MoviesController(IWebHostEnvironment env)
        {
            Storage.Instance.path = env.ContentRootPath + "//testapi.txt";
        }

        [HttpGet]
        public IEnumerable<Movie> Get()
        {

            if (Storage.Instance.Tree != null)
                return Storage.Instance.Tree.Inorden();
            else
                return null;
        }

        [HttpGet]
        [Route("{traversal}")]
        public IEnumerable<Movie> Get(string traversal)
        {

            if (Storage.Instance.Tree == null)
                return null;
            else if (traversal == "preorden")
                return Storage.Instance.Tree.Preorden();
            else if (traversal == "inorden")
                return Storage.Instance.Tree.Inorden();
            else if (traversal == "postorden")
                return Storage.Instance.Tree.Postorden();
            else
                return null;
        }

        [HttpPost]
        public IActionResult Create([FromForm] IFormFile file)
        {
            try
            {
                using var content = new MemoryStream();
                file.CopyToAsync(content);
                var text = Encoding.ASCII.GetString(content.ToArray());
                var deg = JsonSerializer.Deserialize<int>(text);
                Movie testmovie = new Movie();
                Storage.Instance.Tree = new BTree<Movie>(Storage.Instance.path, deg, testmovie.ToFixedString().Length);
                return Ok();
            }
            catch
            {
               return StatusCode(500);
            }
        }

        [HttpDelete]
        public IActionResult Clear()
        {
            if (Storage.Instance.Tree != null)
                Storage.Instance.Tree.Clear();
            return Ok();
        }

        [HttpPost]
        [Route("populate")]
        public IActionResult Add([FromForm] IFormFile file)
        {
            try
            {
                if (Storage.Instance.Tree != null)
                {
                    using var content = new MemoryStream();
                    file.CopyToAsync(content);
                    var text = Encoding.ASCII.GetString(content.ToArray());
                    var list = JsonSerializer.Deserialize<List<Movie>>(text, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    foreach (Movie item in list)
                    {
                        item.SetID();
                        Storage.Instance.Tree.Add(item);
                    }
                    return Ok();
                }
                else
                    return StatusCode(500);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [Route("populate/{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                if (Storage.Instance.Tree != null)
                {
                    var item = new Movie();
                    item.SetID(id);
                    if (Storage.Instance.Tree.Delete(item))
                        return Ok();
                    else
                        return NotFound();
                }
                else
                    return StatusCode(500);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
