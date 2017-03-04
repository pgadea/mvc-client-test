using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MVCClientTest.Models.ViewModels;
using System.Text;
using MVCClientTest.Models.Helpers;
using System.Net;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCClientTest.Controllers
{
    public class HomeController : Controller
    {
        HttpClient client;
        public HomeController()
        {
            client = new BookStoreClient().Client;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(int? pageno, int? pagesize, string sort = "Id")
        {
            string contentresult = string.Empty;
            string etagvalue = string.Empty;
            string apiurl = "books" + "?sort=" + sort;

            if (pageno != null)
                apiurl = apiurl + "&pageNo=" + pageno;
            if (pagesize != null)
                apiurl = apiurl + "&pagesize=" + pagesize;


            if (TempData != null && TempData.Count > 0 && TempData["etagforbooks"] != null)
            {
                etagvalue = (string)TempData["etagforbooks"];

                // client.DefaultRequestHeaders.IfNoneMatch.TryParseAdd(etagvalue);
                client.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", etagvalue);
            }

            HttpResponseMessage response = await client.GetAsync(apiurl);

            if (!(response.StatusCode == HttpStatusCode.OK) && !(response.StatusCode == HttpStatusCode.NotModified))
            {
                return Content("An error occured while connecting to web api");
            }

            //PageInfo pageInfo = ReadPageInfroFromHeader(response.Headers);

            string etagHeaderforbooks = ReadETagHeader(response.Headers);


            if (response.StatusCode == HttpStatusCode.NotModified)
            {
                contentresult = (string)TempData["books"];
            }
            else
            {
                contentresult = await response.Content.ReadAsStringAsync();
            }

            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(etagHeaderforbooks))
            {
                TempData["etagforbooks"] = etagHeaderforbooks;
                TempData["books"] = contentresult;
            }

            IEnumerable<BookVM> books = JsonConvert.DeserializeObject<IEnumerable<BookVM>>(contentresult);

            return View(books);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookVM model)
        {
            IEnumerable<AuthorVM> authors = await GetAuthors();
            AuthorVM author = authors.First(a => a.NickName == model.AuthorName.Trim());

            model.Author = author;
            HttpResponseMessage response = await client.PostAsync("books", new StringContent(
                                            JsonConvert.SerializeObject(model),
                                            Encoding.Unicode,
                                            "application/json"));
            if (response.StatusCode != HttpStatusCode.Created)
            {
                return Content("An error occured while posting data to web api");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage response = await client.GetAsync("books/" + id + "/true");
            string contentresult = await response.Content.ReadAsStringAsync();

            BookVM book = JsonConvert.DeserializeObject<BookVM>(contentresult);

            book.AuthorName = book.Author.NickName;

            return View(book);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookVM model)
        {

            IEnumerable<AuthorVM> authors = await GetAuthors();
            AuthorVM author = authors.First(a => a.NickName == model.AuthorName.Trim());

            model.Author = author;
            HttpResponseMessage response = await client.PutAsync("books/" + model.Id, new StringContent(
                                            JsonConvert.SerializeObject(model),
                                            Encoding.UTF8,
                                            "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return Content("An error occured while updating data to web api");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync("books/" + id);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return Content("An error occured while deleting the resource web api");
            }

            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<AuthorVM>> GetAuthors()
        {
            HttpResponseMessage response = await client.GetAsync("authors");
            string contentresult = await response.Content.ReadAsStringAsync();

            IEnumerable<AuthorVM> authors = JsonConvert.DeserializeObject<IEnumerable<AuthorVM>>(contentresult);

            return authors;
        }


        private PageInfo ReadPageInfroFromHeader(HttpResponseHeaders responseHeaders)
        {
            if (responseHeaders.Contains("X-PageInfo"))
            {
                var pageHeaderValue = responseHeaders.First(h => h.Key == "X-PageInfo").Value;

                return JsonConvert.DeserializeObject<PageInfo>(pageHeaderValue.First());

            }

            return null;
        }

        private string ReadETagHeader(HttpResponseHeaders responseHeaders)
        {
            if (responseHeaders.Contains("ETag"))
            {
                var etagHeaderValue = responseHeaders.First(h => h.Key == "ETag").Value;

                return etagHeaderValue.First();

            }

            return null;
        }
    }
}
