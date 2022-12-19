using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using URLShortening.Data;
using URLShortening.Entity;

namespace URLShortening.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLsController : ControllerBase
    {
        private readonly UrlDbContext _context;

        public URLsController(UrlDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<URL>>> GetUrls()
        {
            return await _context.Urls.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<URL>> GetURLWithId(int id)
        {
            var url = await _context.Urls.FindAsync(id);

            if (url == null)
            {
                return NotFound();
            }
            return url;
        }

        [HttpGet("fullUrl/{fullUrl}")]
        public async Task<ActionResult<string>> GetURLWithFullUrl(string fullUrl)
        {
            fullUrl = System.Uri.UnescapeDataString(fullUrl);
            var urls = await GetUrls();
            foreach (var url in urls.Value) 
            {
                if (url.FullUrl == fullUrl) 
                {
                    return url.ShortUrl;
                }
            }
            return BadRequest("URL Not Found");
        }

        [HttpGet("shortUrl/{shortUrl}")]
        public async Task<ActionResult<string>> GetURLWithShortUrl(string shortUrl)
        {
            shortUrl = System.Uri.UnescapeDataString(shortUrl);
            var urls = await GetUrls();
            foreach (var url in urls.Value)
            {
                if (url.ShortUrl == shortUrl)
                {
                    return url.FullUrl;
                }
            }
            return BadRequest("URL Not Found");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutURL(int id, URL url)
        {
            if (id != url.Id)
            {
                return BadRequest();
            }
            url = await SetShortUrl(url);
            _context.Urls.Update(url);
            _context.Entry(url).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!URLExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<URL>> PostURL(URL url)
        {
            url = await SetShortUrl(url);
            if (url == null) { 
                return BadRequest("Url is in wrong format");
            }
            _context.Urls.Add(url);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetURLWithId", new { id = url.Id }, url);
        }

        private async Task<URL> SetShortUrl(URL url) 
        {
            string[] fullUrl = url.FullUrl.Split('/');
            if (fullUrl.Length == 1)
            {
                return null;
            }
            if (url.ShortUrl.Split('/').Length > 3)
            {
                return url; //Short url already set
            }
            string shortUrl = await CheckHashedValue(fullUrl[fullUrl.Length - 1]);
            url.ShortUrl = String.Empty;
            for (int i = 0; i < 3; i++) 
            {
                url.ShortUrl += fullUrl[i] + '/';
            }
            url.ShortUrl += shortUrl;
            
            return url;
        }

        private async Task<string> CheckHashedValue(string urlFull) 
        {
            string shortUrl = new HashMap().GetHashedValue(urlFull);
            if (shortUrl != null)
            {
                var urls = await GetUrls();
                foreach (var i in urls.Value)
                {
                    if (i.ShortUrl == shortUrl)
                    {
                        shortUrl = await CheckHashedValue(urlFull);
                    }
                }
            }
            return shortUrl;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteURL(int id)
        {
            var url = await _context.Urls.FindAsync(id);
            if (url == null)
            {
                return NotFound();
            }

            _context.Urls.Remove(url);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllURL()
        {
            var urls = await GetUrls();

            foreach (var url in urls.Value) { 
                _context.Urls.Remove(url);
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool URLExists(int id)
        {
            return _context.Urls.Any(e => e.Id == id);
        }
    }
}
