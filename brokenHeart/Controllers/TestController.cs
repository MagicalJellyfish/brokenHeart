using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using brokenHeart.DB;
using brokenHeart.Entities.Characters;
using brokenHeart;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public TestController(BrokenDbContext context)
        {
            _context = context;
        }

        // GET: api/Test
        /*[HttpPost]
        public async Task<IActionResult> Init()
        {
            _context.Bodyparts.AddRange(Constants.Bodyparts.BaseBodyparts);
            await _context.SaveChanges();
            return Ok();
        }*/

        /*[HttpDelete]
        public async Task<IActionResult> DeleteIssue()
        {
            Bodypart bodypart = _context.Bodyparts.Where(x => x.Id == 2).First();
            _context.Bodyparts.Remove(bodypart);
            await _context.SaveChanges();
            return Ok();
        }*/
    }
}
