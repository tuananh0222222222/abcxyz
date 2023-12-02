using Bogus;
using demodockerv2.webapi.Data;
using demodockerv2.webapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace demodockerv2.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public StudentController(AppDbContext dbContext,
            IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        private async Task Add()
        {
            var faker = new Faker<Student>();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            for (int i = 0; i < 100000; i++)
            {
                faker.RuleFor(s => s.Name, f => f.Name.FullName());
                faker.RuleFor(s => s.Class, f => f.Lorem.Word());
                faker.RuleFor(s => s.Image, f => f.Image.LoremFlickrUrl());
                faker.RuleFor(s => s.Gender, true);

                await _dbContext.Students.AddAsync(faker);
                await _dbContext.SaveChangesAsync();

                Log.Information("Tao ban ghi - " + i);
            }
            stopwatch.Stop();

            Log.Information($"Tao mat thoi gian la: {stopwatch.Elapsed} ");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
          
            await Add();
            var students = _memoryCache.Get<IEnumerable<Student>>("myModelCacheKey");

            if (students == null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(relative: TimeSpan.FromDays(1));

                students = await _dbContext.Students.ToListAsync();

                _memoryCache.Set("myModelCacheKey", students, cacheOptions);

                Log.Information("Cache -" + students);
            }

            return Ok(students);
        }

        // GET api/<StudentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
         
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return BadRequest("Khong tim thay");
            return Ok(student);
        }

        // POST api/<StudentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();
            return Content("Tao thanh cong");
        }

        // PUT api/<StudentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Student student)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var std = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (std == null) return BadRequest("Khong tim thay id ");

            std.Name = student.Name;
            std.Class = student.Class;
            std.Gender = student.Gender;

            _dbContext.Students.Update(std);
            await _dbContext.SaveChangesAsync();

            return Content("Cap nhat thanh cong");
        }

        // DELETE api/<StudentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            if (student == null) return BadRequest("Khong tim thay student");

            _dbContext.Students.Remove(student);
            await _dbContext.SaveChangesAsync();

            return Content("Xoa thanh cong");
        }
    }
}