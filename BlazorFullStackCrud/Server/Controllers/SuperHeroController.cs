﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFullStackCrud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {

        /*
        public static List<Comic> comics = new List<Comic> {
            new Comic{Id = 1, Name = "Marvel"},
            new Comic {Id = 2, Name = "DC"}
        };

        public static List<SuperHero> heroes = new List<SuperHero> {
            new SuperHero{
                Id = 1, 
                FirstName = "Peter",
                LastName = "Parker",
                HeroName = "Spider Man",
                Comic = comics[0],
                ComicId = 1
            },
            new SuperHero{
                Id = 2,
                FirstName = "Benedict",
                LastName = "C",
                HeroName = "Doctor Strange",
                Comic = comics[1],
                ComicId = 2
            },

        }; 
        */
        private readonly DataContext _context;
        public SuperHeroController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetSuperHeroes()
        {
            var heroes = await _context.SuperHeroes.Include(sh => sh.Comic).ToListAsync();
            return Ok(heroes);
        }

        [HttpGet("comics")]
        public async Task<ActionResult<List<Comic>>> GetComics()
        {
            var comics = await _context.Comics.ToListAsync();
            return Ok(comics);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<SuperHero>> GetSingleHero(int id)
        {
            var hero = await _context.SuperHeroes. Include(h=>h.Comic)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hero == null)
            {
                return NotFound("Sorry, no hero here. :/");
            }
            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<SuperHero>> CreateHero(SuperHero hero)
        {
            hero.Comic = null;
            _context.SuperHeroes.Add(hero);
            await _context.SaveChangesAsync();
            return Ok(await GetDbHeroes());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<SuperHero>>> UpdateSuperHero(SuperHero hero, int id)
        {
            var dbHero = await _context.SuperHeroes.Include(sh => sh.Comic).FirstOrDefaultAsync(sh => sh.Id == id);
            if(dbHero == null)
            {
                return NotFound("Sorry, AcceptVerbsAttribute hero not found :/");
            }
            dbHero.FirstName = hero.FirstName;
            dbHero.LastName = hero.LastName;
            dbHero.HeroName = hero.HeroName;
            dbHero.ComicId = hero.ComicId;

            await _context.SaveChangesAsync();

            return Ok(await GetDbHeroes());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteSuperHero( int id)
        {
            var dbHero = await _context.SuperHeroes.Include(sh => sh.Comic).FirstOrDefaultAsync(sh => sh.Id == id);
            if (dbHero == null)
            {
                return NotFound("Sorry, AcceptVerbsAttribute hero not found :/");
            }

            _context.SuperHeroes.Remove(dbHero); 
            await _context.SaveChangesAsync();

            return Ok(await GetDbHeroes());
        }

        private async Task<List<SuperHero>> GetDbHeroes()
        {
            return await _context.SuperHeroes.Include(sh => sh.Comic).ToListAsync();
        }
    }
}
